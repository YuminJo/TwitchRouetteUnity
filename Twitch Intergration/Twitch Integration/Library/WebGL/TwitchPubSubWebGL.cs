using Firesplash.UnityAssets.TwitchIntegration.Skeletons;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Firesplash.UnityAssets.TwitchIntegration.WebGL
{
    /// <summary>
    /// The PubSub instance connects to Twitch's PubSub and provides an easy to use API for it.
    /// </summary>
    public class TwitchPubSubWebGL : TwitchPubSub
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void TwitchWSCreateInstance(string instanceName, string callingClass, string targetAddress);

        [DllImport("__Internal")]
        private static extern void TwitchWSSend(string instanceName, string callingClass, string message);

        [DllImport("__Internal")]
        private static extern void TwitchWSClose(string instanceName, string callingClass);
#endif

        internal TwitchPubSubWebGL(string goName, string wssURL, bool debugMode) : base(goName, wssURL, debugMode)
        {
#if UNITY_WEBGL
            Log("Using WebGL implementation.");
#else
            Log("Somehow we use the WebGL implementation while the code was compiled for native version. This will fail.");
#endif
        }

#if UNITY_WEBGL
        internal override void _Connect()
        {
            TwitchWSCreateInstance(objectName, "TwitchPubSub", pubSubAddress);
        }


        internal override void _SendPubSubMessage(string message)
        {
            if (ConnectionState != DataTypes.General.ConnectionState.CONNECTED)
            {
                Log("Sending data is not possible while ConnectionState is not CONNECTED");
                throw new InvalidOperationException("Cannot send data while disconnected.");
            }

            TwitchWSSend(objectName, "TwitchPubSub", message);
        }




#region WebGL Callbacks
        internal void TwitchWebSocketState_TwitchPubSub(string state)
        {
            if (state.ToLower() == "open")
            {
                Log("Setting ConnectionState to CONNECTED");
                ConnectionState = DataTypes.General.ConnectionState.CONNECTED;

                Log("Invoking OnConnectionEstablished");
                TIDispatcher.Instance.Enqueue(new Action(() => { OnConnectionEstablished?.Invoke(); }));
            }
        }

        internal void TwitchWebSocketMessage_TwitchPubSub(string message)
        {
            ParseMessage().Invoke(message); //websocket message incoming
        }
#endregion
#endif
    }
}
