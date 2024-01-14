using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class CSVWriter : MonoBehaviour
{
    public TextMeshProUGUI LogState;
    public string fileName = "RouetteLog.csv";
    
    List<string[]> data = new List<string[]>();
    string[] tempData;
    
    void DataInit()
    {
    	data.Clear();
        
        tempData = new string[4];
        tempData[0] = "Number";
        tempData[1] = "Name";
        tempData[2] = "Content";
        data.Add(tempData);
	}
    
    [Button("CSV")]
    public void SaveCSVFile()
    {
        LogState.text = "내보내는 중";
        DataInit();

    	tempData = new string[3];

        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        LogState.text = "로그 데이터 수집 : 진행";
        for(int i = 0 ; i < OptionManager.om.LogTransform.childCount; i++)
        {
            LogInfo childLog = OptionManager.om.LogTransform.GetChild(i).gameObject.GetComponent<LogInfo>();
            tempData[0] = i.ToString();
            tempData[1] = childLog.Username.text;
            tempData[2] = childLog.Content.text;
            data.Add(tempData);
            Debug.Log(data[data.Count-1][0] + data[data.Count-1][1] + data[data.Count-1][2]);
            sb.AppendLine(string.Join(delimiter, data[data.Count-1][0] , data[data.Count-1][1] , data[data.Count-1][2]));
        }

        LogState.text = "로그 데이터 수집 : 완료";

        string filepath = SystemPath.GetPath();

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        StreamWriter outStream = System.IO.File.CreateText(filepath + fileName);
        outStream.Write(sb);

        LogState.text = "내보내기 성공 " + filepath + fileName;
        outStream.Close();
    }
}

public static class SystemPath
{
    public static string GetPath(string fileName)
    {
        string path = GetPath();
        return Path.Combine(GetPath(), fileName);
    }

    public static string GetPath()
    {
        string path = null;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(Application.persistentDataPath, "Resources/");
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                path = Application.persistentDataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Assets", "Resources/");       
            case RuntimePlatform.WindowsEditor:
                path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Assets", "Resources/");
            default:
                path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf('/'));
                return Path.Combine(path, "Resources/");
        }
    }
}