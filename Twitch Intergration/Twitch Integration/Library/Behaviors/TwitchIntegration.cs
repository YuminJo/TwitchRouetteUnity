using Firesplash.UnityAssets.TwitchIntegration.Native;
using Firesplash.UnityAssets.TwitchIntegration.Skeletons;
using Firesplash.UnityAssets.TwitchIntegration.WebGL;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Firesplash.UnityAssets.TwitchIntegration
{
    /// <summary>
    /// This is the main behavior used to communicate with twitch
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Firesplash Entertainment/Twitch Integration/Twitch Integration Manager")]
    public class TwitchIntegration : MonoBehaviour
    {

        /// <summary>
        /// The host name to connect to IRC in native mode (FQDN)
        /// </summary>
        [Header("IRC System")]
        [Space]
        [Space]
        [Header("changing these values using scripts.")]
        [Header("Read the documentation before")]
        [Header("WARNING:")]
        [SerializeField]
        public string twitchIRCHostNative = "irc.chat.twitch.tv";

        /// <summary>
        /// The port of twitch's IRC
        /// </summary>
        [SerializeField]
        public int twitchIRCPortNative = 6697;

        /// <summary>
        /// The Websocket-Server-URL used to connect to Chat in WebGL mode
        /// </summary>
        [SerializeField]
        public string twitchIRCHostWebSocket = "wss://irc-ws.chat.twitch.tv";


        
        /// <summary>
        /// The wss address to use to connect to Twitch's PubSub Edge
        /// </summary>
        [Header("PubSub System")]
        [Space]
        [SerializeField]
        public string twitchPubSubAddress = "wss://pubsub-edge.twitch.tv";

        /// <summary>
        /// Enables detailed logging
        /// </summary>
        [Header("General Settings")]
        [Space]
        [SerializeField]
        public bool DebugMode = false;



        private TwitchChat _chat = null;
        /// <summary>
        /// Access the Chat integration
        /// Warning: First use creates a connection to twitch. You can not alter the Connect-Parameters after connecting.
        /// </summary>
        public TwitchChat Chat
        {
            get
            {
                if (_chat == null)
                {
                    if (Application.platform == RuntimePlatform.WebGLPlayer) _chat = new TwitchChatWebGL(gameObject.name, twitchIRCHostNative, twitchIRCPortNative, twitchIRCHostWebSocket, DebugMode);
                    else _chat = new TwitchChatNative(gameObject.name, twitchIRCHostNative, twitchIRCPortNative, twitchIRCHostWebSocket, DebugMode);
                }
                return _chat;
            }
        }


        private TwitchPubSub _pubsub = null;
        /// <summary>
        /// Access the Chat integration
        /// Warning: First use creates a connection to twitch. You can not alter the Connect-Parameters after connecting.
        /// </summary>
        public TwitchPubSub PubSub
        {
            get
            {
                if (_pubsub == null)
                {
                    if (Application.platform == RuntimePlatform.WebGLPlayer) _pubsub = new TwitchPubSubWebGL(gameObject.name, twitchPubSubAddress, DebugMode);
                    else _pubsub = new TwitchPubSubNative(gameObject.name, twitchPubSubAddress, DebugMode);
                }
                return _pubsub;
            }
        }

        internal void Awake()
        {
            if (TIDispatcher.CheckAvailability())
            {
                if (DebugMode) Debug.Log("TI-Dispatcher has been instantiated successfully.");
            }
        }

        internal void Update()
        {
            if (_pubsub != null) _pubsub.PingPong();
        }

#region WebGL Callback Passthrough
#if UNITY_WEBGL
        private void TwitchWebSocketState_TwitchChat(string state)
        {
            if (_chat != null && Application.platform == RuntimePlatform.WebGLPlayer) ((TwitchChatWebGL)_chat).TwitchWebSocketState_TwitchChat(state);
        }

        private void TwitchWebSocketMessage_TwitchChat(string message)
        {
            if (_chat != null && Application.platform == RuntimePlatform.WebGLPlayer) ((TwitchChatWebGL)_chat).TwitchWebSocketMessage_TwitchChat(message);
        }



        private void TwitchWebSocketState_TwitchPubSub(string state)
        {
            if (_pubsub != null && Application.platform == RuntimePlatform.WebGLPlayer) ((TwitchPubSubWebGL)_pubsub).TwitchWebSocketState_TwitchPubSub(state);
        }

        private void TwitchWebSocketMessage_TwitchPubSub(string message)
        {
            if (_pubsub != null && Application.platform == RuntimePlatform.WebGLPlayer) ((TwitchPubSubWebGL)_pubsub).TwitchWebSocketMessage_TwitchPubSub(message);
        }
#endif
#endregion
    }
}
