using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using DG.Tweening;

public class GetVersion : MonoBehaviour {

    public string Version;
    public GameObject VersionObject;
    void Start() {
        StartCoroutine(GetVersionData());
    }
 
    IEnumerator GetVersionData() {
        UnityWebRequest www = UnityWebRequest.Get("https://pastebin.com/raw/igjsp2Sj");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            string myVersion = www.downloadHandler.text;
            if(Version != myVersion)
            {
                VersionObject.gameObject.SetActive(true);
                VersionObject.transform.DOScale(new Vector3(1,1,1),0.5f).SetEase(Ease.InOutCubic);
            }
        }
    }
}