using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    private float musicVolume, sfxVolume;
    public bool isThisMusic;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (SettingsManager.Instance == null)
        {
            yield return null;
        }

        switch (isThisMusic)
        {
            case true:
                slider.value = SettingsManager.Instance.MusicVolume;
                break;
            case false:
                slider.value = SettingsManager.Instance.SFXVolume;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (SettingsManager.Instance == null) return;

        switch (isThisMusic)
        {
            case true:
                musicVolume = slider.value;
                break;
            case false:
                sfxVolume = slider.value;
                break;
        }

        if (isThisMusic) if (musicVolume == SettingsManager.Instance.MusicVolume) return;
        if (!isThisMusic) if (sfxVolume == SettingsManager.Instance.SFXVolume) return;

        switch (isThisMusic)
        {
            case true:
                SettingsManager.Instance.SetMusicVolume(musicVolume);
                break;
            case false:
                SettingsManager.Instance.SetSFXVolume(sfxVolume);
                break;
        }
    }
}
