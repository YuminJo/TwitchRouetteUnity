using Newtonsoft.Json;
using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.DataSubStructures;
using System.Collections.Generic;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC.Metadata;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData
{
    /// <summary>
    /// Some strange ID object...
    /// </summary>
    [Serializable]
    public class AbsolutelyStupidPubSubWhisperDataObject
    {
        /// <summary>
        /// Not documented. Probably a message ID?
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; private set; }
    }

    /// <summary>
    /// Unfortunately this message is documented but without any documentation... We tried our best to elaborate.
    /// </summary>
    [Serializable]
    public class PubSubWhisperEventData : PubSubEventMessageData
    {
        /// <summary>
        /// Always whisper_received
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The twitch documentation is not clear on this data structure and the JSON they deliver is invalid. It is just implemented for reference. Blame Twitch. Sorry.
        /// </summary>
        [JsonProperty("data")]
        public AbsolutelyStupidPubSubWhisperDataObject Data { get; private set; }

        /// <summary>
        /// Guess: The new "Reply" feature? Probably this is indicating a reply to another message's Data.Id
        /// </summary>
        [JsonProperty("thread_id")]
        public string ThreadId { get; private set; }

        /// <summary>
        /// The actual chat message containing all emotes as text
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; private set; }

        /// <summary>
        /// A Unix Timestamp when the message was sent
        /// </summary>
        [JsonProperty("sent_ts")]
        public int SentTs { get; private set; }

        /// <summary>
        /// The user id of the chatter who sent the message
        /// </summary>
        [JsonProperty("from_id")]
        public int FromId { get; private set; }

        /// <summary>
        /// The IRC Tages of this message containing additional information
        /// </summary>
        [JsonProperty("tags")]
        public IRCMessageTags Tags { get; private set; }

        /// <summary>
        /// The Chatter-Object of the receiver of this message
        /// </summary>
        [JsonProperty("recipient")]
        public Chatter Recipient { get; private set; }

        /// <summary>
        /// A unique id of the pubsub message
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; private set; }
    }
}