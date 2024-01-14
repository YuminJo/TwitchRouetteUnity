using Newtonsoft.Json;
using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.DataSubStructures;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData
{
    /// <summary>
    /// The data delivered by a new subscriber event
    /// </summary>
    [Serializable]
    public class PubSubSubscribeEventData : PubSubEventMessageData
    {
        /// <summary>
        /// Login name of the person who subscribed or sent a gift subscription
        /// </summary>
        [JsonProperty("user_name")]
        public string UserName { get; private set; }

        /// <summary>
        /// Display name of the person who subscribed or sent a gift subscription
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        /// <summary>
        /// Name of the channel that has been subscribed or subgifted
        /// </summary>
        [JsonProperty("channel_name")]
        public string ChannelName { get; private set; }

        /// <summary>
        /// User ID of the person who subscribed or sent a gift subscription
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; private set; }

        /// <summary>
        /// ID of the channel that has been subscribed or subgifted
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; private set; }

        /// <summary>
        /// Time when the subscription or gift was completed
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }

        /// <summary>
        /// Subscription Plan ID, values: Prime, 1000, 2000, 3000
        /// </summary>
        [JsonProperty("sub_plan")]
        public string SubPlan { get; private set; }

        /// <summary>
        /// Channel Specific Subscription Plan Name
        /// </summary>
        [JsonProperty("sub_plan_name")]
        public string SubPlanName { get; private set; }

        /// <summary>
        /// Only for Sub-Gifts! Null otherwise. This is not the number of months the user subscribed, but the consecutive number of months a gifter has gifted in the channel
        /// </summary>
        [JsonProperty("months"), Obsolete("This field is deprecated by Twitch.")]
        public int Months { get; private set; }

        /// <summary>
        /// Cumulative number of tenure months of the subscription
        /// </summary>
        [JsonProperty("cumulative_months")]
        public int CumulativeMonths { get; private set; }

        /// <summary>
        /// Denotes the user’s most recent (and contiguous) subscription tenure streak in the channel
        /// </summary>
        [JsonProperty("streak_months")]
        public int StreakMonths { get; private set; }

        /// <summary>
        /// Event type associated with the subscription product, values: sub, resub, subgift, anonsubgift, resubgift, anonresubgift
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; private set; }

        /// <summary>
        /// User ID of the subscription gift recipient
        /// </summary>
        [JsonProperty("recipient_id")]
        public string ReceipientId { get; private set; }

        /// <summary>
        /// Login name of the subscription gift recipient
        /// </summary>
        [JsonProperty("recipient_user_name")]
        public string ReceipientUserName { get; private set; }

        /// <summary>
        /// Display name of the person who received the subscription gift
        /// </summary>
        [JsonProperty("recipient_display_name")]
        public string ReceipientDisplayName { get; private set; }

        /// <summary>
        /// If this sub message was caused by a gift subscription
        /// </summary>
        [JsonProperty("is_gift")]
        public bool IsGift { get; private set; } = false;

        /// <summary>
        /// The message send with the sharing of the event - It may contain emotes.
        /// </summary>
        [JsonProperty("sub_message")]
        public PubSubEventChatMessage SubMessage { get; private set; }

        /// <summary>
        /// Number of months gifted as part of a single, multi-month gift
        /// </summary>
        [JsonProperty("multi_month_duration")]
        public int MultiMonthDuration { get; private set; } = 1;
    }
}