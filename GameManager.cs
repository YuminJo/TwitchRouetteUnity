using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public GameObject LoginTwitch;
    public void Awake()
    {
        gm = this;
    }
    [Button("아침")]
    public void Morning()
    {
        DOTween.To(()=> LightColorController.Lc.time, x=> 
        LightColorController.Lc.time = x, 0.01f , 5).SetEase(Ease.InOutCubic);
    }

    [Button("점심")]
    public void Lunch()
    {
        DOTween.To(()=> LightColorController.Lc.time, x=> 
        LightColorController.Lc.time = x, 0.5f , 5).SetEase(Ease.InOutCubic);
    }

    [Button("저녁")]
    public void Night()
    {
        DOTween.To(()=> LightColorController.Lc.time, x=> 
        LightColorController.Lc.time = x, 1 , 5).SetEase(Ease.InOutCubic);
    }
}
