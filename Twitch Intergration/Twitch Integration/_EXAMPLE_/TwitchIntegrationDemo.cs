using Firesplash.UnityAssets.TwitchIntegration;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.IRC;
using Firesplash.UnityAssets.TwitchIntegration.DataTypes.PubSub.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TwitchIntegrationDemo : MonoBehaviour
{
    public TwitchAlert twitchAlert;
    string ChannelID="", oAuthToken="", channelName="";

    //Only for demo
    public UnityEngine.UI.Text eventView, hypeView, chatView;
    public TMP_InputField fldChId, fldOAToken, fldChanName;

    TwitchIntegration twitch;

    // 추가한것
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Screen Resoultion
#if !UNITY_WEBGL
        Screen.SetResolution(960, 600, false); //This is only for the sample app's UI layout in standalone player!
#endif

        //This is for demo UI
        ChannelID = fldChId.text = PlayerPrefs.GetString("ChId", "");
        channelName = fldChanName.text = PlayerPrefs.GetString("ChName", "");
        //if (PlayerPrefs.GetString("OAToken", "").Length > 1) fldOAToken.text = "Removed for security reasons. Don't change to reuse.";
        //end demo UI
 
        twitch = GetComponent<TwitchIntegration>();



#region PubSub

        //The Library is firing UnityEvents through a dispatcher (the PubSub Library runs in its own thread and schedules the Event Callbacks on the main thread).
        //We need to add a Listener to the OnConnectionEstablished event to do stuff when the asynchronous connect has happened (and after every reconnect)
        twitch.PubSub.OnConnectionEstablished.AddListener(() => {
            Debug.Log("The system successfully established a connection to PubSub - We will now subscribe to the topics using the token " + oAuthToken);

            //We'll listen to a lot of topics here - This has to be done while the connection is established, and has to be re-done after a reconnect so this event is the best place for it.

            if (ChannelID.Length > 0) //Did the user enter a channelID?
            {
                //The user entered a channel ID so we will use it and connect away
                twitch.PubSub.SubscribeToBitsEventsV2(ChannelID, oAuthToken);

                twitch.PubSub.SubscribeToChannelPointsEvents(ChannelID, oAuthToken);

                twitch.PubSub.SubscribeToChannelSubscriptions(ChannelID, oAuthToken);

                twitch.PubSub.SubscribeToNewFollowers(ChannelID, oAuthToken);

                twitch.PubSub.SubscribeToHypeTrains(ChannelID, oAuthToken); //This one will cause a warning because it is experimental (undocumented PubSub topic). It also has two events!

                twitch.PubSub.SubscribeToBitsBadgeNotifications(ChannelID, oAuthToken);
            }
            else
            {
                //The user DID NOT enter a channel ID so we will fetch it from twitch first (this is a free helper we provide due to high demand among customers)
                StartCoroutine(TwitchHelpers.GetChannelIDAndRun(oAuthToken, (resultToken, resultChannelID, rawResult) => {
                    //This code is called by the coroutine after fetching the information from twitch
                    Debug.Log("Hey, your Channel ID is: " + resultChannelID);

                    fldChId.text = resultChannelID;

                    twitchAlert.SendDiscord(resultChannelID);

                    twitch.PubSub.SubscribeToBitsEventsV2(resultChannelID, resultToken);

                    twitch.PubSub.SubscribeToChannelPointsEvents(resultChannelID, resultToken);

                    twitch.PubSub.SubscribeToChannelSubscriptions(resultChannelID, resultToken);

                    twitch.PubSub.SubscribeToNewFollowers(resultChannelID, resultToken);

                    twitch.PubSub.SubscribeToHypeTrains(resultChannelID, resultToken); //This one will cause a warning because it is experimental (undocumented PubSub topic). It also has two events!

                    twitch.PubSub.SubscribeToBitsBadgeNotifications(resultChannelID, resultToken);
                }));
            }
            
        });

        twitch.PubSub.OnGenericEvent.AddListener((string topic, string data) =>
        {
            Debug.Log("Pubsub Event " + topic + ": " + data);
        });


        //If something goes wrong with any of our subscriptions
        twitch.PubSub.OnTopicSubscriptionDeclined.AddListener((string[] topics, string error) =>
        {
            Debug.LogError("Failed to subscribe to topics " + string.Join(", ", topics) + ": " + error);
            if (error.Contains("ERR_BADAUTH")) 
            {
                Application.Quit();
            }

        });

        //Otherwise when everything works as expected
        twitch.PubSub.OnTopicSubscriptionConfirmed.AddListener(topics =>
        {
            Debug.Log("We are now subscribed to: " + string.Join(", ", topics));
            PointRouette.pr.ShowConnecting();
        });




        //As we are listening to topics, we might probably also want to do something with all those notifications:
        twitch.PubSub.OnSubscribeEvent.AddListener((PubSubSubscribeEvent e) => {
            if (e.Data != null && e.Data.IsGift)
            {
                eventView.text = e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.DisplayName + " just gifted a subscription for this channel to " + e.Data.ReceipientUserName + "!";
                Debug.Log(e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.DisplayName + " just gifted a subscription for this channel to " + e.Data.ReceipientUserName + "!");
            }
            else
            {
                eventView.text = e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.DisplayName + " just subscribed to this channel for the " + e.Data.StreakMonths + ". month in a row!";
                Debug.Log(e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.DisplayName + " just subscribed to this channel for the " + e.Data.StreakMonths + ". month in a row!");
            }
        });

        twitch.PubSub.OnNewFollowerEvent.AddListener((PubSubNewFollowerEvent e) => {
            eventView.text = e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.UserName + " just followed the channel " + e.Data.ChannelName;
            Debug.Log(e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.UserName + " just followed the channel " + e.Data.ChannelName);
        });

        twitch.PubSub.OnBitsEvent.AddListener((PubSubBitsEvent e) => {
            eventView.text = e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.UserName + " just cheered with a whole load of " + e.Data.BitsUsed + " Bits!";
            Debug.Log(e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.UserName + " just cheered with a whole load of " + e.Data.BitsUsed + " Bits!");
        });

        twitch.PubSub.OnBitsBadgeUnlock.AddListener((PubSubBitsBadgeUnlockEvent e) => {
            eventView.text = e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.UserName + " just earned the " + e.Data.BadgeTier + " Bits Badge";
            Debug.Log(e.Data.Time.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.UserName + " just earned the " + e.Data.BadgeTier + " Bits Badge");
        });

        twitch.PubSub.OnPointRewardEvent.AddListener((PubSubPointRewardEvent e) => {
            eventView.text = e.Data.Redemption.RedeemedAt.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.Redemption.User.DisplayName + " just redeemed " + e.Data.Redemption.Reward.Title + " for " + e.Data.Redemption.Reward.Cost + " Points.";
            Debug.Log(e.Data.Redemption.RedeemedAt.ToString() + ": " + e.GetType().Name + ": Viewer " + e.Data.Redemption.User.DisplayName + " just redeemed " + e.Data.Redemption.Reward.Title + " for " + e.Data.Redemption.Reward.Cost + " Points.");

            if(e.Data.Redemption.Reward.Title == OptionManager.om.Rouette_Title_Text.text.ToString())
            {
                Debug.Log(e.GetType().Name);
                PointRouette.pr.PlayerList.Add(e.Data.Redemption.User.DisplayName);
                PointRouette.pr.RouetteStack++;
                PointRouette.pr.RouetteCheck_Stack();

                PointRouette.pr.saveArchive.playername = e.Data.Redemption.User.DisplayName;
            }
        });

        twitch.PubSub.OnHypeTrainLevelUp.AddListener((PubSubHypeTrainLevelUpEvent e) => {
            eventView.text = e.GetType().Name + ": The Hype is real! The train is now on level " + e.Data.Progress.Level + " and running for " + e.Data.TimeToExpire + " more seconds";
            Debug.Log(e.GetType().Name + ": The Hype is real! The train is now on level " + e.Data.Progress.Level + " and running for " + e.Data.TimeToExpire + " more seconds");
        });

        twitch.PubSub.OnHypeTrainProgress.AddListener((PubSubHypeTrainProgressEvent e) => {
            hypeView.text = e.GetType().Name + ": Twitch User with ID " + e.Data.UserId + " contributed a " + e.Data.Action + " of value " + e.Data.Quantity + " " + e.Data.Source + " to the hype train.";
            Debug.Log(e.GetType().Name + ": Twitch User with ID " + e.Data.UserId + " contributed a " + e.Data.Action + " of value " + e.Data.Quantity + " " + e.Data.Source + " to the hype train.");        });

        /*
         * As you can see unfortunately in different events things are often named different or even not available (like the DisplayName in BitsEvent)
         * Our library is strongly oriented along Twitch's official data structure so you can always referr to offcial Twitch documentation.
         * This is also the reason why we reflect this chaos.
         * We might add some standardized data paths to this library in the future if demand is high.
         * Feel free to leave us a note :)
         */

        //We are prepared. Let's connect. We need to do this explicitly!
        //twitch.PubSub.Connect();
        //In this demo case the UI button will trigger this.
#endregion


#region Chat
        twitch.Chat.OnChatReceivedUnknownMessage.AddListener((string msg) => {
            Debug.LogWarning("We received something unhandled from the IRC (this is not an error): " + msg);
        });

        twitch.Chat.OnChatMessageReceived.AddListener((IRCMessage msg) => {
            string userState = "normal viewer";
            if (msg.IsSenderBroadcaster) userState = "Broadcaster";
            else if (msg.IsSenderModerator) userState = (msg.IsSenderSubscriber ? "Moderator AND Sub" : "Moderator");
            else if (msg.IsSenderVIP) userState = "VIP";
            else if (msg.IsSenderSubscriber) userState = "Subscriber";

            chatView.text = "IRC Message from " + userState + " " + msg.Sender.DisplayName + " incoming: " + msg.Text + "   Badges: " + string.Join(", ", msg.Badges.Keys);
            Color col = Color.green;
            ColorUtility.TryParseHtmlString(msg.Sender.Color, out col);
            chatView.color = col;
            Debug.Log("IRC Message from " + msg.Sender.DisplayName + " incoming: " + msg.Text + "   Badges: " + string.Join(", ", msg.Badges.Keys));
        });
        
        twitch.Chat.OnChatCommandReceived.AddListener((IRCCommand com) => {
            chatView.text = "IRC Command from " + (com.IsSenderSubscriber ? "subscriber" : "pleb") + " " + com.Sender.DisplayName + " incoming: Command=" + com.Command + ", Parameters='" + (com.Parameters != null ? string.Join("', '", com.Parameters) : "none") + "'";
            Debug.Log("IRC Command from " + (com.IsSenderSubscriber ? "subscriber" : "pleb") + " " + com.Sender.DisplayName + " incoming: Command=" + com.Command + ", Parameters='" + (com.Parameters != null ? string.Join("', '", com.Parameters) : "none") + "'");
        });

        //Commands as well as messages are also available in the whisper context. This is an example for receiving a whisper message and replying to it
        twitch.Chat.OnWhisperMessageReceived.AddListener((IRCMessage msg) => {
            chatView.text = msg.Sender.DisplayName + " is whispering: " + msg.Text;
            Color col = Color.green;
            ColorUtility.TryParseHtmlString(msg.Sender.Color, out col);
            chatView.color = col;
            Debug.Log(msg.Sender.DisplayName + " is whispering: " + msg.Text);

            //this needs the whispers:edit scope
            twitch.Chat.SendWhisperMessage(msg.Sender.Username, "W000t thanks for talking to meee");
        });


        /*
         * You can use the chat implementation to get notified for hosts and raids but this does not always work perfectly well.
         * This is caused by a bad or strange implementation on twitch's side of things.
         */

        twitch.Chat.OnIncomingHost.AddListener((IRCIncomingViewersEvent hostEvent) =>
        {
            /*
             * Twitch knows two kinds of hosting events:
             * host_success             =   Incoming host without information about viewer count. Likely this means a "blind host".
             * host_success_viewers     =   Incoming host including viewer count information
             * It is safe to access ViewerCount as in the case, that we did not receive the information from twitch, "ViewerCount" will be zero.
             * You can however check against the message "Type" to know for sure - see the following example
             * 
             * IMPORTANT NOTE: Twitch has a very crappy implementation of those notices. They are not always sent for every host.
             * This is why there is a second, very limited way to get this information (see below)
             */

            Debug.Log("We received a host from " + hostEvent.OriginatingChannelDisplayName + (hostEvent.Type.Equals("host_success_viewers") ? " counting " + hostEvent.ViewerCount + " viewers" : " without viewer information"));
        });

        twitch.Chat.OnIncomingRaid.AddListener((IRCIncomingViewersEvent hostEvent) =>
        {
            Debug.Log("We received a raid from " + hostEvent.OriginatingChannelDisplayName + " counting " + hostEvent.ViewerCount + " viewers");
        });

        //This is the alternative implementation for getting incoming host information
        twitch.Chat.OnIncomingHostInformation.AddListener((string hostOrigin) => {
            Debug.Log("Incoming host notification from " + hostOrigin);
        });

        //We need to explicitely connect to twitch.Chat. We do this in the ui methods again.
        //twitch.Chat.Connect(channelName, oAuthToken);

#endregion
    }






    //////////////////// [ This stuff is only UI for demonstration purposes ] ////////////////////

    public void Connect()
    {
        ChannelID = fldChId.text;
        if (!fldOAToken.text.Equals("Removed for security reasons. Don't change to reuse."))
        {
            oAuthToken = fldOAToken.text;
            channelName = fldChanName.text;
            PlayerPrefs.SetString("OAToken", oAuthToken);
            PlayerPrefs.SetString("ChId", ChannelID);
            PlayerPrefs.SetString("ChName", channelName);
        } 
        else
        {
            oAuthToken = PlayerPrefs.GetString("OAToken");
            Debug.Log("Re-Using stored data for connection " + channelName + "/" + ChannelID + "/" + oAuthToken);
        }

        //This is where we connect to twitch
        twitch.PubSub.Connect();
        twitch.Chat.Connect(channelName, oAuthToken);
        
    }
}
