using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    [SerializeField]
    private Toggle toggle;
    private bool musicToggle, sfxToggle;
    public bool isThisMusic;
    // Start is called before the first frame update
    void Start()
    {
        switch (isThisMusic)
        {
            case true:
                toggle.isOn = SettingsManager.Instance.MusicMuted;
                break;
            case false:
                toggle.isOn = SettingsManager.Instance.SFXMuted;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (isThisMusic)
        {
            case true:
                musicToggle = toggle.isOn;
                break;
            case false:
                sfxToggle = toggle.isOn;
                break;
        }
        
        if (isThisMusic) if (musicToggle == SettingsManager.Instance.MusicMuted) return;
        if (!isThisMusic) if (sfxToggle == SettingsManager.Instance.SFXMuted) return;

        switch (isThisMusic)
        {
            case true:
                if (toggle.isOn) SettingsManager.Instance.SetMusicMuted(true);
                else SettingsManager.Instance.SetMusicMuted(false);
                break;
            case false:
                if (toggle.isOn) SettingsManager.Instance.SetSFXMuted(true);
                else SettingsManager.Instance.SetSFXMuted(false);
                break;
        }
    }
}
