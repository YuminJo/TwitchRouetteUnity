using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager sm;

    public void Start()
    {
        sm = this;

        OnLoadPoint();
        OnLoadContent();
        OnLoadSetting();
    }

    private void OnLoadPoint()
    {
        if(ES3.KeyExists("Point") == false)
        {
            ES3.Save("Point",0);
        }
        else
        {
            //포인트를 불러옴
            PointRouette.pr.PointValue = ES3.Load<int>("Point");
            PointRouette.pr.PointSlider.value = PointRouette.pr.PointValue;
        }
    }
    
    private void OnLoadContent()
    {
        if(ES3.KeyExists("RouetteList1") == false)
        {
            ES3.Save<List<PointRouette.RouetteList>>("RouetteList1", PointRouette.pr.rouetteLists);

            //EULAOPEN
            //PointRouette.pr.EULAOBJ.gameObject.SetActive(true);

            //OPENROUETTE
            OptionManager.om.Edit_Rouette_Obj.gameObject.SetActive(true);
            
            #if UNITY_EDITOR
            Debug.Log(" ===> Save : Initalize RouetteLists ");
            #endif
        }
        else
        {
            OptionManager.om.LoginPanel.gameObject.SetActive(true);

            try{OptionManager.om.Rouette_Title_Text.text = ES3.Load<string>("RouetteTitle");}
            catch(KeyNotFoundException){}

            try{OptionManager.om.StarSystem = ES3.Load<bool>("RouetteStar");}
            catch(KeyNotFoundException){}

            try{PointRouette.pr.Skip = ES3.Load<bool>("RouetteSkip");}
            catch(KeyNotFoundException){}

            try{OptionManager.om.LogBlank = ES3.Load<bool>("LogBlank");}
            catch(KeyNotFoundException){}

            //룰렛 내용 불러옴
            try{PointRouette.pr.rouetteLists = ES3.Load<List<PointRouette.RouetteList>>("RouetteList1");}
            catch(KeyNotFoundException){}

            try{PointRouette.pr.Speed = ES3.Load<float>("RouetteSpeed");
            OptionManager.om.RouetteSpeed_Slider.value = PointRouette.pr.Speed;
            OptionManager.om.OnChangeRouetteSpeed();}
            catch(KeyNotFoundException){}

            for ( int i = 0; i < PointRouette.pr.rouetteLists.Count; i++)
            {
                OptionManager.om.OnLoadRouette(PointRouette.pr.rouetteLists[i].RouetteName,PointRouette.pr.rouetteLists[i].RouettePercentage,i);
            }
            
            OptionManager.om.StarSwitch.isOn = OptionManager.om.StarSystem;
            OptionManager.om.SkipSwitch.isOn = PointRouette.pr.Skip;
            OptionManager.om.LogSwitch.isOn = OptionManager.om.LogBlank;
            OptionManager.om.OnEditRouetteValue("");

            #if UNITY_EDITOR
            Debug.Log(" ===> OnLoad : RouetteList ");
            #endif
        }
            OptionManager.om.StarSwitch.SwitchAniatmion();
            OptionManager.om.SkipSwitch.SwitchAniatmion();
            OptionManager.om.LogSwitch.SwitchAniatmion();
    }

    public void OnLoadSetting()
    {
        try
        {
        OptionManager.om.SfxVolume.value = ES3.Load<float>("SfxVolume");
        AudioManager.am.Sfxaudio.volume = OptionManager.om.SfxVolume.value;
        }catch(KeyNotFoundException){}

        try
        {
        OptionManager.om.StarVolume.value = ES3.Load<float>("StarVolume");
        AudioManager.am.StarAudio.volume = OptionManager.om.StarVolume.value;
        }catch(KeyNotFoundException){}

        try
        {
        OptionManager.om.Resoultion_Value.value = ES3.Load<int>("ResoultionValue");
        OptionManager.om.ChangeResoultion();
        }catch(KeyNotFoundException){}

        try
        {
        OptionManager.om.hexColorField.UpdateColor(ES3.Load<string>("ColorPicker"));
        }catch(KeyNotFoundException){}
    }

    public void SaveValue(string ValueName, int ValueInt)
    {
        ES3.Save(ValueName,ValueInt);
    }

    public void SaveRouette()
    {
        try
        {
        ES3.Save("RouetteTitle",OptionManager.om.Rouette_Title_Text.text.ToString());
        ES3.Save("RouetteStar",OptionManager.om.StarSystem);
        ES3.Save("RouetteSkip",PointRouette.pr.Skip);
        ES3.Save("RouetteSpeed",PointRouette.pr.Speed);
        ES3.Save("LogBlank",OptionManager.om.LogBlank);
        
        OptionManager.om.SettingPanel.SetActive(false);
        }
        catch(FormatException){}
    }

    public void SaveRouetteList()
    {
        //Initalize Rouette Save
        PointRouette.pr.rouetteLists.Clear();

        try
        {
        foreach(var OptionList in OptionManager.om.Rouette_Option_List)
        {
            RouetteInfo RI = OptionList.GetComponent<RouetteInfo>();

            PointRouette.RouetteList RouetteInfo = new PointRouette.RouetteList(RI.RouetteName_Text.text,float.Parse(RI.RouettePercentage_Text.text));
            PointRouette.pr.rouetteLists.Add(RouetteInfo);
        }
        ES3.Save("RouetteList1", PointRouette.pr.rouetteLists);}
        catch(FormatException){}
    }

    public void SaveOption()
    {
        ES3.Save("SfxVolume",OptionManager.om.SfxVolume.value);
        ES3.Save("StarVolume",OptionManager.om.StarVolume.value);
    }

    public void SaveColorPicker()
    {
        ES3.Save("ColorPicker",OptionManager.om.hexColorField.hexInputField.text);
        Debug.Log(OptionManager.om.hexColorField.hexInputField.text);
    }
}
