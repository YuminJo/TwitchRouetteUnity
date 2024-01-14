using Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Events;

namespace Firesplash.UnityAssets.TwitchIntegration.Skeletons
{
    /// <summary>
    /// This class provides access to the chat features
    /// </summary>
    public class TwitchChat
    {
        /// <summary>
        /// This Exception is thrown, when we were not able to parse an IRC message which was identified as chat message
        /// </summary>
        public class MessageParsingException : Exception
        {
            /// <summary>
            /// This Exception is thrown, when we were not able to parse an IRC message which was identified as chat message
            /// </summary>
            public MessageParsingException(Exception innerException, string rawIRCLine) : base("An error occured while parsing the following message from twitch IRC: " + rawIRCLine, innerException) { }
        }




        /// <summary>
        /// The Event which is fired when the connection has been established
        /// </summary>
        public class ChatConnectedEvent : UnityEvent { }
        /// <summary>
        /// The Event which is fired when the connection has been dropped
        /// </summary>
        public class ChatDisconnectedEvent : UnityEvent { }
        /// <summary>
        /// The Event which is fired when a normal chat message is received (excluding commands)
        /// </summary>
        public class ChatMessageReceivedEvent : UnityEvent<IRCMessage> { }
        /// <summary>
        /// The Event which is fired when a command message is received in the chat
        /// </summary>
        public class ChatCommandReceivedEvent : UnityEvent<IRCCommand> { }
        /// <summary>
        /// This event fires when the chat is eighter completely cleared or a user is cleared out. First parameter is the channel, second is the user or null if it's a complete clear.
        /// </summary>
        public class ChatClearedEvent : UnityEvent<string, string> { }
        /// <summary>
        /// To be able to implement unsupported things, this event fires, when we receive a string from the IRC which is not supported natively by this library
        /// </summary>
        public class ChatReceivedUnknownMessageEvent : UnityEvent<string> { }



        /// <summary>
        /// This event is fired when viewers are sent to the channel eighter through a raid or host
        /// </summary>
        public class OnIncomingViewersEvent : UnityEvent<IRCIncomingViewersEvent> { }




        /// <summary>
        /// This event is fired when the connection has been established and the bot is authenticated (ready to send messages)
        /// </summary>
        public ChatConnectedEvent OnChatConnected;
        /// <summary>
        /// This event is fired when the connection to the chat has been interrupted
        /// </summary>
        public ChatDisconnectedEvent OnChatDisconnected;
        /// <summary>
        /// This event is fired when a normal message (without the command identifier) is received
        /// </summary>
        public ChatMessageReceivedEvent OnChatMessageReceived;
        /// <summary>
        /// This event is fired when a command message starting with the command identifier is received
        /// </summary>
        public ChatCommandReceivedEvent OnChatCommandReceived;
        /// <summary>
        /// This event is fired when a normal message (without the command identifier) is received as a WHISPER
        /// </summary>
        public ChatMessageReceivedEvent OnWhisperMessageReceived;
        /// <summary>
        /// This event is fired when a command message starting with the command identifier is received as a WHISPER
        /// </summary>
        public ChatCommandReceivedEvent OnWhisperCommandReceived;
        /// <summary>
        /// This event fires when the chat is eighter completely cleared or a user is cleared out. First parameter is the channel, second is the user's login name or null if it's a complete clear.
        /// </summary>
        public ChatClearedEvent OnChatCleared;
        /// <summary>
        /// To be able to implement unsupported things, this event fires, when we receive a string from the IRC which is not supported natively by this library
        /// </summary>
        public ChatReceivedUnknownMessageEvent OnChatReceivedUnknownMessage;



        /// <summary>
        /// This Event is fired when another channel raids a channel we listen to
        /// </summary>
        public OnIncomingViewersEvent OnIncomingRaid;

        /// <summary>
        /// This Event is fired when another channel hosts a channel we listen to
        /// This event is the clean implementation of twitch's hosting notifications BUT it does not always fire for any host. For example zero viewer hosts often get dropped. This is an issue on twitch's side.
        /// You can use OnIncomingHostInformation to get an event for every host event but this does not provide us with technical data like tags and badges or viewer count.
        /// </summary>
        public OnIncomingViewersEvent OnIncomingHost;

        /// <summary>
        /// This Event is fired when twitch informs about an incoming host.
        /// The difference to OnIncomingHost is, that this event is fired for ANY incoming host but has no technical data. The only thing we know is the DISPLAY name of the hosting channel.
        /// This event does send a string to the callback method containing the hosting channel's DISPLAY name
        /// </summary>
        public UnityEvent<string> OnIncomingHostInformation;




        internal string objectName = "";

        /// <summary>
        /// The host name to connect to
        /// </summary>
        public string twitchIRCHost = "irc.chat.twitch.tv";
        /// <summary>
        /// The port of twitch's irc
        /// </summary>
        public int twitchIRCPort = 6697;
        /// <summary>
        /// The WebSocket to connect to in WebGL Mode
        /// </summary>
        public string twitchIRCWebSocket = "wss://irc-ws.chat.twitch.tv";
        /// <summary>
        /// Setting this to true will cause detailed log output. It's made for development only.
        /// </summary>
        public bool DebugMode = false;


        /// <summary>
        /// Did the twitch chat recognize our tags request?
        /// </summary>
        public bool IsTagsEnabled { get; internal set; } = false;

        /// <summary>
        /// Did the twitch chat recognize our commands request?
        /// </summary>
        public bool IsReceivingCommands { get; internal set; } = false;

        //ChatMessage: @badge-info=;badges=moderator/1,bits-charity/1;client-nonce=8d856bb76ced1ba0b707f42a00d6bf0a;color=#CC6900;display-name=FiresplashTV;emotes=;flags=;id=c588b04a-205a-4cf1-8921-ffda4f94f473;mod=1;room-id=240028952;subscriber=0;tmi-sent-ts=1598522260249;turbo=0;user-id=151974255;user-type=mod :firesplashtv!firesplashtv@firesplashtv.tmi.twitch.tv PRIVMSG #firesplashtest :!x
        //Usernotice (thanks to BarryCarlyon): @badge-info=;badges=turbo/1;color=#9ACD32;display-name=TestChannel;emotes=;id=3d830f12-795c-447d-af3c-ea05e40fbddb;login=testchannel;mod=0;msg-id=raid;msg-param-displayName=TestChannel;msg-param-login=testchannel;msg-param-viewerCount=15;room-id=56379257;subscriber=0;system-msg=15\sraiders\sfrom\sTestChannel\shave\sjoined\n!;tmi-sent-ts=1507246572675;tmi-sent-ts=1507246572675;turbo=1;user-id=123456;user-type= :tmi.twitch.tv USERNOTICE #othertestchannel
        internal Regex regexChatMsg = new Regex(@"\:([a-zA-Z0-9_]+)\!([a-zA-Z0-9_]+)\@([a-zA-Z0-9_]+)\.(.*?)\.twitch\.tv PRIVMSG \#([a-zA-Z0-9_]+) \:(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal Regex regexSystemChatMsg = new Regex(@"\:([a-zA-Z0-9_]+)\!([a-zA-Z0-9_]+)\@([a-zA-Z0-9_]+)\.(.*?)\.twitch\.tv PRIVMSG ([a-zA-Z0-9_]+) \:(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal Regex regexWhisperMsg = new Regex(@"\:([a-zA-Z0-9_]+)\!([a-zA-Z0-9_]+)\@([a-zA-Z0-9_]+)\.(.*?)\.twitch\.tv WHISPER ([a-zA-Z0-9_]+) \:(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal Regex regexUsernotice = new Regex(@"\:(.*?)\.twitch\.tv (USERNOTICE|NOTICE) \#([a-zA-Z0-9_]+)( \:.*)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal Regex regexClearChat = new Regex(@"\:(.*?)\.twitch\.tv CLEARCHAT \#([a-zA-Z0-9_]+)( \:([a-zA-Z0-9_]+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal Regex regexMessageTags = new Regex(@"@([a-zA-Z0-9\-]+=.*?[; ])*\:", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal string commandIdentifier;
        internal string botName;
        internal string botOAuth;
        internal string targetChannel;



        internal void Log(string text)
        {
            if (DebugMode) TIDispatcher.Instance.Enqueue(new Action(() => { UnityEngine.Debug.Log("[Twitch Chat]" + text); }));
        }

        internal TwitchChat(string goName, string ircTcpHost, int ircTcpPort, string ircWSHost, bool debugMode)
        {
            DebugMode = debugMode;
            if (DebugMode) Log("Initializing...");
            OnChatConnected = new ChatConnectedEvent();
            OnChatDisconnected = new ChatDisconnectedEvent();
            OnChatCommandReceived = new ChatCommandReceivedEvent();
            OnChatMessageReceived = new ChatMessageReceivedEvent();
            OnWhisperCommandReceived = new ChatCommandReceivedEvent();
            OnWhisperMessageReceived = new ChatMessageReceivedEvent();
            OnChatCleared = new ChatClearedEvent();
            OnChatReceivedUnknownMessage = new ChatReceivedUnknownMessageEvent();

            OnIncomingRaid = new OnIncomingViewersEvent();
            OnIncomingHost = new OnIncomingViewersEvent();
            OnIncomingHostInformation = new UnityEvent<string>();

            objectName = goName;
            twitchIRCHost = ircTcpHost;
            twitchIRCPort = ircTcpPort;
            twitchIRCWebSocket = ircWSHost;
        }


        /// <summary>
        /// Connects to the Twitch Chat using the credentials provided
        /// </summary>
        /// <param name="botUserName">The Twitch user, the bot should login with</param>
        /// <param name="botOAuthToken">A valid OAuth-Token fo the bot - starting with "oauth:"</param>
        public void Connect(string botUserName, string botOAuthToken)
        {
            targetChannel = botUserName;
            _Connect(botUserName, botOAuthToken, botUserName, "!");
        }

        /// <summary>
        /// Connects to the Twitch Chat using the credentials provided
        /// </summary>
        /// <param name="botUserName">The Twitch user, the bot should login with</param>
        /// <param name="botOAuthToken">A valid OAuth-Token fo the bot - starting with "oauth:"</param>
        /// <param name="channelName">The name of the channel to join (optional, defaults to the bots own channel)</param>
        public void Connect(string botUserName, string botOAuthToken, string channelName)
        {
            targetChannel = channelName;
            _Connect(botUserName, botOAuthToken, channelName, "!");
        }

        /// <summary>
        /// Connects to the Twitch Chat using the credentials provided
        /// </summary>
        /// <param name="botUserName">The Twitch user, the bot should login with</param>
        /// <param name="botOAuthToken">A valid OAuth-Token fo the bot - starting with "oauth:"</param>
        /// <param name="channelName">The name of the channel to join (optional, defaults to the bots own channel)</param>
        /// <param name="commandIdentifier">The identifier for commands (optional, default "!")</param>
        public void Connect(string botUserName, string botOAuthToken, string channelName, string commandIdentifier)
        {
            targetChannel = channelName;
            _Connect(botUserName, botOAuthToken, channelName, commandIdentifier);
        }

        internal virtual void _Connect(string botUserName, string botOAuthToken, string channelName, string commandIdentifier) { }




        /// <summary>
        /// Sends a message to the main chat channel (the one used as paremeter when connecting)
        /// </summary>
        /// <param name="chatMessageText">The chat message to send</param>
        public void SendMessageToChannel(string chatMessageText) {
            SendMessageToChannel(targetChannel, chatMessageText);
        }

        /// <summary>
        /// Sends a message to a channel
        /// </summary>
        /// <param name="targetChannel">The channel to send a message into</param>
        /// <param name="chatMessageText">The chat message to send to the channel</param>
        public void SendMessageToChannel(string targetChannel, string chatMessageText) {
            _WriteToIRC(":" + botName + "!" + botName + "@" + botName + ".tmi.twitch.tv PRIVMSG #" + targetChannel + " :" + chatMessageText);
        }

        /// <summary>
        /// Sends a whisper message to the specified user
        /// Beware of the strict rate limits twitch is applying!
        /// </summary>
        /// <param name="targetUserLoginName">The user's login name to send a message to (Do NOT use the display name!)</param>
        /// <param name="whisperMessageText">The chat message to send to the user</param>
        public void SendWhisperMessage(string targetUserLoginName, string whisperMessageText) {
            _WriteToIRC(":" + botName + "!" + botName + "@" + botName + ".tmi.twitch.tv PRIVMSG #jtv :/w " + targetUserLoginName + " " + whisperMessageText);
        }

        internal virtual void _WriteToIRC(string ircDatagram) { }


        /// <summary>
        /// Tells, if the library is connected to twitch chat
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return _IsConnected();
        }

        internal virtual bool _IsConnected() { return false; }


        /// <summary>
        /// Joins an additional channel
        /// </summary>
        /// <param name="channelName">The name of the channel to join</param>
        public void JoinChannel(string channelName) {
            _WriteToIRC(":" + botName + "!" + botName + "@" + botName + ".tmi.twitch.tv JOIN #" + channelName);
        }





        internal void ParseIRCMessage(string message)
        {
            if (message?.Length < 1)
            {
                return; //message is invalid
            }
            else if (message.StartsWith("PING"))
            {
                _WriteToIRC("PONG :tmi.twitch.tv");
            }
            else if (message.Contains("CAP * ACK :twitch.tv/tags"))
            {
                //:tmi.twitch.tv CAP * ACK :twitch.tv/tags
                IsTagsEnabled = true;
            }
            else if (message.Contains("CAP * ACK :twitch.tv/commands"))
            {
                //:tmi.twitch.tv CAP * ACK :twitch.tv/tags
                IsReceivingCommands = true;
            }
            else if (message.Contains("End of /NAMES list"))
            {
                //it is a successful join response but we ignore this for now. probably a TODO for later lib versions
            }
            else if (regexChatMsg.IsMatch(message))
            {
                //:firesplashtest!firesplashtest@firesplashtest.tmi.twitch.tv PRIVMSG #firesplashtest :Text beginnt hier
                //< FROM IRC:   @badge-info=subscriber/25;badges=broadcaster/1,subscriber/12,bits-charity/1;color=#CC6900;display-name=FiresplashTV;emotes=;flags=;id=bb614d4d-baf2-49da-9681-6fb308cf496a;mod=0;room-id=151974255;subscriber=1;tmi-sent-ts=1584465627617;turbo=0;user-id=151974255;user-type= :firesplashtv!firesplashtv@firesplashtv.tmi.twitch.tv PRIVMSG #firesplashtv :khjgkhj
                MatchCollection matches = regexChatMsg.Matches(message);
                if (matches.Count < 1) return;
                try
                {
                    MatchCollection badgeCollection = regexMessageTags.Matches(message);
                    IRCCommand msg = new IRCCommand(commandIdentifier, matches, badgeCollection);
                    if (msg.IsCommand)
                    {
                        Log("Last received message is a command");
                        TIDispatcher.Instance.Enqueue(new Action(() => { OnChatCommandReceived?.Invoke(msg); }));
                    }
                    else
                    {
                        Log("Last received message is a normal chat message");
                        TIDispatcher.Instance.Enqueue(new Action(() => { OnChatMessageReceived?.Invoke((IRCMessage)msg); }));
                    }
                }
                catch (Exception e)
                {
                    Log("Throwing MessageParsingException as the message was malformed");
                    throw new MessageParsingException(e, message);
                }
            }
            else if (regexWhisperMsg.IsMatch(message))
            {
                //:firesplashtest!firesplashtest@firesplashtest.tmi.twitch.tv PRIVMSG #firesplashtest :Text beginnt hier
                //< FROM IRC:   @badges=;color=;display-name=FiresplashTest;emotes=;message-id=1;thread-id=151974255_240028952;turbo=0;user-id=240028952;user-type= :firesplashtest!firesplashtest@firesplashtest.tmi.twitch.tv WHISPER firesplashtv :hi
                MatchCollection matches = regexWhisperMsg.Matches(message);
                if (matches.Count < 1) return;
                try
                {
                    MatchCollection badgeCollection = regexMessageTags.Matches(message);
                    IRCCommand msg = new IRCCommand(commandIdentifier, matches, badgeCollection);
                    if (msg.IsCommand)
                    {
                        Log("Last received whisper is a command");
                        TIDispatcher.Instance.Enqueue(new Action(() => { OnWhisperCommandReceived?.Invoke(msg); }));
                    }
                    else
                    {
                        Log("Last received whisper is a normal chat message");
                        TIDispatcher.Instance.Enqueue(new Action(() => { OnWhisperMessageReceived?.Invoke((IRCMessage)msg); }));
                    }
                }
                catch (Exception e)
                {
                    Log("Throwing MessageParsingException as the whisper was malformed");
                    throw new MessageParsingException(e, message);
                }
            }
            else if (regexSystemChatMsg.IsMatch(message))
            {
                MatchCollection systemMsgMatchCollection = regexSystemChatMsg.Matches(message);
                if (systemMsgMatchCollection.Count >= 1 && systemMsgMatchCollection[0].Groups.Count >= 7 && systemMsgMatchCollection[0].Groups[6].Captures.Count >= 1)
                {
                    if (systemMsgMatchCollection[0].Groups[6].Captures[0].Value.Contains("is now hosting you"))
                    {
                        //this is a host notice
                        string origin = systemMsgMatchCollection[0].Groups[6].Captures[0].Value.Substring(0, systemMsgMatchCollection[0].Groups[6].Captures[0].Value.IndexOf(" is now"));
                        TIDispatcher.Instance.Enqueue(new Action(() => { OnIncomingHostInformation.Invoke(origin); }));
                    }
                }
            }
            else if (regexClearChat.IsMatch(message))
            {
                MatchCollection clearChatMsgMatchCollection = regexClearChat.Matches(message);
                if (clearChatMsgMatchCollection.Count >= 1 && clearChatMsgMatchCollection[0].Groups.Count > 2)
                {
                    OnChatCleared.Invoke(clearChatMsgMatchCollection[0].Groups[2].Captures[0].Value, (clearChatMsgMatchCollection[0].Groups.Count > 2 ? clearChatMsgMatchCollection[0].Groups[3].Captures[0].Value : null));
                }
            }
            else if (regexUsernotice.IsMatch(message))
            {
                MatchCollection noticeMessageMatches = regexUsernotice.Matches(message);
                if (noticeMessageMatches.Count < 1) return;
                try
                {
                    MatchCollection tagsCollection = regexMessageTags.Matches(message);
                    IRCMessage usernotice = new IRCMessage("", null, tagsCollection); //IRCMessage does not really contain a message - as well as many of the fields - in this case. It's only used for LIMITED metadata extraction
                    //string channel = noticeMetaMatches[0].Groups[3].Captures[0].Value;

                    if (usernotice.Tags.ContainsKey("msg-id"))
                    {
                        switch (usernotice.Tags["msg-id"])
                        {
                            case "raid":
                                //@badge-info=;badges=;color=;display-name=FiresplashTest;emotes=;flags=;id=407651ca-4175-4cca-ab80-cfec4f5e60fc;login=firesplashtest;mod=0;msg-id=raid;msg-param-displayName=FiresplashTest;msg-param-login=firesplashtest;msg-param-profileImageURL=https://static-cdn.jtvnw.net/jtv_user_pictures/2710e2f6-3e44-4534-985c-ecfb59de4826-profile_image-70x70.png;msg-param-viewerCount=1;room-id=151974255;subscriber=0;system-msg=1\sraiders\sfrom\sFiresplashTest\shave\sjoined!;tmi-sent-ts=1621765269121;user-id=240028952;user-type= :tmi.twitch.tv USERNOTICE #firesplashtv
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnIncomingRaid?.Invoke(new IRCIncomingViewersEvent(noticeMessageMatches, usernotice.Tags)); }));
                                break;

                            case "host_success":
                            case "host_success_viewers":
                                TIDispatcher.Instance.Enqueue(new Action(() => { OnIncomingHost?.Invoke(new IRCIncomingViewersEvent(noticeMessageMatches, usernotice.Tags)); }));
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log("Throwing MessageParsingException as the message was malformed");
                    throw new MessageParsingException(e, message);
                }
            }
            else
            {
                Log("Last received message is of an unknown type");
                TIDispatcher.Instance.Enqueue(new Action(() => { OnChatReceivedUnknownMessage?.Invoke(message); }));
            }
        }
    }
}