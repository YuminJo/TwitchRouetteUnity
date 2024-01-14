using Firesplash.UnityAssets.TwitchIntegration.DataTypes.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.DataSubStructures
{
    /// <summary>
    /// This represents an upgrad in the Bits Badge Tier
    /// </summary>
    [Serializable]
    public class BadgeEntitlement
    {
        /// <summary>
        /// The badge level the user has just reached
        /// </summary>
        [JsonProperty("new_version")]
        public int NewVersion { get; private set; }

        /// <summary>
        /// The Badge, the user had before the "levelup"
        /// </summary>
        [JsonProperty("previous_version")]
        public int PreviousVersion { get; private set; }
    }

    /// <summary>
    /// This structure represents a chat message sent along with a PubSub event like the text for a shared resub
    /// </summary>
    [Serializable]
    public class PubSubEventChatMessage
    {
        /// <summary>
        /// The body of the user-entered message when sharing this event.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; private set; }

        /// <summary>
        /// A list of emotes and their position in the message. For example "Hey Kappa !" would contain one entry with start position 4 and end 5
        /// </summary>
        [JsonProperty("emotes")]
        public List<EmoteInChat> Emotes { get; private set; }
    }
}