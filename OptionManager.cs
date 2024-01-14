using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;
using HSVPicker;

public class OptionManager : MonoBehaviour
{
    public static OptionManager om;

    [Title("룰렛 옵션")]
    public bool StarSystem;
    public bool BitSystem;
    public bool GiftSystem;
    
    ///<summary>
    ///룰렛 계산
    ///</summary>
    public int RouettePercentage;
    [FoldoutGroup("룰렛 오브젝트")]
    [InfoBox("룰렛 추가 Prefab에 관한 설정입니다.")]
    public GameObject Rouette_Prefab;
    [FoldoutGroup("룰렛 오브젝트")]
    public Transform Rouette_Option_Transform;

    [Title("룰렛 오브젝트 리스트")]
    public List<GameObject> Rouette_Option_List;

    [Title("룰렛 옵션 목록")]
    public TMP_InputField Rouette_Title_Text;

    [Title("로그인")]
    public GameObject LoginPanel;
    public GameObject SettingPanel;

    [FoldoutGroup("옵션")]
    [Title("SFX볼륨")]
    public Slider SfxVolume;
    [FoldoutGroup("옵션")]
    [Title("STAR볼륨")]
    public Slider StarVolume;
    [FoldoutGroup("옵션")]
    public TMP_Dropdown Resoultion_Value;
    [FoldoutGroup("룰렛 옵션")]
    public TextMeshProUGUI Percent_Value;
    [FoldoutGroup("룰렛 옵션")]
    public GameObject Edit_Rouette_Obj;
    [FoldoutGroup("룰렛 옵션")]
    public TextMeshProUGUI RouetteSpeed_Text;
    [FoldoutGroup("룰렛 옵션")]
    public Slider RouetteSpeed_Slider;
    [FoldoutGroup("룰렛 옵션")]
    public TextMeshProUGUI StarSystem_Text;
    [FoldoutGroup("룰렛 옵션")]
    public TextMeshProUGUI AnimationSystem_Text;

    [FoldoutGroup("로그")]
    public GameObject LogObject;
    [FoldoutGroup("로그")]
    public GameObject LogPrefab;
    [FoldoutGroup("로그")]
    public Transform LogTransform;
    [FoldoutGroup("로그")]
    public bool LogBlank;
    [FoldoutGroup("메뉴")]
    public RectTransform Menu;

    [FoldoutGroup("컬러값")]
    public HexColorField hexColorField;

    [FoldoutGroup("스위치")]
    public Switch StarSwitch;
    [FoldoutGroup("스위치")]
    public Switch SkipSwitch;
    [FoldoutGroup("스위치")]
    public Switch LogSwitch;
    public void Awake()
    {
        om = this;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SettingPanel.gameObject.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.F2))
        {
            LogObject.gameObject.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnShowMenu();
        }
    }

    public bool ShowMenu;
    public void OnShowMenu()
    {
        DOTween.Kill(ShowMenu);

        ShowMenu = ShowMenu ? ShowMenu = false : ShowMenu = true;

        if(!ShowMenu) Menu.DOAnchorPosY(-370,0.5f).SetEase(Ease.InOutCubic);
        else Menu.DOAnchorPosY(0,0.5f).SetEase(Ease.InOutCubic);
        
    }
    
    public void OnCreateRouette()
    {
        if(Rouette_Option_List.Count < 20)
        {
            GameObject RouettePrefab = Instantiate(Rouette_Prefab,Rouette_Option_Transform);
            Rouette_Option_List.Add(RouettePrefab);

            RouettePrefab.GetComponent<RouetteInfo>().RouettePercentage_Text.onValueChanged.AddListener(OnEditRouetteValue);
        }
    }

    public void OnLoadRouette(string name,float percentage,int number)
    {
        #if UNITY_EDITOR
        Debug.Log("===> Create Rouette" + name + " / " + percentage);
        #endif

        if(number != 0)
        {
        GameObject RouettePrefab = Instantiate(Rouette_Prefab,Rouette_Option_Transform);
        RouetteInfo rouetteInfo = RouettePrefab.GetComponent<RouetteInfo>();
        Rouette_Option_List.Add(RouettePrefab);
        rouetteInfo.RouettePercentage_Text.onValueChanged.AddListener(OnEditRouetteValue);

        rouetteInfo.RouetteName_Text.text = name;
        rouetteInfo.RouettePercentage_Text.text = percentage.ToString();
        }
        else
        {
            RouetteInfo rouetteInfo = Rouette_Option_List[0].GetComponent<RouetteInfo>();
            rouetteInfo.RouetteName_Text.text = name;
            rouetteInfo.RouettePercentage_Text.text = percentage.ToString();
        }
    }

    public void OnDeleteRouette()
    {
        if(Rouette_Option_List.Count >= 1)
        {
            GameObject CurrentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<RouetteInfo>().gameObject;

            foreach(var RouetteList in Rouette_Option_List)
            {
                if(CurrentObj == RouetteList.gameObject)
                {
                    #if UNITY_EDITOR
                    Debug.Log(" ===> Delete Rouette : " + CurrentObj);
                    #endif

                    CurrentObj.GetComponent<RouetteInfo>().RouettePercentage_Text.onValueChanged.RemoveListener(OnEditRouetteValue);

                    Rouette_Option_List.Remove(CurrentObj);
                    Destroy(CurrentObj);

                    break;
                }
            }
        }
    }

    #region EditRouette
    public double OnCountRouetteValue()
    {
        double ValueAmount = 0;

        for(int i = 0 ; i <Rouette_Option_List.Count; i ++)
        {
            try{
            ValueAmount += double.Parse(Rouette_Option_List[i].GetComponent<RouetteInfo>().RouettePercentage_Text.text);
            }
            catch(FormatException){}
        }

        return ValueAmount;
    }

    public void OnEditRouetteValue(string AA)
    {
        double ValueAmount = OnCountRouetteValue();
        Percent_Value.text = "Current Percent : " + ValueAmount;
    }

    public void OnExitRouetteValue()
    {
        double ValueAmount = OnCountRouetteValue();

        if(Math.Round(ValueAmount * 1000)/1000 != 100)
        {
            Percent_Value.text = "<#EA3232>Current Percent : " + ValueAmount;
        }
        else
        {
            SaveManager.sm.SaveRouetteList();
            Edit_Rouette_Obj.gameObject.SetActive(false);
        }
    }

    #endregion
    
    #region RouetteSpeed
    public void OnChangeRouetteSpeed()
    {
        RouetteSpeed_Text.text = Mathf.Round(RouetteSpeed_Slider.value) + " X";
        PointRouette.pr.Speed = Mathf.Round(RouetteSpeed_Slider.value);
    }
    #endregion

    #region Log
    public void Duplicate_Log(string Username , string Content)
    {
        if(Content == "꽝" && LogBlank) return;

        GameObject LogObj = Instantiate(LogPrefab,LogTransform);
        LogInfo Loginfo = LogObj.GetComponent<LogInfo>();
        Loginfo.Username.text = Username;
        Loginfo.Content.text = Content;
    }
    #endregion

    #region Resoultion

    public void ChangeResoultion()
    {
        switch(Resoultion_Value.value)
        {
            case 0:
            Screen.SetResolution(1600,900,false);
            break;

            case 1:
            Screen.SetResolution(1440,810,false);
            break;

            case 2:
            Screen.SetResolution(1280,720,false);
            break;

            case 3:
            Screen.SetResolution(960,540,false);
            break;
        }

        ES3.Save("ResoultionValue",OptionManager.om.Resoultion_Value.value);
    }

    #endregion

    #region URL
    public void OpenTwitter()
    {
        Application.OpenURL("https://twitter.com/doyaguri");
    }

    public void OpenTwitch()
    {
        Application.OpenURL("https://www.twitch.tv/mayro0406");
    }
    #endregion
}
