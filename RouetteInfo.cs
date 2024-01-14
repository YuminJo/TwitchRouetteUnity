using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class RouetteInfo : MonoBehaviour
{
    public TMP_InputField RouetteName_Text;
    //public string RouetteName;

    public TMP_InputField RouettePercentage_Text;
    //public float RouettePercentage;

    public void ClickButton()
    {
        OptionManager.om.OnDeleteRouette();
    }
}
