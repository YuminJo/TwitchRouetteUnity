using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firesplash.UnityAssets.TwitchIntegration.Skeletons;

namespace Firesplash.UnityAssets.TwitchIntegration.Native
{
    /// <summary>
    /// The PubSub instance connects to Twitch's PubSub and provides an easy to use API for it.
    /// </summary>
    public class TwitchPubSubNative : TwitchPubSub
    {
        private Thread readerThread, writerThread;
        private ClientWebSocket ws;


        CancellationTokenSource PubSubCTokenSrc, LowLevelCTokenSrc;


        internal TwitchPubSubNative(string goName, string wssURL, bool debugMode) : base(goName, wssURL, debugMode)
        {
            Log("Using NATIVE implementation.");

            //Create the socket
            ws = new ClientWebSocket();
        }

        internal override void _Connect()
        {
            Task.Run(async () =>
            {
                if (PubSubCTokenSrc != null && !PubSubCTokenSrc.Token.IsCancellationRequested)
                {
                    //Cancel all running threads generously
                    Log("Cancelling PubSubCTokenSrc in preparation of a reconnect");
                    PubSubCTokenSrc.Cancel();
                }

                for (int i = 0; i < 30; i++) {
                    if (i == 19)
                    {
                        //After 10 Seconds we will cancel the underlaying websocket work
                        Log("Cencelling LowLevelCTokenSrc in preparation of a reconnect");
                        LowLevelCTokenSrc.Cancel();
                    }
                    else if (i == 21)
                    {
                        //After 11 seconds, abort the remaining threads as they might be deadlocked
                        Log("Aborting remaining threads after timeout");
                        if (readerThread != null && readerThread.IsAlive) readerThread.Abort();
                        if (writerThread != null && writerThread.IsAlive) writerThread.Abort();
                    }

                    //break the loop when all threads are dead
                    if ((readerThread == null || !readerThread.IsAlive) && (writerThread == null || !writerThread.IsAlive))
                    {
                        Log("No (more) threads are running. Continuing connect sequence...");
                        await Task.Delay(500);
                        break;
                    }

                    //On Timeout (after 15 seconds) throw an exception...
                    if (i == 29) throw new ApplicationException("PubSub Worker-Threads did not finish even after abort.");

                    //Wait a moment
                    await Task.Delay(500);
                }

                PubSubCTokenSrc = new CancellationTokenSource();
                LowLevelCTokenSrc = new CancellationTokenSource();
                subscriptions = new ConcurrentDictionary<string, string[]>();

                try
                {
                    lock (ws)
                    {
                        ws = new ClientWebSocket();
                    }
                } 
                catch (Exception e)
                {
                    Log("Received exception while instanitating WebSocket Client: " + e.ToString());
                }

                try
                {
                    Log("Connecting to PubSub on " + pubSubAddress);
                    ConnectionState = DataTypes.General.ConnectionState.CONNECTING;
                    await ws.ConnectAsync(new Uri(pubSubAddress), PubSubCTokenSrc.Token);
                    while (ws.State != WebSocketState.Open) {
                        await Task.Delay(50);
                    }
                    Log("Connected to PubSub Server.");
                }
                catch (Exception e)
                {
                    Log("Exception while connecting to PubSub: " + e.ToString());
                    TIDispatcher.Instance.Enqueue(new Action(() => { OnError?.Invoke(e); }));
                    ConnectionState = DataTypes.General.ConnectionState.DISCONNECTED;
                    return;
                }

                readerThread = new Thread(new ThreadStart(PubSubReader));
                Log("Starting PubSub Reader-Thread");
                readerThread.Start();

                writerThread = new Thread(new ThreadStart(PubSubWriter));
                Log("Starting PubSub Writer-Thread");
                writerThread.Start();

                Log("Setting ConnectionState to CONNECTED");
                ConnectionState = DataTypes.General.ConnectionState.CONNECTED;

                Log("Invoking OnConnectionEstablished");
                TIDispatcher.Instance.Enqueue(new Action(() => { OnConnectionEstablished?.Invoke(); }));
            });
        }





        //Worker 1
        async void PubSubReader()
        {
            string closeReason = "CLIENT SIDE CLOSE";

            while (!PubSubCTokenSrc.Token.IsCancellationRequested)
            {
                var message = "";
                var binary = new List<byte>();

            READ:
                var buffer = new byte[1024];
                WebSocketReceiveResult res = null;

                try
                {
                    res = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), PubSubCTokenSrc.Token);
                    if (PubSubCTokenSrc.Token.IsCancellationRequested) break;
                }
                catch (OperationCanceledException)
                {
                    Log("PubSub Reader has been aborted in action on request (by PubSubCTokenSrc)");
                    ConnectionState = DataTypes.General.ConnectionState.DISCONNECTED;
                    break;
                }
                catch (Exception e) {
                    Log("An Exception caused the PubSubReader to stop unexpectedly: " + e.ToString());
                    ConnectionState = DataTypes.General.ConnectionState.ERROR;
                    break;
                }

                if (res == null)
                    goto READ; //we got nothing. Wait for data.

                if (res.MessageType == WebSocketMessageType.Close)
                {
                    ConnectionState = DataTypes.General.ConnectionState.DISCONNECTING;
                    Log("Received Server-Side-Close");
                    closeReason = "SERVER SIDE CLOSE";
                    break;
                }

                if (res.MessageType == WebSocketMessageType.Text)
                {
                    if (!res.EndOfMessage)
                    {
                        message += Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        goto READ;
                    }
                    message += Encoding.UTF8.GetString(buffer).TrimEnd('\0');

                    Log("Received PubSub datagram, passing to parse action");
                    ParseMessage().Invoke(message);
                }
                else
                {
                    if (!res.EndOfMessage)
                    {
                        goto READ;
                    }

                    Log("Received a non-text message which is not supported. Ignoring it...");
                }
                buffer = null;

            }

            Log("PubSubReader loop has exited");

            ConnectionState = DataTypes.General.ConnectionState.DISCONNECTED;
            try
            {
                Log("Closing Websocket");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, closeReason, LowLevelCTokenSrc.Token);
                Log("Cancelling LowLevelCTokenSrc");
                LowLevelCTokenSrc.Cancel();
            }
            catch (OperationCanceledException)
            {
                Log("PubSub Reader has been aborted while closing websocket on work done (by LowLevelCTokenSrc)");
            }
        }

        /// <summary>
        /// This is a debug function. You cann pass a Pubsub json message string as parameter to test the functionality of this library.
        /// </summary>
        /// <param name="json">The JSON string</param>
        [Obsolete]
        public void DEBUG_ParsePubSub_JSON(string json)
        {
            ParseMessage().Invoke(json);
        }


        //Worker 2
        async void PubSubWriter()
        {
            while (!PubSubCTokenSrc.Token.IsCancellationRequested)
            {
                await Task.Delay(100);
                var msg = sendQueue.Take(PubSubCTokenSrc.Token);
                if (msg.Item1.Add(new TimeSpan(0, 0, 10)) < DateTime.UtcNow)
                {
                    continue;
                }
                var buffer = Encoding.UTF8.GetBytes(msg.Item2);
                try
                {
                    await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, LowLevelCTokenSrc.Token);
                }
                catch (OperationCanceledException)
                {
                    Log("PubSub Writer has been aborted in action on request (by LowLevelCTokenSrc)");
                    break;
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex);
                    Log("Cancelling LowLevelCTokenSrc on Writer exception: " + ex.ToString());
                    LowLevelCTokenSrc.Cancel();
                    ConnectionState = DataTypes.General.ConnectionState.DISCONNECTED;
                    break;
                }
            }

            Log("PubSubWriter loop has exited");
        }


        internal override void _SendPubSubMessage(string message)
        {
            if (ConnectionState != DataTypes.General.ConnectionState.CONNECTED)
            {
                Log("Sending data is not possible while ConnectionState is not CONNECTED");
                throw new InvalidOperationException("Cannot send data while disconnected.");
            }

            try
            {
                Task.Run(() =>
                {
                    sendQueue.Add(new Tuple<DateTime, string>(DateTime.UtcNow, message));
                }).Wait(100, PubSubCTokenSrc.Token);
            }
            catch (OperationCanceledException)
            {
                Log("PubSub Sending has been aborted in action on request (by PubSubCTokenSrc)");
                return;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
            }
        }


    }
}
