using Firesplash.UnityAssets.TwitchIntegration.DataTypes.General;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC.Metadata;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC
{
    /// <summary>
    /// This object represents a single message in the chat
    /// </summary>
    public class IRCMessage
    {
        /// <summary>
        /// Who sent the message?
        /// </summary>
        public Chatter Sender { get; private set; }

        /// <summary>
        /// The channel where this message has been received in
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// The raw text of the message
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Is this message a bot command?
        /// </summary>
        public bool IsCommand { get; } = false;

        /// <summary>
        /// True, if the sender of this message is subscriber in the channel
        /// </summary>
        public bool IsSenderSubscriber
        {
            get
            {
                return Badges.ContainsKey("subscriber") || Badges.ContainsKey("founder");
            }
        }

        /// <summary>
        /// True, if the sender of this message is subscriber in the channel
        /// </summary>
        public bool IsSenderVIP
        {
            get
            {
                return Badges.ContainsKey("vip");
            }
        }

        /// <summary>
        /// True, if the sender of this message is the broadcaster in the channel
        /// </summary>
        public bool IsSenderBroadcaster
        {
            get
            {
                return Badges.ContainsKey("broadcaster");
            }
        }

        /// <summary>
        /// True, if the sender of this message is the broadcaster in the channel
        /// </summary>
        public bool IsSenderModerator
        {
            get
            {
                return Badges.ContainsKey("moderator");
            }
        }

        /// <summary>
        /// This provides a parsed and better usable access to the badges than Sender.Badges does.
        /// If you would try to find out the subscriber months, the subscriber badge would probably show a wrong looking
        /// value as the badges are for the "levels" whereas the exact number of months sits inside the Tag badge-info!
        /// Please referr to Twitchs documentation: https://dev.twitch.tv/docs/irc/tags
        /// </summary>
        public Dictionary<string, int> Badges;

        /// <summary>
        /// A dictionary containing all Tags the message has attached. The key represents the badge type, The value is null if there is no data for it available.
        /// </summary>
        public Dictionary<string, string> Tags { get; }

        /// <summary>
        /// This consturctor is internal to the library.
        /// </summary>
        public IRCMessage(string commandIdentifier, MatchCollection msgMatchCollection, MatchCollection msgTagsCollection)
        {
            //we need to extract the tags
            Tags = new Dictionary<string, string>();
            if (msgTagsCollection != null && msgTagsCollection.Count > 0)
            {
                try
                {
                    foreach (Capture badge in msgTagsCollection[0].Groups[1].Captures)
                    {
                        try
                        {
                            if (badge.Value.Contains("="))
                            {
                                string[] badgeData = badge.Value.Split('=');
                                Tags.Add(badgeData[0], badgeData[1].Substring(0, badgeData[1].Length - 1));
                            }
                            else
                            {
                                Tags.Add(badge.Value.Substring(0, badge.Value.Length - 1), null);
                            }
                        }
                        catch (Exception e)
                        {
                            TIDispatcher.Instance.Enqueue(new Action(() => { UnityEngine.Debug.LogError("[Twitch Chat] " + e.GetType().Name + " while parsing the tag '" + badge.Value + "' of last IRC message: " + e.Message); }));
                        }
                    }
                }
                catch (Exception e)
                {
                    TIDispatcher.Instance.Enqueue(new Action(() => { UnityEngine.Debug.LogError("[Twitch Chat] " + e.GetType().Name + " while parsing tags of last IRC message: " + e.Message); }));
                    UnityEngine.Debug.LogError("[Twitch Chat] " + e.GetType().Name + " while parsing tags of last IRC message: " + e.Message);
                }
            }

            Badges = new Dictionary<string, int>();
            List<UserBadge> badges = new List<UserBadge>();
            if (Tags.ContainsKey("badges") && Tags["badges"] != null)
            {
                foreach (string badge in Tags["badges"].Split(','))
                {
                    string[] badgeData = badge.Split('/');
                    if (badgeData.Length > 1)
                    {
                        badges.Add(new UserBadge()
                        {
                            Id = badgeData[0],
                            Version = badgeData[1]
                        });
                        try
                        {
                            Badges.Add(badgeData[0], int.Parse(badgeData[1]));
                        }
                        catch (FormatException e)
                        {
                            Badges.Add(badgeData[0], 1); //override to at least have the badge
                            UnityEngine.Debug.LogWarning("[Twitch Chat] Error parsing badge level for a chat message: " + badge + " -> " + e.Message);
                        }
                    }
                    else
                    {
                        badges.Add(new UserBadge()
                        {
                            Id = badge,
                            Version = "1"
                        });
                        Badges.Add(badge, 1);
                    }
                }
            }



            if (msgMatchCollection != null)
            {
                //Basic message parsing
                Sender = new Chatter()
                {
                    DisplayName = Tags.ContainsKey("display-name") ? Tags["display-name"] : msgMatchCollection[0].Groups[1].Captures[0].Value,
                    Id = Tags.ContainsKey("user-id") ? Tags["user-id"] : null,
                    Username = msgMatchCollection[0].Groups[1].Captures[0].Value.ToLower(),
                    Badges = badges,
                    Color = Tags.ContainsKey("color") ? Tags["color"] : null
                };
                Text = msgMatchCollection[0].Groups[6].Captures[0].Value;
                Channel = msgMatchCollection[0].Groups[5].Captures[0].Value;

                IsCommand = false;
                if (Text.Trim().StartsWith(commandIdentifier))
                {
                    //Command parsing
                    IsCommand = true;
                }
            }
        }
    }

    /// <summary>
    /// This object represents a chat message that has been identified as a possible command to us. It has been preprocessed for further usage.
    /// </summary>
    public class IRCCommand : IRCMessage
    {
        /// <summary>
        /// The Parameters given to the command as an array. Empty array if no parameters given.
        /// </summary>
        public string[] Parameters { get; private set; }

        /// <summary>
        /// All parameters as a raw text - just as received from twitch
        /// </summary>
        public string ParameterText { get; private set; }

        /// <summary>
        ///  The command we received
        /// </summary>
        public string Command { get; private set; }

        internal IRCCommand(string commandIdentifier, MatchCollection msgMatchCollection, MatchCollection msgTagsCollection) : base(commandIdentifier, msgMatchCollection, msgTagsCollection)
        {
            if (IsCommand)
            {
                if (Text.IndexOf(' ') > 0 && Text.Length > Text.IndexOf(' '))
                {
                    Command = Text.Substring(1, Text.IndexOf(' ')).Trim();
                    ParameterText = Text.Substring(Text.IndexOf(' ') + 1);
                    Parameters = ParameterText.Split(' ');
                }
                else
                {
                    Command = Text.Substring(1);
                    Parameters = new string[] { };
                }
            }
        }
    }
}
