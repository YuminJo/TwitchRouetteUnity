using Newtonsoft.Json;
using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.DataSubStructures;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData
{
    /// <summary>
    /// The data delivered by a commerce event
    /// </summary>
    [Serializable]
    public class PubSubCommerceEventData : PubSubEventMessageData
    {
        /// <summary>
        /// The login name of the user who made a transaction
        /// </summary>
        [JsonProperty("user_name")]
        public string UserName { get; private set; }

        /// <summary>
        /// The display name of the user who made a transaction
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        /// <summary>
        /// The name of the channel where the transaction has been made
        /// </summary>
        [JsonProperty("channel_name")]
        public string ChannelName { get; private set; }

        /// <summary>
        /// The id of the user who made a transaction
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; private set; }

        /// <summary>
        /// The id of the channel where the transaction has been made
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; private set; }

        /// <summary>
        /// The time when the transaction was made
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; private set; }

        /// <summary>
        /// The URL to an image of the item which was bought by the user
        /// </summary>
        [JsonProperty("item_image_url")]
        public string ItemImageUrl { get; private set; }

        /// <summary>
        /// The description of the item which was bought by the user
        /// </summary>
        [JsonProperty("item_description")]
        public string ItemDescription { get; private set; }

        /// <summary>
        /// If the transaction supports the channel
        /// </summary>
        [JsonProperty("supports_channel")]
        public bool SupportsChannel { get; private set; }

        /// <summary>
        /// The Umessage given by the user when the commerce was fulfilled. May contain emotes.
        /// </summary>
        [JsonProperty("purchase_message")]
        public PubSubEventChatMessage PurchaseMessage { get; private set; }
    }
}