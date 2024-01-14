using Firesplash.UnityAssets.TwitchIntegration.Skeletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Firesplash.UnityAssets.TwitchIntegration.WebGL
{
    /// <summary>
    /// This Behaviour connects to the Twitch Chat and provides an easy to use Event-drive api to handle chat messages
    /// </summary>
    public class TwitchChatWebGL : TwitchChat
    {
#if UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void TwitchWSCreateInstance(string instanceName, string callingClass, string targetAddress);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void TwitchWSSend(string instanceName, string callingClass, string message);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void TwitchWSClose(string instanceName, string callingClass);

        string wsState = "closed";
        bool isOpen = false;
#endif

        internal TwitchChatWebGL(string goName, string ircTcpHost, int ircTcpPort, string ircWSHost, bool debugMode) : base(goName, ircTcpHost, ircTcpPort, ircWSHost, debugMode)
        {
#if UNITY_WEBGL
            Log("Using WebGL implementation.");
#else
            Log("Somehow we use the WebGL implementation while the code was compiled for native version. This will fail.");
#endif
        }

#if UNITY_WEBGL
        internal override void _Connect(string botUserName, string botOAuthToken, string channelName, string commandIdentifier)
        {
            botName = botUserName.ToLower();
            botOAuth = (botOAuthToken.StartsWith("oauth:") ? "" : "oauth:") + botOAuthToken;
            targetChannel = channelName.ToLower();
            this.commandIdentifier = commandIdentifier;

            TwitchWSCreateInstance(objectName, "TwitchChat", twitchIRCWebSocket);
        }

        internal override void _WriteToIRC(string ircDatagram)
        {
            if (!wsState.Equals("open"))
            {
                Log("Tried to send a message to the Twitch Chat while it was not connected.");
                return;
            }
            TwitchWSSend(objectName, "TwitchChat", ircDatagram);
        }

        #region WebGL Callbacks
        internal void TwitchWebSocketState_TwitchChat(string state)
        {
            wsState = state.ToLower();
            if (!isOpen && wsState.Equals("open"))
            {
                //join sequence
                IsTagsEnabled = false;

                Log("Authenticating with server");
                _WriteToIRC("PASS " + botOAuth);
                _WriteToIRC("NICK " + botName);
                _WriteToIRC("USER " + botName + " 8 * :" + botName);

                //Capabilities
                _WriteToIRC("CAP REQ :twitch.tv/tags");
                _WriteToIRC("CAP REQ :twitch.tv/commands");

                //Join the target channel
                if (targetChannel != null) JoinChannel(targetChannel);

                Log("Client connected and ready to serve");

                isOpen = true;

                OnChatConnected?.Invoke();

            }
            else if (state.ToLower().Equals("close"))
            {
                if (isOpen) OnChatDisconnected?.Invoke();
                isOpen = false;
            }
        }

        internal void TwitchWebSocketMessage_TwitchChat(string message)
        {
            if (message.Length < 1) return;

            ParseIRCMessage(message.Trim());
        }
        #endregion

        internal override bool _IsConnected()
        {
            return isOpen && wsState.Equals("open");
        }


#endif
    }
}
