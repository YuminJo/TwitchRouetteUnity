using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Security.Authentication;
using UnityEngine.Networking;

namespace Firesplash.UnityAssets.TwitchIntegration
{
    /// <summary>
    /// This static class contains methods which shall help you to work with the twitch API to get your PubSub/Chat project up quickly.
    /// These are convenience methods that do not really belong to the asset.
    /// </summary>
    public static class TwitchHelpers
    {
        /// <summary>
        /// This is a Unity Coroutine which starts a UnityWebRequest against Twitch's validate endpoint to get the channelID belonging to the specified token.
        /// When the webrequest returns successfully, the actionToTun will be invoked.
        /// If something went wrong or the token was invalid it will throw an InvalidCredentialException. The Action will not be invoked in such cases.
        /// You need to call this using StartCoroutine.
        /// </summary>
        /// <param name="oAuthToken">The token to use later on</param>
        /// <param name="actionToRun">The action that will be ran with the oauth token (1st), the channel ID (2nd) and the complete returned string of the validate endpoint as parameters (oAuthToken, channelID, completeResoponseJSON)</param>
        public static IEnumerator GetChannelIDAndRun(string oAuthToken, Action<string, string, string> actionToRun)
        {
            UnityWebRequest uwr = UnityWebRequest.Get("https://id.twitch.tv/oauth2/validate");
            uwr.SetRequestHeader("Authorization", "OAuth " + oAuthToken);
            yield return uwr.SendWebRequest();
            if (uwr.responseCode != 200)
            {
                throw new InvalidCredentialException(uwr.downloadHandler.text);
            }
            else
            {
                string buffer = uwr.downloadHandler.text;
                actionToRun.Invoke(oAuthToken, JObject.Parse(buffer).GetValue("user_id").ToString(), buffer);
            }
        }
    }
}
