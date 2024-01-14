using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Events = Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Events;

namespace Firesplash.UnityAssets.TwitchIntegration.Skeletons
{
    /// <summary>
    /// This class provides access to the PubSub features
    /// </summary>
    public class TwitchPubSub
    {
        #region Event Class Definitions
        /// <summary>
        /// This Event gets fired when the connection to the websocket is established. You should subscribe to the topics in it's callback.
        /// </summary>
        public class ConnectionEstablishedEvent : UnityEvent { }
        /// <summary>
        /// This Event gets fired when an exception occures within the Pubsub implementation
        /// </summary>
        public class ErrorEvent : UnityEvent<Exception> { }
        /// <summary>
        /// This Event gets fired when any event is sent by the PubSub server. Using this event in combination with SubscribeGeneric, you can subscribe to any event - even unsupported ones.
        /// </summary>
        public class TopicSubscriptionConfirmedEvent : UnityEvent<string[]> { }
        /// <summary>
        /// This Event gets fired when any event is sent by the PubSub server. Using this event in combination with SubscribeGeneric, you can subscribe to any event - even unsupported ones.
        /// </summary>
        public class TopicSubscriptionDeclinedEvent : UnityEvent<string[], string> { }
        /// <summary>
        /// This Event gets fired when any event is sent by the PubSub server. Using this event in combination with SubscribeGeneric, you can subscribe to any event - even unsupported ones.
        /// </summary>
        public class GenericEvent : UnityEvent<string, string> { }
        /// <summary>
        /// This Event gets fired when A Bits V1 or V2 event is received
        /// </summary>
        public class BitsEvent : UnityEvent<Events.PubSubBitsEvent> { }
        /// <summary>
        /// This Event gets fired when a Bits Badge gets unlocked
        /// </summary>
        public class BitsBadgeUnlockEvent : UnityEvent<Events.PubSubBitsBadgeUnlockEvent> { }
        /// <summary>
        /// This Event gets fired when a viewer claims Point Rewards
        /// </summary>
        public class PointRewardEvent : UnityEvent<Events.PubSubPointRewardEvent> { }
        /// <summary>
        /// This Event gets fired when a viewer subscribes to a monitored channel
        /// </summary>
        public class SubscribeEvent : UnityEvent<Events.PubSubSubscribeEvent> { }
        /// <summary>
        /// This Event gets fired when a viewer follows a monitored channel
        /// </summary>
        public class NewFollowerEvent : UnityEvent<Events.PubSubNewFollowerEvent> { }
        /// <summary>
        /// This Event gets fired when a viewer buys games through the channel
        /// </summary>
        public class CommerceEvent : UnityEvent<Events.PubSubCommerceEvent> { }
        /// <summary>
        /// This Event gets fired when a whisper is received on a subscribed channel (user)
        /// </summary>
        public class WhisperEvent : UnityEvent<Events.PubSubWhisperEvent> { }
        /// <summary>
        /// This Event gets fired when a HypeTrain progresses or raises it's level in a monitored channel
        /// </summary>
        public class HypeTrainProgressEvent : UnityEvent<Events.PubSubHypeTrainProgressEvent> { }
        
        /// <summary>
        /// This Event gets fired when a HypeTrain progresses or raises it's level in a monitored channel
        /// </summary>
        public class HypeTrainLevelUpEvent : UnityEvent<Events.PubSubHypeTrainLevelUpEvent> { }
        #endregion



        internal string objectName = "";
        internal string pubSubAddress = "wss://pubsub-edge.twitch.tv";

        /// <summary>
        /// Setting this to true will cause detailed log output. It's made for development only.
        /// </summary>
        public bool DebugMode = false;
        /// <summary>
        /// If you set this to true, it will suppredd Debug.LogWarning-Outputs about using experimental or undocumented features.
        /// Recommendation: Don't do this. Really. I mean it.
        /// </summary>
        public bool suppressExperimentalFeatureWarnings = false;






        /// <summary>
        /// Represents the current state of connection
        /// </summary>
        internal Firesplash.UnityAssets.TwitchIntegration.DataTypes.General.ConnectionState ConnectionState = Firesplash.UnityAssets.TwitchIntegration.DataTypes.General.ConnectionState.DISCONNECTED;

        #region Unity Events
        /// <summary>
        /// This event is fired, when the connection becomes ready. The perfect place to send your subscriptions.
        /// Please note that this is also called on a reconnect and you have to re-subscribe on such a reconnect event! Remember that tokens do not last infinitely!
        /// </summary>
        [HideInInspector]
        public UnityEvent OnConnectionEstablished;

        /// <summary>
        /// This event is fired, whenever an error occures during the work of the PubSub-Thread. It's argument is the Exception which is thrown.
        /// </summary>
        public ErrorEvent OnError;

        /// <summary>
        /// This event is fired when a subscription gets confirmed by the server. The string[] it brings is the array of topics you subscribed successfully to.
        /// </summary>
        public TopicSubscriptionConfirmedEvent OnTopicSubscriptionConfirmed;

        /// <summary>
        /// This event is fired when a subscription gets confirmed by the server. The string[] it brings is the array of topics you tried to subscribe to. The second string is the error message sent by Twitch.
        /// </summary>
        public TopicSubscriptionDeclinedEvent OnTopicSubscriptionDeclined;



        /// <summary>
        /// This is a generic event. Every message that is received on the PubSub is sent out using this event in it's original String-Form (data -> message) with no parsing.
        /// You can use this to even subscribe to Events which are not handled by this library natively. The second string is the topic, we received the message under.
        /// </summary>
        public GenericEvent OnGenericEvent;

        /// <summary>
        /// This event is fired, when a Bit Message V1 or V2 Event is received (channel-bits-events-v1 or channel-bits-events-v2).
        /// Hint: The data structure of V1 and V2 is shared, V2 simply omits transmitting some data if unapplicable so those fields might be null. You can control whether you want to receive V1, V2 or both through your subscription.
        /// You can detect the v1 or v2 type by parsing the Topic field we add to the event
        /// </summary>
        public BitsEvent OnBitsEvent;

        /// <summary>
        /// This event is fired, when a Bits Badge Unlock message is received (channel-bits-badge-unlocks).
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public BitsBadgeUnlockEvent OnBitsBadgeUnlock;

        /// <summary>
        /// This event is fired, when a Channel Points Event is received
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public PointRewardEvent OnPointRewardEvent;

        /// <summary>
        /// This event is fired, when a Channel Points Event is received
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public SubscribeEvent OnSubscribeEvent;

        /// <summary>
        /// This event is fired, when a Channel Points Event is received
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public NewFollowerEvent OnNewFollowerEvent;

        /// <summary>
        /// This event is fired, when a Channel Points Event is received
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public CommerceEvent OnCommerceEvent;

        /// <summary>
        /// This event is fired, when a Channel Points Event is received
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public WhisperEvent OnWhisper;

        /// <summary>
        /// This event is fired, when a Hype Train makes progress (for every action that contributes to the Hype Train)
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public HypeTrainProgressEvent OnHypeTrainProgress;

        /// <summary>
        /// This event is fired, when a Hype Train evolves to the enxt level
        /// We extend Twitch's json structure by a field "Topic" that contains the exact topic, the message was received for.
        /// </summary>
        public HypeTrainLevelUpEvent OnHypeTrainLevelUp;

        #endregion




        internal BlockingCollection<Tuple<DateTime, string>> sendQueue = new BlockingCollection<Tuple<DateTime, string>>();
        internal ConcurrentDictionary<string, string[]> subscriptions = new ConcurrentDictionary<string, string[]>();
        float TimeLeftUntilReconnect = -1;
        float TimeLeftUntilPing = 30;





        internal TwitchPubSub(string goName, string wssURL, bool debugMode)
        {
            DebugMode = debugMode;
            pubSubAddress = wssURL;
            objectName = goName;

            //Initialize Events
            OnConnectionEstablished = new UnityEvent();
            OnTopicSubscriptionConfirmed = new TopicSubscriptionConfirmedEvent();
            OnTopicSubscriptionDeclined = new TopicSubscriptionDeclinedEvent();
            OnError = new ErrorEvent();
            OnGenericEvent = new GenericEvent();

            //Specialized events
            OnBitsEvent = new BitsEvent();
            OnBitsBadgeUnlock = new BitsBadgeUnlockEvent();
            OnPointRewardEvent = new PointRewardEvent();
            OnSubscribeEvent = new SubscribeEvent();
            OnNewFollowerEvent = new NewFollowerEvent();
            OnCommerceEvent = new CommerceEvent();
            OnWhisper = new WhisperEvent();
            OnHypeTrainProgress = new HypeTrainProgressEvent();
            OnHypeTrainLevelUp = new HypeTrainLevelUpEvent();

            ConnectionState = Firesplash.UnityAssets.TwitchIntegration.DataTypes.General.ConnectionState.DISCONNECTED;

            //Make sure we got a dispatcher
            TIDispatcher.CheckAvailability();
        }

        internal void Log(string text)
        {
            if (DebugMode) TIDispatcher.Instance.Enqueue(new Action(() => { UnityEngine.Debug.Log("[" + DateTime.UtcNow + "][Twitch PubSub] " + text); }));
        }

        internal void PubSubPong()
        {
            Log("Received PONG from server. Everything seems fine.");
            TimeLeftUntilReconnect = -1;
        }

        internal Action<string> ParseMessage()
        {
            return new Action<string>((msg) => {
                PubSubBaseDatagram psdgram = JsonConvert.DeserializeObject<PubSubBaseDatagram>(msg);
                PubSubEvent psevent;

                switch (psdgram.Type)
                {
                    case "RECONNECT":
                        Log("  -> Message is a RECONNECT order. Reconnecting...");
                        ConnectionState = DataTypes.General.ConnectionState.RECONNECTING;
                        _Connect();
                        return;

                    case "PONG":
                        Log("  -> Message is a RESPONSE to our PING");
                        TIDispatcher.Instance.Enqueue(new Action(() => { PubSubPong(); }));
                        return;

                    case "RESPONSE":
                        psevent = JsonConvert.DeserializeObject<PubSubEvent>(msg);
                        Log("  -> Message is a RESPONSE to nonce '" + psevent.Nonce + "'");

                        string[] subscriptionsBuffer;
                        subscriptions.TryRemove(psevent.Nonce, out subscriptionsBuffer);
                        if (subscriptionsBuffer == null) subscriptionsBuffer = new string[] { "~ Nonce Unknown ~" };

                        if (psevent.Error.Length > 0)
                        {
                            Log("  -> It's an error: " + psevent.Error);
                            TIDispatcher.Instance.Enqueue(new Action(() => { OnTopicSubscriptionDeclined?.Invoke(subscriptionsBuffer, psevent.Error); }));
                        }
                        else
                        {
                            Log("  -> It's a success message!");
                            TIDispatcher.Instance.Enqueue(new Action(() => { OnTopicSubscriptionConfirmed?.Invoke(subscriptionsBuffer); }));
                        }
                        return;

                    case "MESSAGE":
                        psevent = JsonConvert.DeserializeObject<PubSubEvent>(msg);
                        Log("  -> Message is an Event: " + psevent.Data.Topic.Split('.')[0].ToLower());
                        switch (psevent.Data.Topic.Split('.')[0].ToLower())
                        {
                            case "channel-bits-events-v2":
                            case "channel-bits-events-v1":
                                Log("Invoking OnBitsEvent");
                                Events.PubSubBitsEvent be = JsonConvert.DeserializeObject<Events.PubSubBitsEvent>((string)psevent.Data.Message);
                                be.Topic = psevent.Data.Topic;
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnBitsEvent?.Invoke(be); }));
                                break;

                            case "channel-bits-badge-unlocks":
                                Log("Invoking OnBitsEvent");
                                Events.PubSubBitsBadgeUnlockEvent bb = new Events.PubSubBitsBadgeUnlockEvent(psevent.Data.Message);
                                bb.Topic = psevent.Data.Topic;
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnBitsBadgeUnlock?.Invoke(bb); }));
                                break;

                            case "channel-points-channel-v1":
                                Log("Invoking OnPointRewardEvent");
                                Events.PubSubPointRewardEvent pe = JsonConvert.DeserializeObject<Events.PubSubPointRewardEvent>((string)psevent.Data.Message);
                                pe.Topic = psevent.Data.Topic;
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnPointRewardEvent?.Invoke(pe); }));
                                break;

                            case "channel-subscribe-events-v1":
                                Log("Invoking OnSubscribeEvent");
                                Events.PubSubSubscribeEvent se = new Events.PubSubSubscribeEvent(psevent.Data.Message);
                                se.Topic = psevent.Data.Topic;
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnSubscribeEvent?.Invoke(se); }));
                                break;

                            case "following":
                                Log("Invoking OnNewFollowerEvent");
                                Events.PubSubNewFollowerEvent nfe = new Events.PubSubNewFollowerEvent(psevent.Data.Message);
                                nfe.Topic = psevent.Data.Topic;
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnNewFollowerEvent?.Invoke(nfe); }));
                                break;

                            case "channel-commerce-events-v1":
                                Log("Invoking OnCommerceEvent");
                                Events.PubSubCommerceEvent ce = new Events.PubSubCommerceEvent(psevent.Data.Message);
                                ce.Topic = psevent.Data.Topic;
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnCommerceEvent?.Invoke(ce); }));
                                break;

                            case "whispers":
                                Log("Invoking OnWhisper");
                                Events.PubSubWhisperEvent we = new Events.PubSubWhisperEvent(psevent.Data.Message);
                                we.Topic = psevent.Data.Topic;
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnWhisper?.Invoke(we); }));
                                break;

                            case "hype-train-events-v1":
                                if (psevent.Type.ToLower().Equals("hype-train-progression"))
                                {
                                    Log("Invoking OnHypeTrainProgress");
                                    Events.PubSubHypeTrainProgressEvent hp = new Events.PubSubHypeTrainProgressEvent(psevent.Data.Message);
                                    hp.Topic = psevent.Data.Topic;
                                    TIDispatcher.Instance.Enqueue(new Action(() => { OnHypeTrainProgress?.Invoke(hp); }));
                                }
                                else if (psevent.Type.ToLower().Equals("hype-train-level-up"))
                                {
                                    Log("Invoking OnHypeTrainLevelUp");
                                    Events.PubSubHypeTrainLevelUpEvent hp = new Events.PubSubHypeTrainLevelUpEvent(psevent.Data.Message);
                                    hp.Topic = psevent.Data.Topic;
                                    TIDispatcher.Instance.Enqueue(new Action(() => { OnHypeTrainLevelUp?.Invoke(hp); }));
                                }
                                else
                                {
                                    if (!this.suppressExperimentalFeatureWarnings) TIDispatcher.Instance.Enqueue(new Action(() => { UnityEngine.Debug.LogWarning("A HypeTrain topic sent an unsupported event. Report this to the developer: " + msg); }));
                                }
                                break;
                        }

                        try
                        {
                            TIDispatcher.Instance.Enqueue(new Action(() => { OnGenericEvent?.Invoke(msg, psevent.Data.Topic); }));
                        }
                        catch (Exception e)
                        {
                            Log("Could not raise GenericEvent for last received message: " + e.ToString());
                        }
                        break;
                }
            });
        }








        /// <summary>
        /// Actually connects to the servers.
        /// </summary>
        public void Connect()
        {
            _Connect();
        }

        internal virtual void _Connect() { }

        internal virtual void _SendPubSubMessage(string message) { }













        /// <summary>
        /// Using this subscription, you can subscribe to any PubSub event - even if not yet supported by this library natively.
        /// Instead as a filtered Object you will receive the complete JSON "message" sent by twitch through PubSubGenericEvent as the second Event parameter (as a string).
        /// <b>Subscriptions may only be made after a successful connection. Use OnConnectionEstablished-Event!</b>
        /// </summary>
        /// <param name="topics">The Topics to subscribe according to Twitch's documentation packaged as an array of strings</param>
        /// <param name="oAuthToken">The oAuth Bearer Token to use for connecting - The token must be eligible for the required scopes.</param>
        public void SubscribeGeneric(string[] topics, string oAuthToken)
        {
            if (ConnectionState != Firesplash.UnityAssets.TwitchIntegration.DataTypes.General.ConnectionState.CONNECTED)
            {
                Log("Subscribing to topics is not possible while ConnectionState is not CONNECTED");
                throw new InvalidOperationException("You may not subscribe to PubSub-Topics when connection is not ready. You should use the OnConnectionEstablished Event to suscribe or check ConnectionState manually");
            }

            string nonce = Guid.NewGuid().ToString();
            while (!subscriptions.ContainsKey(nonce) && !subscriptions.TryAdd(nonce, topics)) ;

            PubSubListenRequest lr = new PubSubListenRequest(nonce, topics, oAuthToken);
            _SendPubSubMessage(JsonConvert.SerializeObject(lr));
        }



        /// <summary>
        /// Subscribes to the Topic required to receive Bits V1 Events
        /// </summary>
        /// <param name="channels">An Array of strings containing channel IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToBitsEventsV1(string[] channels, string oAuthToken)
        {
            List<string> topics = new List<string>();
            foreach (string channel in channels)
            {
                topics.Add("channel-bits-events-v1." + channel);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }
        /// <summary>
        /// Subscribes to the Topic required to receive Bits V1 Events
        /// </summary>
        /// <param name="channel">The channel ID you want to receive events for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToBitsEventsV1(string channel, string oAuthToken)
        {
            SubscribeToBitsEventsV1(new string[] { channel }, oAuthToken);
        }



        /// <summary>
        /// Subscribes to the Topic required to receive Bits V2 Events
        /// </summary>
        /// <param name="channels">An Array of strings containing channel IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToBitsEventsV2(string[] channels, string oAuthToken)
        {
            List<string> topics = new List<string>();
            foreach (string channel in channels)
            {
                topics.Add("channel-bits-events-v2." + channel);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }
        /// <summary>
        /// Subscribes to the Topic required to receive Bits V2 Events
        /// </summary>
        /// <param name="channel">The channel ID you want to receive events for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToBitsEventsV2(string channel, string oAuthToken)
        {
            SubscribeToBitsEventsV2(new string[] { channel }, oAuthToken);
        }



        /// <summary>
        /// Subscribes to the Topic required to receive Bits Badge Notifications
        /// </summary>
        /// <param name="channels">An Array of strings containing channel IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToBitsBadgeNotifications(string[] channels, string oAuthToken)
        {
            List<string> topics = new List<string>();
            foreach (string channel in channels)
            {
                topics.Add("channel-bits-badge-unlocks." + channel);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }
        /// <summary>
        /// Subscribes to the Topic required to receive Bits Badge Notifications
        /// </summary>
        /// <param name="channel">The channel ID you want to receive events for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToBitsBadgeNotifications(string channel, string oAuthToken)
        {
            SubscribeToBitsBadgeNotifications(new string[] { channel }, oAuthToken);
        }




        /// <summary>
        /// Subscribes to the Topic required to receive Channel Points Events
        /// </summary>
        /// <param name="channels">An Array of strings containing channel IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToChannelPointsEvents(string[] channels, string oAuthToken)
        {
            List<string> topics = new List<string>();
            foreach (string channel in channels)
            {
                topics.Add("channel-points-channel-v1." + channel);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }
        /// <summary>
        /// Subscribes to the Topic required to receive Channel Points Events
        /// </summary>
        /// <param name="channel">The channel ID you want to receive events for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToChannelPointsEvents(string channel, string oAuthToken)
        {
            SubscribeToChannelPointsEvents(new string[] { channel }, oAuthToken);
        }




        /// <summary>
        /// Subscribes to the Topic required to receive Channel Subscriptions
        /// </summary>
        /// <param name="channels">An Array of strings containing channel IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToChannelSubscriptions(string[] channels, string oAuthToken)
        {
            List<string> topics = new List<string>();
            foreach (string channel in channels)
            {
                topics.Add("channel-subscribe-events-v1." + channel);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }
        /// <summary>
        /// Subscribes to the Topic required to receive Channel Subscriptions
        /// </summary>
        /// <param name="channel">The channel ID you want to receive events for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToChannelSubscriptions(string channel, string oAuthToken)
        {
            SubscribeToChannelSubscriptions(new string[] { channel }, oAuthToken);
        }




        /// <summary>
        /// Subscribes to the Topic required to receive new Followers
        /// </summary>
        /// <param name="channels">An Array of strings containing channel IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToNewFollowers(string[] channels, string oAuthToken)
        {
            List<string> topics = new List<string>();
            foreach (string channel in channels)
            {
                topics.Add("following." + channel);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }
        /// <summary>
        /// Subscribes to the Topic required to receive new Followers
        /// </summary>
        /// <param name="channel">The channel ID you want to receive events for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToNewFollowers(string channel, string oAuthToken)
        {
            SubscribeToNewFollowers(new string[] { channel }, oAuthToken);
        }




        /// <summary>
        /// Subscribes to the Topic required to receive Whispers
        /// </summary>
        /// <param name="users">An Array of strings containing user IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToWhispers(string[] users, string oAuthToken)
        {
            List<string> topics = new List<string>();
            foreach (string user in users)
            {
                topics.Add("whispers." + user);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }
        /// <summary>
        /// Subscribes to the Topic required to receive Whispers
        /// </summary>
        /// <param name="user">The user ID you want to receive whispers for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToWhispers(string user, string oAuthToken)
        {
            SubscribeToWhispers(new string[] { user }, oAuthToken);
        }




        /// <summary>
        /// !!! EXPERIMENTAL UNOFFICIAL API !!!
        /// Subscribes to the Topic required to receive Hype Train events.
        /// This topic is undocumented so using it might work, crash your app or cause an atomic war.
        /// Use at your own risk.
        /// </summary>
        /// <param name="users">An Array of strings containing user IDs</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToHypeTrains(string[] users, string oAuthToken)
        {
            if (!this.suppressExperimentalFeatureWarnings) TIDispatcher.Instance.Enqueue(new Action(() => { UnityEngine.Debug.LogWarning("You are subscribing to the undocumented Hype Train API which my change at any time. Use at your own risk and responsibility."); }));
            List<string> topics = new List<string>();
            foreach (string user in users)
            {
                topics.Add("hype-train-events-v1." + user);
            }
            SubscribeGeneric(topics.ToArray(), oAuthToken);
        }

        /// <summary>
        /// Subscribes to the Topic required to receive Whispers
        /// </summary>
        /// <param name="user">The user ID you want to receive whispers for</param>
        /// <param name="oAuthToken">The authorized oAuth token to use for subscription</param>
        public void SubscribeToHypeTrains(string user, string oAuthToken)
        {
            SubscribeToHypeTrains(new string[] { user }, oAuthToken);
        }


        internal void PingPong()
        {
            if (ConnectionState != DataTypes.General.ConnectionState.CONNECTED)
            {
                TimeLeftUntilPing = 15; //We set this to a close but fixed amount of time
                if (ConnectionState == DataTypes.General.ConnectionState.ERROR)
                {
                    ConnectionState = DataTypes.General.ConnectionState.RECONNECTING;
                    Connect();
                }
                return; //Skip
            }

            TimeLeftUntilPing -= Time.unscaledDeltaTime;
            if (TimeLeftUntilPing < 0)
            {
                TimeLeftUntilPing = 150 + UnityEngine.Random.Range(0f, 30f); //150 seconds + jitter
                TimeLeftUntilReconnect = 12f; //Twitch's 10 seconds + 2 seconds internal dispatching grace time
                Log("Sending PING to PubSub...");
                _SendPubSubMessage("{\"type\": \"PING\"}"); //Hey Server! R u still there?
            }

            if (TimeLeftUntilReconnect > -1)
            {
                TimeLeftUntilReconnect -= Time.unscaledDeltaTime;
                if (TimeLeftUntilReconnect < 0)
                {
                    TimeLeftUntilReconnect = -1;
                    Log("Got no PONG from PubSub. Reconnecting...");
                    Connect(); //reconnect
                }
            }
        }
    }
}
