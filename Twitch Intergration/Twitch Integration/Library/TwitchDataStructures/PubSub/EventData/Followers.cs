using Newtonsoft.Json;
using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.DataSubStructures;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData
{
    /// <summary>
    /// The data delivered by a new follower event
    /// The data is guessed as we had no record.
    /// </summary>
    [Serializable]
    public class PubSubNewFollowerEventData : PubSubEventMessageData
    {
        /// <summary>
        /// Login name of the person who followed
        /// </summary>
        [JsonProperty("user_name")]
        public string UserName { get; private set; }

        /// <summary>
        /// Display name of the person who followed
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        /// <summary>
        /// Name of the channel that has been followed
        /// </summary>
        [JsonProperty("channel_name")]
        public string ChannelName { get; private set; }

        /// <summary>
        /// User ID of the person who followed
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; private set; }

        /// <summary>
        /// ID of the channel that has been followed
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; private set; }

        /// <summary>
        /// Time when the follow was completed
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }
    }
}