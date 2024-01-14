using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TwitchAlert : MonoBehaviour
{
    public string webhook_link = "";
    public void SendDiscord(string ChannelID)
    {
        StartCoroutine(SendWebhook(webhook_link, "BROADCASTING CHANNEL ID : **" + ChannelID +"**" , (sucess) =>
        {
            if(sucess)
            Debug.Log("MESSAGE SENT");
        }));
    }

    IEnumerator SendWebhook(string link, string message, System.Action<bool> action)
    {
        WWWForm form = new WWWForm();
        form.AddField("content", message);
        using (UnityWebRequest www = UnityWebRequest.Post(link, form))
        {
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                action(false);
            }
            else
            action(true);
        }
    }
}
