using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplaySettingsUI : MonoBehaviour
{
    public Button scrollingButton;
    public Button invertedUIButton;
    public Button invertedButtonButton;
    public Button buttonModeButton;
    public Button blockSizeButton;
    public TMP_InputField customBlockSizeInput;
    public Button blockLanesButton;
    public TMP_InputField customBlockLanesInput;
    public Button ratioAButton;
    public Button ratioBButton;
    public TMP_InputField ratioAInputField;
    public TMP_InputField ratioBInputField;
    public Button clearDataButton;

    private TMP_InputField currentInputField;

    private void Start()
    {
        InitializeUI();
        clearDataButton.onClick.AddListener(ClearData);
    }

    private void InitializeUI()
    {
        UpdateScrollingButtonText();
        UpdateInvertedUIButtonText();
        UpdateInvertedButtonButtonText();
        UpdateButtonModeButtonText();
        UpdateBlockSizeButtonText();
        UpdateBlockLanesButtonText();
        UpdateRatioAButtonText();
        UpdateRatioBButtonText();
    }

    // Button text updates
    private void UpdateScrollingButtonText() =>
        scrollingButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.Scrolling.ToString();

    private void UpdateInvertedUIButtonText() =>
        invertedUIButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.InvertedUI ? "True" : "False";

    private void UpdateInvertedButtonButtonText() =>
        invertedButtonButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.InvertedButton ? "True" : "False";

    private void UpdateButtonModeButtonText() =>
        buttonModeButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.CurrentButtonMode.ToString();

    private void UpdateBlockSizeButtonText() =>
        blockSizeButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.BlockSize == SettingsManager.BlockSizeMode.Auto
            ? "Auto"
            : $"{SettingsManager.Instance.CustomBlockSize}";

    private void UpdateBlockLanesButtonText() =>
        blockLanesButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.BlockLanes == SettingsManager.BlockLaneMode.One
            ? "1"
            : $"{SettingsManager.Instance.CustomBlockLanes}";

    private void UpdateRatioAButtonText() =>
        ratioAButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.RatioA.ToString();

    private void UpdateRatioBButtonText() =>
        ratioBButton.GetComponentInChildren<TextMeshProUGUI>().text = SettingsManager.Instance.RatioB.ToString();

    // Button actions
    public void ToggleScrolling()
    {
        SettingsManager.ScrollingDirection nextDirection = SettingsManager.Instance.Scrolling == SettingsManager.ScrollingDirection.Left
            ? SettingsManager.ScrollingDirection.Right
            : SettingsManager.ScrollingDirection.Left;

        SettingsManager.Instance.SetScrollingDirection(nextDirection);
        UpdateScrollingButtonText();
    }

    public void ToggleInvertedUI()
    {
        bool nextState = !SettingsManager.Instance.InvertedUI;
        SettingsManager.Instance.SetInvertedUI(nextState);
        UpdateInvertedUIButtonText();
    }

    public void ToggleInvertedButton()
    {
        bool nextState = !SettingsManager.Instance.InvertedButton;
        SettingsManager.Instance.SetInvertedButton(nextState);
        UpdateInvertedButtonButtonText();
    }

    public void ToggleButtonMode()
    {
        int nextMode = ((int)SettingsManager.Instance.CurrentButtonMode + 1) % 3;
        SettingsManager.Instance.SetButtonMode((SettingsManager.ButtonMode)nextMode);
        UpdateButtonModeButtonText();
    }

    public void ToggleBlockSize()
    {
        if (SettingsManager.Instance.BlockSize == SettingsManager.BlockSizeMode.Auto)
        {
            EnterInputMode(customBlockSizeInput, value =>
            {
                if (int.TryParse(value, out int customSize))
                {
                    SettingsManager.Instance.SetBlockSize(SettingsManager.BlockSizeMode.Custom, customSize);
                }
                UpdateBlockSizeButtonText();
            });
        }
        else
        {
            SettingsManager.Instance.SetBlockSize(SettingsManager.BlockSizeMode.Auto);
            UpdateBlockSizeButtonText();
        }
    }

    public void ToggleBlockLanes()
    {
        if (SettingsManager.Instance.BlockLanes == SettingsManager.BlockLaneMode.One)
        {
            EnterInputMode(customBlockLanesInput, value =>
            {
                if (int.TryParse(value, out int customLanes))
                {
                    SettingsManager.Instance.SetBlockLanes(SettingsManager.BlockLaneMode.Custom, customLanes);
                }
                UpdateBlockLanesButtonText();
            });
        }
        else
        {
            SettingsManager.Instance.SetBlockLanes(SettingsManager.BlockLaneMode.One);
            UpdateBlockLanesButtonText();
        }
    }

    public void EditRatioA()
    {
        EnterInputMode(ratioAInputField, value =>
        {
            if (int.TryParse(value, out int ratioA) && ratioA >= 1 && ratioA <= 5)
            {
                SettingsManager.Instance.SetRatio(ratioA, SettingsManager.Instance.RatioB);
            }
            else
            {
                // Show warning if out of range (to be implemented)
            }
            UpdateRatioAButtonText();
        });
    }

    public void EditRatioB()
    {
        EnterInputMode(ratioBInputField, value =>
        {
            if (int.TryParse(value, out int ratioB) && ratioB >= 1 && ratioB <= 5)
            {
                SettingsManager.Instance.SetRatio(SettingsManager.Instance.RatioA, ratioB);
            }
            else
            {
                // Show warning if out of range (to be implemented)
            }
            UpdateRatioBButtonText();
        });
    }

    // Input mode handler
    private void EnterInputMode(TMP_InputField inputField, System.Action<string> onSubmit)
    {
        if (currentInputField != null)
        {
            currentInputField.DeactivateInputField();
        }

        currentInputField = inputField;
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();

        inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener(value =>
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                onSubmit(value);
            }
            inputField.gameObject.SetActive(false);
            currentInputField = null;
        });
    }

    private void ClearData()
    {
        SettingsManager.Instance.ResetToDefault();
        RefreshUI();
    }

    private void RefreshUI()
    {
        // Refresh UI elements to reflect default settings
        AudioManager.Instance.UpdateMusicSettings();
        AudioManager.Instance.UpdateSFXSettings();

        SettingsManager.Instance.ResetKeybind("Karu");
        SettingsManager.Instance.ResetKeybind("Lahu");
        SettingsManager.Instance.ResetKeybind("Pause");
        SettingsManager.Instance.ResetKeybind("End");

        UpdateScrollingButtonText();
        UpdateInvertedUIButtonText();
        UpdateInvertedButtonButtonText();
        UpdateButtonModeButtonText();
        UpdateBlockSizeButtonText();
        UpdateBlockLanesButtonText();
        UpdateRatioAButtonText();
        UpdateRatioBButtonText();
    }
}
