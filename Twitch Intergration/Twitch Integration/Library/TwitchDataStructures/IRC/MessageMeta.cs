using Firesplash.UnityAssets.TwitchIntegration.DataTypes.General;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.DataSubStructures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC.Metadata
{
    /// <summary>
    /// The IRC-Tags of a message
    /// </summary>
    public class IRCMessageTags
    {
        /// <summary>
        /// The login name of the sender
        /// </summary>
        [JsonProperty("login")]
        public string Login { get; private set; }

        /// <summary>
        /// The display name of the sender
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        /// <summary>
        /// A hexadecimal color string like #FF00FF which indicates the user's chosen name color
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; private set; }

        /// <summary>
        /// The list of emotes and their position in the text
        /// </summary>
        [JsonProperty("emotes")]
        public List<EmoteInChat> Emotes { get; private set; }

        /// <summary>
        /// The list of badges the sender has
        /// </summary>
        [JsonProperty("badges")]
        public List<UserBadge> Badges { get; private set; }
    }


    /// <summary>
    /// The metadata of the sender/receiver of an IRC message
    /// </summary>
    [Serializable]
    public class Chatter
    {
        /// <summary>
        /// The user id of the Chatter
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }

        /// <summary>
        /// The login name of the Chatter
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// The display name of the Chatter
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; internal set; }

        /// <summary>
        /// A hexadecimal color string like #FF00FF which indicates the Chatter's chosen name color. This can be null or empty string if it has never been set.
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; internal set; }

        /// <summary>
        /// The badges the Chatter has
        /// </summary>
        [JsonProperty("badges")]
        public List<UserBadge> Badges { get; internal set; }
    }
}