using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyBinder : MonoBehaviour
{
    public string action; // Action name (e.g., "MoveLeft", "Jump")
    public TextMeshProUGUI keyText; // Text to display the current keybind

    private bool isRebinding = false;

    private IEnumerator Start()
    {
        while (SettingsManager.Instance == null)
        {
            yield return null;
        }

        UpdateKeyText();
    }

    public void StartRebinding()
    {
        if (isRebinding || InputBlocker.IsRebinding) return;

        isRebinding = true;
        InputBlocker.IsRebinding = true;
        //keyText.text = "Press any key...";
        StartCoroutine(WaitForKey());
    }

    private System.Collections.IEnumerator WaitForKey()
    {
        var keyTextCol = keyText.color;
        keyTextCol = new Color(keyTextCol.r, 0.8f, keyTextCol.b);
        keyText.color = keyTextCol;
        
        while (!Input.anyKeyDown)
        {
            yield return null; // Wait for user input
        }

        keyTextCol = new Color(keyTextCol.r, 0f, keyTextCol.b);
        keyText.color = keyTextCol;

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                SettingsManager.Instance.SetKeybind(action, keyCode);
                break;
            }
        }

        isRebinding = false;
        UpdateKeyText();
    }

    private void UpdateKeyText()
    {
        if (SettingsManager.Instance.Keybinds.ContainsKey(action))
        {
            keyText.text = SettingsManager.Instance.Keybinds[action].ToString();
        }
        else
        {
            keyText.text = "Unbound";
        }

        isRebinding = false;
        InputBlocker.IsRebinding = false;
    }

    public void ResetKeybind()
    {
        SettingsManager.Instance.ResetKeybind(action);
        UpdateKeyText();
    }

    public static class InputBlocker
    {
        public static bool IsRebinding { get; set; } = false;
    }
}
