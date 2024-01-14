using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public class AudioManager : MonoBehaviour
{
    public static AudioManager am;
    public AudioSource Sfxaudio;
    public AudioClip[] SfxList;

    [Title("Star Audio")]
    public AudioSource StarAudio;
    public AudioClip[] StarList;

    public void Awake()
    {
        am = this;
    }

    public void SetClip(int number)
    {
        Sfxaudio.clip = SfxList[number];
    }

    public void SetStar(int number)
    {
        StarAudio.clip = StarList[number];
    }

    public void ChangeSfx()
    {
        Sfxaudio.volume = OptionManager.om.SfxVolume.value;
    }

    public void ChangeStar()
    {
        StarAudio.volume = OptionManager.om.StarVolume.value;
    }
}
