using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayToResult : MonoBehaviour
{
    public Button yourButton; // Reference to your UI button

    private void Start()
    {
        // Set up the button listener
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(OnButtonPress);
        }
    }

    private void OnButtonPress()
    {
        // Call the singleton method to change the scene
        SettingsManager.Instance.ChangeSceneToResult();
    }
}
