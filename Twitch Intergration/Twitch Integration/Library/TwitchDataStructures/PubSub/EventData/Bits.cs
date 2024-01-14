using Newtonsoft.Json;
using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.DataSubStructures;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData
{
    /// <summary>
    /// The data contained in a bits badge unlock notice
    /// </summary>
    [Serializable]
    public class PubSubBitsBadgeUnlockData : PubSubEventMessageData
    {
        /// <summary>
        /// The ID of the twitch user who unlocked the badge
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; private set; }

        /// <summary>
        /// The ID of the twitch user who unlocked the badge
        /// </summary>
        [JsonProperty("user_name")]
        public string UserName { get; private set; }

        /// <summary>
        /// The ID of the channel where the badge was unlocked
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; private set; }

        /// <summary>
        /// The name of the channel where the badge was unlocked
        /// </summary>
        [JsonProperty("channel_name")]
        public string ChannelName { get; private set; }

        /// <summary>
        /// Value of Bits badge tier that was earned (1000, 10000, etc.)
        /// </summary>
        [JsonProperty("badge_tier")]
        public int BadgeTier { get; private set; }

        /// <summary>
        /// [Optional] Custom message included with share - May be null.
        /// </summary>
        [JsonProperty("chat_message")]
        public string ChatMessage { get; private set; }

        /// <summary>
        /// Time when the new Bits badge was earned.
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }
    }

    /// <summary>
    /// The data contained in a Bits V1 or V2 message
    /// </summary>
    [Serializable]
    public class PubSubBitsEventData : PubSubEventMessageData
    {
        /// <summary>
        /// The login of the twitch user who cheered
        /// </summary>
        [JsonProperty("user_name")]
        public string UserName { get; private set; }

        /// <summary>
        /// The name of the channel where the cheer happened
        /// </summary>
        [JsonProperty("channel_name")]
        public string ChannelName { get; private set; }

        /// <summary>
        /// The ID of the twitch user who cheered
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; private set; }

        /// <summary>
        /// The ID of the channel where the cheer happened
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; private set; }

        /// <summary>
        /// Time when the Bits were used.
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }

        /// <summary>
        /// Chat message sent with the cheer.
        /// </summary>
        [JsonProperty("chat_message")]
        public string ChatMessage { get; private set; }

        /// <summary>
        /// Number of bits used.
        /// </summary>
        [JsonProperty("bits_used")]
        public int BitsUsed { get; private set; }

        /// <summary>
        /// All time total number of Bits used in the channel by this specific user.
        /// </summary>
        [JsonProperty("total_bits_used")]
        public int TotalBitsUsed { get; private set; }

        /// <summary>
        /// Event type associated with this use of Bits.
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// Information about a user’s new badge level, if the cheer was not anonymous and the user reached a new badge level with this cheer. Otherwise, null.
        /// </summary>
        [JsonProperty("badge_entitlement")]
        public BadgeEntitlement BadgeEntitlement { get; private set; }
    }
}