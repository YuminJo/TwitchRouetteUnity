using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
public class LogInfo : MonoBehaviour
{
    public TextMeshProUGUI Username;
    public TextMeshProUGUI Content;

    public void DestroyLog()
    {
        Destroy(this.gameObject);
    }
}
