using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC
{
    public class IRCEventNotice
    {
        /// <summary>
        /// The channel where this message has been received in
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Thy type of this event message (msg-id)
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The message twitch sent along with this event
        /// NOTE: Not all events contain a message!
        /// </summary>
        public string Message { get; }

        internal IRCEventNotice(MatchCollection noticeMetaMatches, Dictionary<string,string> tags)
        {
            Channel = noticeMetaMatches[0].Groups[3].Captures[0].Value;
            if (noticeMetaMatches[0].Groups.Count >= 5 && noticeMetaMatches[0].Groups[4].Captures.Count >= 1 && noticeMetaMatches[0].Groups[4].Captures[0].Value.Length > 2) Message = noticeMetaMatches[0].Groups[4].Captures[0].Value.Substring(2);
            if (tags.ContainsKey("msg-id")) Type = tags["msg-id"];
        }
    }

    /// <summary>
    /// This event is the same for Hosts or Raiders. It's origin can be distinguished by the IsRaidEvent field.
    /// Please not that for hosts two events may appear (Types host_success and host_success_viewers)
    /// </summary>
    public class IRCIncomingViewersEvent : IRCEventNotice
    {
        /// <summary>
        /// The login name of the channel where the viewers originated from (the raider/hoster)
        /// </summary>
        public string OriginatingChannel;

        /// <summary>
        /// The display name of the channel where the viewers originated from (the raider/hoster)
        /// </summary>
        public string OriginatingChannelDisplayName;

        /// <summary>
        /// The number of viewers sent to the channel
        /// </summary>
        public int ViewerCount;

        /// <summary>
        /// True, if the event is an incoming raid, false for a host
        /// </summary>
        public bool IsRaidEvent;

        internal IRCIncomingViewersEvent(MatchCollection noticeMetaMatches, Dictionary<string, string> tags) : base(noticeMetaMatches, tags)
        {
            IsRaidEvent = tags["msg-id"].Equals("raid");
            if (tags.ContainsKey("msg-param-login")) OriginatingChannel = tags["msg-param-login"];
            if (tags.ContainsKey("msg-param-displayName")) OriginatingChannelDisplayName = tags["msg-param-displayName"];
            try
            {
                ViewerCount = 0;
                if (!IsRaidEvent && tags["msg-id"].Equals("host_success_viewers") && Message != null && Message.Length > 5)
                {
                    Regex vierwerCountExtractRegex = new Regex(@"\:(.*?)\s.+?(\d+) viewers", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    MatchCollection vierwerCountExtractMatches = vierwerCountExtractRegex.Matches(Message);
                    if (vierwerCountExtractMatches != null && vierwerCountExtractMatches.Count >= 1 && vierwerCountExtractMatches[0].Groups.Count >= 3)
                    {
                        ViewerCount = int.Parse(vierwerCountExtractMatches[0].Groups[2].Captures[0].Value);
                    }
                }
                else if (IsRaidEvent && tags.ContainsKey("msg-param-viewerCount"))
                {
                    ViewerCount = int.Parse(tags["msg-param-viewerCount"]);
                }
            }
            catch (Exception) { };
        }
    }
}
