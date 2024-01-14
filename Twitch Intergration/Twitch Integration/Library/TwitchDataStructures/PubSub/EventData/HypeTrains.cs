using Newtonsoft.Json;
using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.General;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData
{
    /// <summary>
    /// The Payload for a Hype Train LevelUp
    /// </summary>
    [Serializable]
    public class PubSubHypeTrainEventData : PubSubEventMessageData
    {
        /// <summary>
        /// Likely this represents the time in seconds until this train expires.
        /// For OnHypeTrainProgress this is always null!
        /// </summary>
        [JsonProperty("time_to_expire")]
        public long TimeToExpire { get; private set; }

        /// <summary>
        /// The current Progress of this train
        /// </summary>
        [JsonProperty("progress")]
        public HypeTrainProgress Progress { get; private set; }
    }

    /// <summary>
    /// The Payload for a Hype Train Progression
    /// </summary>
    [Serializable]
    public class PubSubHypeTrainProgressEventData : PubSubHypeTrainEventData
    {
        /// <summary>
        /// The ID of the twitch user who just contributed to the train
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; private set; }

        /// <summary>
        /// Likely this is the sequence number of the contribution, numbering all contributions one by one consecutively...
        /// </summary>
        [JsonProperty("sequence_id")]
        public int SequenceId { get; private set; }

        /// <summary>
        /// The action the viewer performed to contribute to the train. e.g. CHEER
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; private set; }

        /// <summary>
        /// The source of hype e.g. BITS (can likely also be SUBSCRIPTION or something like that - As this is undocumented: You will have to elaborate. We'd love to get your feedback.)
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; private set; }

        /// <summary>
        /// The amount of "source" used to contribute to the hype
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; private set; }
    }

}