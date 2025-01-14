using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("CentralManager", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMainMenu()
    {
        SettingsManager.Instance.ChangeSceneBackToMainMenu();
    }

    public void BackToGameplay()
    {
        SettingsManager.Instance.ChangeSceneBackToGameplay();
    }
}
