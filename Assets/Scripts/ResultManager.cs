using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public GameObject screenEffect;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("CentralManager", LoadSceneMode.Additive);
        StartCoroutine(screenEffectAlpha(1f,0f));
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

    private IEnumerator screenEffectAlpha(float alphaBefore, float alphaAfter)
    {
        if (alphaBefore != 0f){
            screenEffect.SetActive(true);
        }

        var screenCol = screenEffect.GetComponent<Renderer>().material.color;
        screenCol = new Color(screenCol.r, screenCol.g, screenCol.b, alphaBefore);

        if (alphaBefore == alphaAfter) yield break;

        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration){
            screenCol.a = Mathf.Lerp(alphaBefore, alphaAfter, elapsedTime/duration);
            screenEffect.GetComponent<Renderer>().material.color = screenCol;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        screenCol.a = alphaAfter;
        screenEffect.GetComponent<Renderer>().material.color = screenCol;

        if (alphaAfter == 0f){
            screenEffect.SetActive(false);
        }
    }
}
