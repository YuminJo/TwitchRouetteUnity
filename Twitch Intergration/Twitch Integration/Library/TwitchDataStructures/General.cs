using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.General
{
#pragma warning disable CS1591
    /// <summary>
    /// This is primarily used internally for connection state tracking
    /// </summary>
    public enum ConnectionState { DISCONNECTED, CONNECTED, CONNECTING, RECONNECTING, DISCONNECTING, ERROR };
#pragma warning restore CS1591 // Fehlender XML-Kommentar für öffentlich sichtbaren Typ oder Element


    /// <summary>
    /// This class represents a user's essential identification data on twitch
    /// </summary>
    [Serializable]
    public class TwitchUser
    {
        /// <summary>
        /// The user ID of the represented user
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The login name of the represented user
        /// </summary>
        [JsonProperty("login")]
        public string Login { get; private set; }

        /// <summary>
        /// The display name of the represented user
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }
    }

    /// <summary>
    /// This class represents a badge a user can have in the chat
    /// </summary>
    [Serializable]
    public class UserBadge
    {
        /// <summary>
        /// The twitch internal ID of this badge
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }

        /// <summary>
        /// The version of this badge
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; internal set; }
    }

    /// <summary>
    /// This represents a single Emote at any point in the text.
    /// </summary>
    [Serializable]
    public class EmoteInChat
    {
        /// <summary>
        /// The zero-based first character in the string which shall be replaced by the image
        /// </summary>
        [JsonProperty("start")]
        public int Start { get; private set; }

        /// <summary>
        /// The LENGTH of the text which represents this Emote (e.g. Kappa = 5)
        /// </summary>
        [JsonProperty("end")]
        public int End { get; private set; }

        /// <summary>
        /// The Emote ID - You can use it to request the image URL at the Helix API
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; private set; }
    }

    /// <summary>
    /// The current progress of the running hype train
    /// </summary>
    [Serializable]
    public class HypeTrainProgress
    {
        /// <summary>
        /// Current HypeTrain Level with all metadata
        /// </summary>
        [JsonProperty("level")]
        public HypeTrainLevel Level { get; private set; }

        /// <summary>
        /// The value of this specific contribution
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; private set; }

        /// <summary>
        /// Unknown
        /// </summary>
        [JsonProperty("goal")]
        public int Goal { get; private set; }

        /// <summary>
        /// The total yet contributed hype including this contribution
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; private set; }

        /// <summary>
        /// Unknown - Probably the remaining seconds for this level
        /// </summary>
        [JsonProperty("remaining_seconds")]
        public int RemainingSeconds { get; private set; }
    }

    /// <summary>
    /// An Object representing a HypeTrain Level with metadata
    /// </summary>
    [Serializable]
    public class HypeTrainLevel
    {
        /// <summary>
        /// The current level
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; private set; }

        /// <summary>
        /// The Hype-Goal
        /// </summary>
        [JsonProperty("goal")]
        public int Goal { get; private set; }

        /// <summary>
        /// The rewards offered in this level
        /// </summary>
        [JsonProperty("rewards")]
        public List<HypeTrainReward> Rewards { get; private set; }
    }

    /// <summary>
    /// Represents a reward for participating in a hype train level
    /// </summary>
    [Serializable]
    public class HypeTrainReward
    {
        /// <summary>
        /// Thy type of this reward - Usually EMOTE
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The (unique?) ID of this reward
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// Unknown
        /// </summary>
        [JsonProperty("group_id")]
        public string GroupId { get; private set; }

        /// <summary>
        /// unknown
        /// </summary>
        [JsonProperty("reward_level")]
        public int RewardLevel { get; private set; }
    }

    /// <summary>
    /// URLs to Images
    /// </summary>
    [Serializable]
    public class TwitchImageURLs
    {
        /// <summary>
        /// Small icon URL
        /// </summary>
        [JsonProperty("url_1x")]
        public string Url1x { get; private set; }

        /// <summary>
        /// Medium icon URL
        /// </summary>
        [JsonProperty("url_2x")]
        public string Url2x { get; private set; }

        /// <summary>
        /// Big icon URL
        /// </summary>
        [JsonProperty("url_4x")]
        public string Url4x { get; private set; }
    }
}