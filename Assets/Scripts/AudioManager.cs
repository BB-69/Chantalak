using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Slider musicSlider, sfxSlider;
    public Toggle musicToggle, sfxToggle;

    // Lists for music and sound effect AudioSources
    private List<AudioSource> musicSources = new List<AudioSource>();
    private List<AudioSource> sfxSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }
        
        // Initialize audio settings
        UpdateMusicSettings();
        UpdateSFXSettings();
    }

    // Register an AudioSource as music
    public void RegisterMusicSource(AudioSource source)
    {
        if (!musicSources.Contains(source))
        {
            musicSources.Add(source);
            ApplyMusicSettings(source); // Apply current settings
        }
    }

    // Register an AudioSource as a sound effect
    public void RegisterSFXSource(AudioSource source)
    {
        if (!sfxSources.Contains(source))
        {
            sfxSources.Add(source);
            ApplySFXSettings(source); // Apply current settings
        }
    }

    // Update music settings for all sources
    public void UpdateMusicSettings()
    {
        foreach (var source in musicSources)
        {
            ApplyMusicSettings(source);
        }
    }

    // Update SFX settings for all sources
    public void UpdateSFXSettings()
    {
        foreach (var source in sfxSources)
        {
            ApplySFXSettings(source);
        }
    }

    // Apply music settings to a specific source
    private void ApplyMusicSettings(AudioSource source)
    {
        source.volume = SettingsManager.Instance.MusicMuted ? 0f : SettingsManager.Instance.MusicVolume;
        musicSlider.value = SettingsManager.Instance.MusicVolume;
        musicToggle.isOn = SettingsManager.Instance.MusicMuted;
    }

    // Apply SFX settings to a specific source
    private void ApplySFXSettings(AudioSource source)
    {
        source.volume = SettingsManager.Instance.SFXMuted ? 0f : SettingsManager.Instance.SFXVolume;
        sfxSlider.value = SettingsManager.Instance.SFXVolume;
        sfxToggle.isOn = SettingsManager.Instance.SFXMuted;
    }
}
