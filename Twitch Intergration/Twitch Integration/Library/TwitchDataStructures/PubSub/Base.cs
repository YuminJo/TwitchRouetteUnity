using Newtonsoft.Json;
using System;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base
{
#pragma warning disable CS1591 // Fehledes XML-Kommentar für öffentlich sichtbaren Typ oder Element
    [Serializable]
    public class PubSubBaseDatagram
    {
        [JsonProperty("type")]
        public string Type { get; internal set; }
    }

    [Serializable]
    public class PubSubListenRequestData
    {
        [JsonProperty("topics")]
        public string[] Topics { get; internal set; }

        [JsonProperty("auth_token")]
        public string AuthToken { get; internal set; }

        //[JsonProperty("nonce")]
        //public string Nonce { get; internal set; }
    }

    [Serializable]
    public class PubSubListenRequest : PubSubBaseDatagram
    {
        [JsonProperty("data")]
        public PubSubListenRequestData Data { get; private set; }

        [JsonProperty("nonce")]
        public string Nonce { get; private set; }

        public PubSubListenRequest(string nonce, string[] topics, string token) : base()
        {
            this.Type = "LISTEN";
            this.Nonce = nonce;
            this.Data = new PubSubListenRequestData()
            {
                AuthToken = token,
                Topics = topics
            };
        }
    }

    [Serializable]
    public class PubSubEventData
    {
        [JsonProperty("topic")]
        public string Topic { get; private set; }

        [JsonProperty("message")]
        public object Message { get; private set; }
    };

    [Serializable]
    public class PubSubEvent : PubSubBaseDatagram
    {
        [JsonProperty("data")]
        public PubSubEventData Data { get; private set; } = new PubSubEventData();

        [JsonProperty("nonce")]
        public string Nonce { get; internal set; }

        [JsonProperty("error")]
        public string Error { get; internal set; }
    }

    [Serializable]
    public class PubSubEventMessageData { };

    [Serializable]
    public partial class PubSubEventMessage
    {
        [JsonIgnore]
        public string Topic { get; internal set; }

        /// <summary>
        ///The data payload of this datagram
        /// </summary>
        [JsonProperty("data")]
        public PubSubEventMessageData Data { get; private set; }

        /// <summary>
        /// This field has nothing to do with "v1" or "v2" like it's available for bits!
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; private set; }

        /// <summary>
        /// This is set for some events like hype trains. It can give more information about the event that caused this datagram or about what this datagram represents as twitch sometimes sends only a subset of fields depending on this "Type". Referr to Twitch's documentation if you need further information.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; private set; }

        /// <summary>
        /// Indicates the type of the data - You should not need it as we distinguish for you. This is only set for Bits Events. For all the other events see "Type"
        /// </summary>
        [JsonProperty("message_type")]
        public string MessageType { get; private set; }

        /// <summary>
        /// This is an id which identifies this exact PubSub datagram
        /// </summary>
        [JsonProperty("message_id")]
        public string MessageId { get; private set; }

        /// <summary>
        /// This field is null for most events
        /// </summary>
        [JsonProperty("is_anonymous")]
        public bool IsAnonymous { get; private set; } = false;
    }
#pragma warning restore CS1591 // Fehledes XML-Kommentar für öffentlich sichtbaren Typ oder Element
}