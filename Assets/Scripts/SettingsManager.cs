using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public enum Camera { Menu, Gameplay, Result }
    public Camera CameraCurrent { get; private set; }

    // Audio settings
    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;
    public bool MusicMuted { get; private set; } = false;
    public bool SFXMuted { get; private set; } = false;

    // Keybind settings
    public Dictionary<string, KeyCode> Keybinds { get; private set; } = new Dictionary<string, KeyCode>();

    public Dictionary<string, KeyCode> defaultKeybinds = new Dictionary<string, KeyCode>
    {
        { "Karu", KeyCode.Z },
        { "Lahu", KeyCode.X },
        { "Pause", KeyCode.Escape },
        { "End", KeyCode.Return }
    };
    
    // Gameplay settings
    public enum ScrollingDirection { Left, Right }
    public ScrollingDirection Scrolling { get; private set; }

    public bool InvertedUI { get; private set; }
    public bool InvertedButton { get; private set; }

    public enum ButtonMode { Show, Hide, Disable }
    public ButtonMode CurrentButtonMode { get; private set; }

    public enum BlockSizeMode { Auto, Custom }
    public BlockSizeMode BlockSize { get; private set; }
    public int CustomBlockSize { get; private set; }

    public enum BlockLaneMode { One, Custom }
    public BlockLaneMode BlockLanes { get; private set; }
    public int CustomBlockLanes { get; private set; }

    public int RatioA { get; private set; }
    public int RatioB { get; private set; }





    private void Awake()
    {
        CameraCurrent = Camera.Gameplay;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // Load settings from PlayerPrefs or another save system
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeSceneToGameplay()
    {
        // Set the current camera to Gameplay
        CameraCurrent = Camera.Gameplay;

        // Load the Gameplay scene and unload the Menu scene
        SceneManager.LoadScene("Gameplay"); // This will load the Gameplay scene
        SceneManager.UnloadSceneAsync("MainMenu"); // Optionally unload the MainMenu scene if it's no longer needed
    }

    public void ChangeSceneToResult()
    {
        // Set the current camera to Gameplay
        CameraCurrent = Camera.Result;

        // Load the Gameplay scene and unload the Menu scene
        SceneManager.LoadScene("Result"); // This will load the Gameplay scene
        SceneManager.UnloadSceneAsync("Gameplay"); // Optionally unload the MainMenu scene if it's no longer needed
    }

    public void ChangeSceneBackToGameplay()
    {
        // Set the current camera to Gameplay
        CameraCurrent = Camera.Gameplay;

        // Load the Gameplay scene and unload the Menu scene
        SceneManager.LoadScene("Gameplay"); // This will load the Gameplay scene
        SceneManager.UnloadSceneAsync("Result"); // Optionally unload the MainMenu scene if it's no longer needed
    }

    public void ChangeSceneBackToMainMenu()
    {
        // Set the current camera to Gameplay
        CameraCurrent = Camera.Menu;

        // Load the Gameplay scene and unload the Menu scene
        SceneManager.LoadScene("MainMenu"); // This will load the Gameplay scene
        SceneManager.UnloadSceneAsync("Result"); // Optionally unload the MainMenu scene if it's no longer needed
    }





    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp(volume, 0f, 1f);
        SaveSettings();
        AudioManager.Instance.UpdateMusicSettings(); // Notify AudioManager
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp(volume, 0f, 1f);
        SaveSettings();
        AudioManager.Instance.UpdateSFXSettings(); // Notify AudioManager
    }

    public void SetMusicMuted(bool muted)
    {
        MusicMuted = muted;
        SaveSettings();
        AudioManager.Instance.UpdateMusicSettings(); // Notify AudioManager
    }

    public void SetSFXMuted(bool muted)
    {
        SFXMuted = muted;
        SaveSettings();
        AudioManager.Instance.UpdateSFXSettings(); // Notify AudioManager
    }





    public void SetKeybind(string action, KeyCode key)
    {
        if (Keybinds.ContainsKey(action))
        {
            Keybinds[action] = key;
            SaveKeybinds();
        }
    }

    public void ResetKeybind(string action)
    {
        if (defaultKeybinds.ContainsKey(action))
        {
            Keybinds[action] = defaultKeybinds[action];
            SaveKeybinds();
        }
    }

    private void SaveKeybinds()
    {
        foreach (var keybind in Keybinds)
        {
            PlayerPrefs.SetString($"Keybind_{keybind.Key}", keybind.Value.ToString());
        }
        PlayerPrefs.Save();
    }





    public void SetScrollingDirection(ScrollingDirection direction)
    {
        Scrolling = direction;
        SaveGameplaySettings();
    }

    public void SetInvertedUI(bool inverted)
    {
        InvertedUI = inverted;
        SaveGameplaySettings();
    }

    public void SetInvertedButton(bool inverted)
    {
        InvertedButton = inverted;
        SaveGameplaySettings();
    }

    public void SetButtonMode(ButtonMode mode)
    {
        CurrentButtonMode = mode;
        SaveGameplaySettings();
    }

    public void SetBlockSize(BlockSizeMode mode, int customSize = 1)
    {
        BlockSize = mode;
        if (mode == BlockSizeMode.Custom)
        {
            CustomBlockSize = Mathf.Clamp(customSize, 1, 5);
        }
        SaveGameplaySettings();
    }

    public void SetBlockLanes(BlockLaneMode mode, int customLanes = 1)
    {
        BlockLanes = mode;
        if (mode == BlockLaneMode.Custom)
        {
            CustomBlockLanes = Mathf.Clamp(customLanes, 1, 5);
        }
        SaveGameplaySettings();
    }

    public void SetRatio(int ratioA, int ratioB)
    {
        RatioA = Mathf.Clamp(ratioA, 1, 5);
        RatioB = Mathf.Clamp(ratioB, 1, 5);
        SaveGameplaySettings();
    }

    private void SaveGameplaySettings()
    {
        PlayerPrefs.SetInt("Scrolling", (int)Scrolling);
        PlayerPrefs.SetInt("InvertedUI", InvertedUI ? 1 : 0);
        PlayerPrefs.SetInt("InvertedButton", InvertedButton ? 1 : 0);
        PlayerPrefs.SetInt("ButtonMode", (int)CurrentButtonMode);
        PlayerPrefs.SetInt("BlockSize", (int)BlockSize);
        PlayerPrefs.SetInt("CustomBlockSize", CustomBlockSize);
        PlayerPrefs.SetInt("BlockLanes", (int)BlockLanes);
        PlayerPrefs.SetInt("CustomBlockLanes", CustomBlockLanes);
        PlayerPrefs.SetInt("RatioA", RatioA);
        PlayerPrefs.SetInt("RatioB", RatioB);

        PlayerPrefs.Save();
    }





    private void SaveSettings()
    {
        // Use PlayerPrefs or another save system
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.SetInt("MusicMuted", MusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SFXMuted", SFXMuted ? 1 : 0);
        
        foreach (var keybind in Keybinds)
        {
            PlayerPrefs.SetString($"Keybind_{keybind.Key}", keybind.Value.ToString());
        }
        
        PlayerPrefs.SetInt("Scrolling", (int)Scrolling);
        PlayerPrefs.SetInt("InvertedUI", InvertedUI ? 1 : 0);
        PlayerPrefs.SetInt("InvertedButton", InvertedButton ? 1 : 0);
        PlayerPrefs.SetInt("ButtonMode", (int)CurrentButtonMode);
        PlayerPrefs.SetInt("BlockSize", (int)BlockSize);
        PlayerPrefs.SetInt("CustomBlockSize", CustomBlockSize);
        PlayerPrefs.SetInt("BlockLanes", (int)BlockLanes);
        PlayerPrefs.SetInt("CustomBlockLanes", CustomBlockLanes);
        PlayerPrefs.SetInt("RatioA", RatioA);
        PlayerPrefs.SetInt("RatioB", RatioB);
        
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // Load settings
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        MusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        SFXMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

        Scrolling = (ScrollingDirection)PlayerPrefs.GetInt("Scrolling", (int)ScrollingDirection.Left);
        InvertedUI = PlayerPrefs.GetInt("InvertedUI", 0) == 1;
        InvertedButton = PlayerPrefs.GetInt("InvertedButton", 0) == 1;
        CurrentButtonMode = (ButtonMode)PlayerPrefs.GetInt("ButtonMode", (int)ButtonMode.Show);
        BlockSize = (BlockSizeMode)PlayerPrefs.GetInt("BlockSize", (int)BlockSizeMode.Auto);
        CustomBlockSize = PlayerPrefs.GetInt("CustomBlockSize", 1);
        BlockLanes = (BlockLaneMode)PlayerPrefs.GetInt("BlockLanes", (int)BlockLaneMode.One);
        CustomBlockLanes = PlayerPrefs.GetInt("CustomBlockLanes", 1);
        RatioA = PlayerPrefs.GetInt("RatioA", 1);
        RatioB = PlayerPrefs.GetInt("RatioB", 1);

        // Load keybinds
        Keybinds.Clear();
        foreach (var keybind in defaultKeybinds)
        {
            string savedKey = PlayerPrefs.GetString($"Keybind_{keybind.Key}", keybind.Value.ToString());
            if (System.Enum.TryParse(savedKey, out KeyCode keyCode))
            {
                Keybinds[keybind.Key] = keyCode;
            }
        }
    }

    // Add a method to reset all settings to default values
    public void ResetToDefault()
    {
        // Audio
        MusicVolume = 1.0f;
        SFXVolume = 1.0f;
        MusicMuted = false;
        SFXMuted = false;

        // Keybinds
        Keybinds = new Dictionary<string, KeyCode>(defaultKeybinds);

        // Gameplay
        Scrolling = ScrollingDirection.Left;
        InvertedUI = false;
        InvertedButton = false;
        CurrentButtonMode = ButtonMode.Show;
        BlockSize = BlockSizeMode.Auto;
        CustomBlockSize = 1; // Set to some default value
        BlockLanes = BlockLaneMode.One;
        CustomBlockLanes = 1; // Set to some default value
        RatioA = 1; // Default ratio
        RatioB = 1;

        // Save the default settings
        SaveSettings();
    }
}