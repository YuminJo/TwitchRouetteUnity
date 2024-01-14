using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.General;
using Newtonsoft.Json;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData
{
    /// <summary>
    /// An object representing a point reward
    /// </summary>
    [Serializable]
    public class PointReward
    {
        /// <summary>
        /// The ID for this reward
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The ID of the channel this reward was redeemed on
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; private set; }

        /// <summary>
        /// The title of this reward
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; private set; }

        /// <summary>
        /// The prompt the user had to enter text for (if any)
        /// </summary>
        [JsonProperty("prompt")]
        public string Prompt { get; private set; }

        /// <summary>
        /// The cost in channel points for this reward
        /// </summary>
        [JsonProperty("cost")]
        public int Cost { get; private set; }

        /// <summary>
        /// True if the user had to answer a prompt
        /// </summary>
        [JsonProperty("is_user_input_required")]
        public bool IsUserInputRequired { get; private set; }

        /// <summary>
        /// True if this can only be redeemed by subscribers
        /// </summary>
        [JsonProperty("is_sub_only")]
        public bool IsSubOnly { get; private set; }

        /// <summary>
        /// The image object for this point reward
        /// </summary>
        [JsonProperty("image")]
        public TwitchImageURLs Image { get; private set; }

        /// <summary>
        /// The default image for this type of reward. Use, if no image set or loading fails.
        /// </summary>
        [JsonProperty("default_image")]
        public TwitchImageURLs DefaultImage { get; private set; }

        /// <summary>
        /// The background color for the reward button
        /// </summary>
        [JsonProperty("background_color")]
        public string BackgroundColor { get; private set; }

        /// <summary>
        /// True if the reward is enabled (Huh? Can viewers redeem disabled rewards?!)
        /// </summary>
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// True if the reward is currently paused (Huh? Can viewers redeem disabled rewards?!)
        /// </summary>
        [JsonProperty("is_paused")]
        public bool IsPaused { get; private set; }

        /// <summary>
        /// True if the reward is ins tock (Huh? Can viewers redeem out-of-stock rewards?!)
        /// </summary>
        [JsonProperty("is_in_stock")]
        public bool IsInStock { get; private set; }

        /// <summary>
        /// Maximum stock per stream info for this reward
        /// </summary>
        [JsonProperty("max_per_stream")]
        public PointRewardMaxPerStream MaxPerStream { get; private set; }

        /// <summary>
        /// True, if there is no approval needed by the streamer (false if it lands in the approval queue first)
        /// </summary>
        [JsonProperty("should_redemptions_skip_request_queue")]
        public bool ShouldRedemptionsSkipRequestQueue { get; private set; }
    }

    /// <summary>
    /// Maximum stock per stream info for a reward
    /// </summary>
    [Serializable]
    public class PointRewardMaxPerStream
    {
        /// <summary>
        /// Is stock limited?
        /// </summary>
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// If limited, this is the stock per stream
        /// </summary>
        [JsonProperty("max_per_stream")]
        public int MaxPerStream { get; private set; }
    }

    /// <summary>
    /// Information about an actual redemption
    /// </summary>
    [Serializable]
    public class PointRewardRedemption
    {
        /// <summary>
        /// The unique rdemption transaction id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The Twitch USer Object representing the user who redeemed it
        /// </summary>
        [JsonProperty("user")]
        public TwitchUser User { get; private set; }

        /// <summary>
        /// The ID of the channel where the reward was redeemed on
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; private set; }

        /// <summary>
        /// The timestamp when this reward was redeemed
        /// </summary>
        [JsonProperty("redeemed_at")]
        public DateTime RedeemedAt { get; private set; }

        /// <summary>
        /// The reward object which was redeemed
        /// </summary>
        [JsonProperty("reward")]
        public PointReward Reward { get; private set; }

        /// <summary>
        /// The message which was entered by the user for this reward
        /// </summary>
        [JsonProperty("user_input")]
        public string UserInput { get; private set; }

        /// <summary>
        /// The status of this reward
        /// Twitch docs: reward redemption status, will be FULFULLED if a user skips the reward queue, UNFULFILLED otherwise
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; private set; }
    }

    /// <summary>
    /// A data object for reward redemption metadata
    /// </summary>
    [Serializable]
    public class PubSubPointRewardRedemptionData
    {
        /// <summary>
        /// The timestamp when teh redemption occured
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// The actual redemption object
        /// </summary>
        [JsonProperty("redemption")]
        public PointRewardRedemption Redemption { get; private set; }
    }
}