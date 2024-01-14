using Newtonsoft.Json;
using System;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Base;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.EventData;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Events
{
#pragma warning disable CS1591
    [Serializable]
    public class PubSubBitsEvent : PubSubEventMessage
    {
        [JsonProperty("data")]
        new public PubSubBitsEventData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubBitsBadgeUnlockEvent : PubSubEventMessage
    {
        internal PubSubBitsBadgeUnlockEvent(object data)
        {
            //Damn twitch...
            Data = JsonConvert.DeserializeObject<PubSubBitsBadgeUnlockData>(data.ToString());
        }

        [JsonProperty("data")]
        new public PubSubBitsBadgeUnlockData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubPointRewardEvent : PubSubEventMessage
    {
        [JsonProperty("data")]
        new public PubSubPointRewardRedemptionData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubSubscribeEvent : PubSubEventMessage
    {
        internal PubSubSubscribeEvent(object data)
        {
            //Damn twitch...
            Data = JsonConvert.DeserializeObject<PubSubSubscribeEventData>(data.ToString());
        }

        [JsonProperty("data")]
        new public PubSubSubscribeEventData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubNewFollowerEvent : PubSubEventMessage
    {
        internal PubSubNewFollowerEvent(object data)
        {
            //Damn twitch...
            Data = JsonConvert.DeserializeObject<PubSubNewFollowerEventData>(data.ToString());
        }

        [JsonProperty("data")]
        new public PubSubNewFollowerEventData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubCommerceEvent : PubSubEventMessage
    {
        internal PubSubCommerceEvent(object data)
        {
            //Damn twitch...
            Data = JsonConvert.DeserializeObject<PubSubCommerceEventData>(data.ToString());
        }

        [JsonProperty("data")]
        new public PubSubCommerceEventData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubWhisperEvent : PubSubEventMessage
    {
        internal PubSubWhisperEvent(object data)
        {
            //Damn twitch...
            Data = JsonConvert.DeserializeObject<PubSubWhisperEventData>(data.ToString());
        }

        [JsonProperty("data")]
        new public PubSubWhisperEventData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubHypeTrainLevelUpEvent : PubSubEventMessage
    {
        internal PubSubHypeTrainLevelUpEvent(object data)
        {
            //Damn twitch...
            Data = JsonConvert.DeserializeObject<PubSubHypeTrainEventData>(data.ToString());
        }

        [JsonProperty("data")]
        new public PubSubHypeTrainEventData Data { get; private set; } //override only the data property
    }

    [Serializable]
    public class PubSubHypeTrainProgressEvent : PubSubEventMessage
    {
        internal PubSubHypeTrainProgressEvent(object data)
        {
            //Damn twitch...
            Data = JsonConvert.DeserializeObject<PubSubHypeTrainProgressEventData>(data.ToString());
        }

        [JsonProperty("data")]
        new public PubSubHypeTrainProgressEventData Data { get; private set; } //override only the data property
    }
#pragma warning restore CS1591
}
