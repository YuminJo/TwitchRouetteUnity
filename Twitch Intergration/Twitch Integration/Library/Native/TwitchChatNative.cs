using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System;
using System.Net.Security;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using Firesplash.UnityAssets.TwitchIntegration.Skeletons;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC;

namespace Firesplash.UnityAssets.TwitchIntegration.Native
{
    /// <summary>
    /// This Behaviour connects to the Twitch Chat and provides an easy to use Event-drive api to handle chat messages
    /// </summary>
    public class TwitchChatNative : TwitchChat
    {
        private StreamReader ircInput;
        private StreamWriter ircOutput;
        private TcpClient ircConnection;
        private SslStream ircSSL;
        private Thread ircThread;

        internal TwitchChatNative(string goName, string ircTcpHost, int ircTcpPort, string ircWSHost, bool debugMode) : base(goName, ircTcpHost, ircTcpPort, ircWSHost, debugMode)
        {

        }

        internal override void _Connect(string botUserName, string botOAuthToken, string channelName, string commandIdentifier)
        {
            Log("Starting IRC connect Sequence");
            //Cleanup
            //if (poolBoyThread != null && poolBoyThread.IsAlive) poolBoyThread.Abort();
            if (ircThread != null && ircThread.IsAlive)
            {
                Log("Aborting previous IRC Thread");
                ircThread.Abort();
            }

            if (ircConnection != null && ircConnection.Connected)
            {
                Log("Closing previous connection");
                lock (ircConnection)
                {
                    if (ircInput != null) ircInput.Close();
                    if (ircOutput != null) ircOutput.Close();
                    if (ircSSL != null) ircSSL.Close();
                    ircConnection.Close();
                    //ircConnection.Dispose();
                }
            }

            Task.Run(() =>
            {
                //Setup
                botName = botUserName.ToLower();
                botOAuth = (botOAuthToken.StartsWith("oauth:") ? "" : "oauth:") + botOAuthToken;
                targetChannel = channelName.ToLower();
                this.commandIdentifier = commandIdentifier;
                IsTagsEnabled = false;

                //Prepare connection
                Log("Initializing connection");
                ircConnection = new TcpClient(twitchIRCHost, twitchIRCPort);

                Log("Securing connection");
                ircSSL = new SslStream(ircConnection.GetStream(), true);
                ircSSL.AuthenticateAsClient(twitchIRCHost);

                //Get IO-Streams
                Log("Getting IO-Streams");
                ircInput = new StreamReader(ircSSL);
                ircOutput = new StreamWriter(ircSSL);

                Log("Authenticating with server");
                //Authenticate
                ircOutput.WriteLine("PASS " + botOAuth);
                ircOutput.WriteLine("NICK " + botName);
                ircOutput.WriteLine("USER " + botName + " 8 * :" + botName);
                ircOutput.Flush();

                //Capabilities
                ircOutput.WriteLine("CAP REQ :twitch.tv/tags");
                ircOutput.WriteLine("CAP REQ :twitch.tv/commands");

                //Flush the output buffer
                ircOutput.Flush();

                //Employ personnel
                Log("Starting IRC Thread");
                //poolBoyThread = new Thread(new ThreadStart(PoolBoy));
                ircThread = new Thread(new ThreadStart(IRCThread));

                //Start work
                //poolBoyThread.Start();
                ircThread.Start();

                //Join the target channel
                if (targetChannel != null) JoinChannel(targetChannel);

                Log("Client connected and ready to serve");
                OnChatConnected?.Invoke();
            });
        }


        /**
         * The butler is around here all the time waiting to serve requests
         */
        private void IRCThread()
        {
            string message = "";
            while (ircConnection.Connected)
            {
                message = ReadFromIRC();
                if (message == null)
                {
                    Thread.Sleep(50);
                    continue;
                }
                ParseIRCMessage(message);
            }
            OnChatDisconnected?.Invoke();
        }

#region Technical IRC implementation
        internal override void _WriteToIRC(string ircDatagram)
        {
            if (ircConnection == null || !ircConnection.Connected) throw new InvalidOperationException("You cannot send data while disconnected.");
            Log("> TO IRC:     " + ircDatagram);
            ircOutput.WriteLine(ircDatagram);
            ircOutput.Flush();
        }

        private string ReadFromIRC()
        {
            string line = ircInput.ReadLine();
            if (line != null && line.Length < 1)
            {
                ircInput.BaseStream.Flush();
                Log("! FLUSHING IRC INPUT because of faulty behavior (empty messages)");
                return null;
            }
            else if (line == null) return null;

            Log("< FROM IRC:   " + line);
            return line;
        }

        internal override bool _IsConnected()
        {
            return ircConnection.Connected;
        }
#endregion
    }
}
