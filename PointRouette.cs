using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class PointRouette : MonoBehaviour
{
    [Title("룰렛 기본정보")]
    // [LabelText("룰렛 제목")]
    // public string Rouette_Title;
    [LabelText("천장 값")]
    public int PointValue;
    [Title("룰렛 정보")]
    public static PointRouette pr;
    [LabelText("룰렛")]
    public int RouetteCount;
    public TextMeshProUGUI RouetteText;
    public TextMeshProUGUI RouettePlayer;
    public RectTransform RouetteImage;
    public List<RouetteList> rouetteLists;
    public List<string> PlayerList;
    /// <summary>
    /// 룰렛 기록저장
    /// </summary>
    public List<RouetteArchive> rouetteArchive;
    public RouetteArchive saveArchive;
    public string ResultRouette;
    public int RouetteStack;
    private bool RouetteIsRunning;

    [Title("로그인 체크")]
    public GameObject LoginPanel;

    [Title("별 이미지")]
    [InfoBox("이 오브젝트는 별을 출력할때 사용합니다.")]
    public GameObject[] StarImg;
    public int StarCount;

    [Title ("천장 시스템")]
    public Slider PointSlider;
    public RectTransform PointRect;

    [Title("EULA")]
    [InfoBox("EULA 약관입니다.")]
    public GameObject EULAOBJ;

    [FoldoutGroup("옵션")]
    [InfoBox("Animation Speed")]
    public float Speed;

    [FoldoutGroup("옵션")]
    [InfoBox("Animation Skip")]
    public bool Skip;

    void Awake()
    {
        pr = this;

        PlayerList = new List<string>();

        LoginPanel.gameObject.transform.localScale= (new Vector3(0.7f,0.7f,1));
    }

    void Start()
    {
        //Animation LoginPanel To Start Program
        LoginPanel.transform.DOScale(new Vector3(1,1,1),0.7f).SetEase(Ease.InOutCubic);
    }
    
    [Serializable]
    public struct RouetteList
    {
        public string RouetteName;
        public float RouettePercentage;

        public RouetteList(string RouetteName, float RouettePercentage)
        {
            this.RouetteName = RouetteName;
            this.RouettePercentage = RouettePercentage;
        }
    }

    [Serializable]
    public struct RouetteArchive
    {
        public string playername;
         public string rouettecontent;

        public RouetteArchive(string playername, string rouettecontent)
        {
            this.playername = playername;
            this.rouettecontent = rouettecontent;
        }
    }
    
    [Button("랜덤 룰렛")]
    public void RouetteCheck_Stack()
    {
        if(!RouetteIsRunning && RouetteStack >= 1)
        {
        RouetteStack--;
        RouettePlayer.text = PlayerList[0].ToString();
        PlayerList.RemoveAt(0);
        Rouette(RouettePlayer.text);
        }

        //TODO : 천장시스템 입니다.
        // if(!CeilingSystemCheck)
        // {
        //     Debug.Log("체크체크");
        //     StartCoroutine(CeilingSystem());
        // }
    }
    public void Rouette(string Username)
    {
        RouetteIsRunning = true;

        //Sort List
        SortList();
        
        //Initalize Rouette Value
        float RandomPercent = UnityEngine.Random.Range(0f,100f);
        float RemainPercentage = 0;

        for(int i = 0 ; i < rouetteLists.Count; i++)
        {
            RemainPercentage += rouetteLists[i].RouettePercentage;

            if(RandomPercent <= RemainPercentage)
            {
                saveArchive.playername = Username;
                saveArchive.rouettecontent = rouetteLists[i].RouetteName;
                ResultRouette = rouetteLists[i].RouetteName;
                break;
            }
            else
            {
                StarCount++;
            }
        }

        rouetteArchive.Add(saveArchive);
        OptionManager.om.Duplicate_Log(Username,saveArchive.rouettecontent);
        
        if(RouetteImage.rect.height < 750f)
        {
        //이거 속도 수정해야함
        RouetteImage.DOSizeDelta(new Vector2(RouetteImage.rect.width,750f),(1f/Speed)).SetEase(Ease.InOutCubic).OnComplete(()=>
        {
        StartCoroutine(RouetteAni(ResultRouette));
        });
        }
        else
        {
        StartCoroutine(RouetteAni(ResultRouette));
        }
    }
    
    private void SortList()
    {
        rouetteLists.Sort(delegate (RouetteList A, RouetteList B)
        {
            if (A.RouettePercentage < B.RouettePercentage ) return 1;
            else if (A.RouettePercentage > B.RouettePercentage ) return -1;
            return 0;
        });
    }


    IEnumerator RouetteAni(string Result)
    {
        if(!Skip)
        {
        float Time = 0.01f;
        int Count = 0;
        //SFX
        AudioManager.am.SetClip(0);
        AudioManager.am.SetStar(0);
        while(Count <= 70)
        {
            //SFX
            AudioManager.am.Sfxaudio.Play();

            RouetteText.text = rouetteLists[UnityEngine.Random.Range(0,rouetteLists.Count)].RouetteName;
            yield return new WaitForSeconds(Time);
            
            Count++;
            Time += (Time * 0.05f) / Speed;

            if(OptionManager.om.StarSystem)
            {
            switch(Count)
            {
                case 20:
                if(StarCount > 0)
                {
                    StarCount--;
                    StarImg[0].transform.DOScale(new Vector3(0.7f,0.7f,1),0.4f).SetEase(Ease.InOutCubic);
                    AudioManager.am.StarAudio.Play();
                }
                break;

                case 50:
                if(StarCount > 0)
                {
                    StarCount--;
                    StarImg[1].transform.DOScale(new Vector3(0.5f,0.5f,1),0.4f).SetEase(Ease.InOutCubic);
                    AudioManager.am.StarAudio.Play();
                }
                break;

                case 60:
                if(StarCount > 0)
                {
                    StarCount--;
                    StarImg[2].transform.DOScale(new Vector3(0.5f,0.5f,1),0.4f).SetEase(Ease.InOutCubic);
                    AudioManager.am.StarAudio.Play();
                }
                break;
            }
            }
        }
        yield return new WaitForSeconds(1f/Speed);
        RouetteText.text = Result;
        //SFX
        // AudioManager.am.SetClip(1);
        // AudioManager.am.Sfxaudio.Play();

        AudioManager.am.SetStar(1);
        AudioManager.am.StarAudio.Play();
        yield return new WaitForSeconds(3f/Speed);}

        else{
        RouetteText.text = Result;
        yield return new WaitForSeconds(0.1f);
        }
        
        if(RouetteStack <= 0){
        // * END IF SKIP
        RouetteImage.DOSizeDelta(new Vector2(RouetteImage.rect.width,0f),1f/Speed).SetEase(Ease.InOutCubic).OnComplete(()
        =>
        {
        //Star Deactive
        foreach(var Stars in StarImg)
        {
        Stars.transform.localScale = new Vector3(0,0,1);
        }
        });}
        else
        {
        foreach(var Stars in StarImg)
        {
        Stars.transform.localScale = new Vector3(0,0,1);
        }
        }

        RouetteIsRunning = false;
        StarCount = 0;
        RouetteCheck_Stack();
        yield return 0;
    }

    bool OnShowConnect;
    public void ShowConnecting()
    {
        LoginPanel.gameObject.SetActive(false);

        OptionManager.om.ShowMenu = false;
        OptionManager.om.OnShowMenu();
    }

    private bool CeilingSystemCheck;
    public IEnumerator CeilingSystem()
    {
        CeilingSystemCheck = true;

        PointRect.DOAnchorPosY(200,1f).SetEase(Ease.InOutCubic).OnComplete(()
        =>
        {
        //슬라이더 업데이트
        PointValue += 1;
        SaveManager.sm.SaveValue("Point",PointValue);
        PointSlider.DOValue(PointValue,1f).SetEase(Ease.InOutCubic);
        });

        yield return new WaitForSeconds(4f);
        PointRect.DOAnchorPosY(-200,1f).SetEase(Ease.InOutCubic);
        
        CeilingSystemCheck = false;
        yield return 0;
    }

    #region URL
    public void OpenEULA()
    {
        Application.OpenURL("https://pear-limpet-cb5.notion.site/EULA-a5b613ff85904eaeb382ddf3ed73fedd");
    }
    #endregion
}