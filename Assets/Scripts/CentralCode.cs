using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CentralCode : MonoBehaviour
{
    public OptionPage optionPage;
    public PlayPage playPage;


    public GameObject screenBlock;
    public GameObject sideBarPlayLeft, sideBarOptionsLeft, sideBarInfoLeft, sideBarExitLeft;
    public GameObject sideBarPlayRight, sideBarOptionsRight, sideBarInfoRight, sideBarExitRight;
    private GameObject sideBarLeft, sideBarRight;
    public GameObject sideBarObject;
    public GameObject bgPlay, bgOptions, bgInfo, bgExit;
    private RectTransform bgCover;
    private GameObject bgElement;



    public bool isAnimating;
    private bool buttonIsActivating;
    public bool buttonIsNormal;
    public bool disableButtons;
    public GameObject button0, button1, button2, button3, button4;
    public TextMeshProUGUI text0, text1, text2, text3, text4;
    private string selectedButton;

    private bool loop = false;

    private Vector2 sideBarSize;
    public bool getSideBarSizeDone;

    private Vector2 firstPosBar, secondPosBar;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("CentralManager", LoadSceneMode.Additive);

        getSideBarSizeDone = false;
        sideBarSize = sideBarObject.GetComponent<Transform> ().localScale;
        getSideBarSizeDone = true;

        firstPosBar = sideBarOptionsLeft.GetComponent<Transform> ().localPosition;
        secondPosBar = firstPosBar;
        secondPosBar.x = secondPosBar.x + sideBarSize.x;

        touchBlock(false);
        isAnimating = false;
        buttonIsActivating = false;
        buttonIsNormal = true;
        disableButtons = false;

        button0.SetActive(true);
        button1.SetActive(true);
        button2.SetActive(true);
        button3.SetActive(false);   // Cancelled Feature
        button4.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] allButtons = {button0, button1, button2, button3, button4};
        string[] allButtonSelection = {"button0", "button1", "button2", "button3", "button4"};


        /*if (isAnimating)
        {
            for (int i = 0; i < allButtons.Length; i++)
            {
                Button buttonElement = allButtons[i].GetComponent<Button> ();
                buttonElement.interactable = false;
            }
        }
        else
        {
            for (int i = 0; i < allButtons.Length; i++)
            {
                Button buttonElement = allButtons[i].GetComponent<Button> ();
                buttonElement.interactable = true;
            }
        }*/





        if (disableButtons)
        {
            if (selectedButton == "button1")
            {
                button0.SetActive(false);
                button2.SetActive(false);
                button3.SetActive(false);
                button4.SetActive(false);
            }
            else if (selectedButton == "button2")
            {
                button0.SetActive(false);
                button1.SetActive(false);
                button3.SetActive(false);
                button4.SetActive(false);
            }
            else if (selectedButton == "button3")
            {
                button0.SetActive(false);
                button1.SetActive(false);
                button2.SetActive(false);
                button4.SetActive(false);
            }
            else if (selectedButton == "button4")
            {
                button0.SetActive(false);
                button1.SetActive(false);
                button2.SetActive(false);
                button3.SetActive(false);
            }
        }
        else
        {
            button0.SetActive(true);
            button1.SetActive(true);
            button2.SetActive(true);
            button3.SetActive(false);
            button4.SetActive(true);
        }


        if (buttonIsNormal)
        {
            if (loop)
            {
                if (selectedButton == "button1"){
                    StartCoroutine(Fade(text0, text2, text3, text4, 0f, 1f));
                }
                else if (selectedButton == "button2"){
                    StartCoroutine(Fade(text0, text1, text3, text4, 0f, 1f));
                }
                else if (selectedButton == "button3"){
                    StartCoroutine(Fade(text0, text1, text2, text4, 0f, 1f));
                }
                else if (selectedButton == "button4"){
                    StartCoroutine(Fade(text0, text1, text2, text3, 0f, 1f));
                }

                for (int i = 0; i < allButtons.Length; i++)
                {
                    Button buttonElement = allButtons[i].GetComponent<Button> ();
                    buttonElement.interactable = true;
                }
                loop = false;
            }

            buttonIsActivating = false;
        }
        else
        {
            loop = true;
        }
    }

    public void buttonClicked(string button)
    {
        if (!buttonIsActivating)
        {
            buttonIsActivating = true;
            buttonIsNormal = false;

            selectedButton = button;
            if (button == "button1")
            {
                StartCoroutine(Fade(text0, text2, text3, text4, 1f, 0f));

                /*button0.SetActive(false);
                button2.SetActive(false);
                button3.SetActive(false);
                button4.SetActive(false);*/
            }
            else if (button == "button2")
            {
                StartCoroutine(Fade(text0, text1, text3, text4, 1f, 0f));

                /*button0.SetActive(false);
                button1.SetActive(false);
                button3.SetActive(false);
                button4.SetActive(false);*/
            }
            else if (button == "button3")
            {
                StartCoroutine(Fade(text0, text1, text2, text4, 1f, 0f));

                /*button0.SetActive(false);
                button1.SetActive(false);
                button2.SetActive(false);
                button4.SetActive(false);*/
            }
            else if (button == "button4")
            {
                StartCoroutine(Fade(text0, text1, text2, text3, 1f, 0f));

                /*button0.SetActive(false);
                button1.SetActive(false);
                button2.SetActive(false);
                button3.SetActive(false);*/
            }
        }
    }

    private IEnumerator Fade(TextMeshProUGUI thing1, TextMeshProUGUI thing2, TextMeshProUGUI thing3, TextMeshProUGUI thing4, float colBefore, float colAfter)
    {
        TextMeshProUGUI[] allThings = {thing1, thing2, thing3, thing4};
        float duration = 0.01f;

        foreach (var textElement in allThings)
        {
            if (textElement == null) continue; // Skip if textElement is null
            Color originalColor = textElement.color;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(colBefore, colAfter, elapsedTime / duration);

                // Update the color's alpha channel
                textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                yield return null;
            }

            // Ensure the final alpha value is set
            textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, colAfter);
        }
    }

    public void touchBlock(bool condition)
    {
        if (condition){
            screenBlock.SetActive(true);
        }
        else {
            screenBlock.SetActive(false);
        }
    }



















// When each menu options are clicked

    public void playClicked(bool isReverse)
    {
        if (!isReverse) StartCoroutine(menuOpen("play"));
        else StartCoroutine(menuClose("play"));
    }

    public void optionsClicked(bool isReverse)
    {
        if (!isReverse) StartCoroutine(menuOpen("options"));
        else StartCoroutine(menuClose("options"));
    }

    public void infoClicked(bool isReverse)
    {
        if (!isReverse) StartCoroutine(menuOpen("info"));
        else StartCoroutine(menuClose("info"));
    }

    public void exitClicked()
    {
        StartCoroutine(menuOpen("exit"));

        Application.Quit();
        
    }






    private IEnumerator menuOpen(string menuClicked)
    {
        yield return fadeMenuBG(menuClicked, 0f, 1f);
        yield return moveSideBar("in", menuClicked);



        if (menuClicked == "play")
        {
            StartCoroutine(playPage.playOpen());
        }

        else if (menuClicked == "options")
        {
            
            StartCoroutine(optionPage.optionOpen());
        }

        else if (menuClicked == "info")
        {

        }

        else if (menuClicked == "exit")
        {

        }

        

    }

    private IEnumerator menuClose(string menuClicked)
    {
        if (menuClicked == "play")
        {
            StopCoroutine(playPage.playOpen());
            StartCoroutine(playPage.playClose());
        }

        else if (menuClicked == "options")
        {
            
            StopCoroutine(optionPage.optionOpen());
            StartCoroutine(optionPage.optionClose());
        }

        else if (menuClicked == "info")
        {

        }

        else if (menuClicked == "exit")
        {

        }
        


        yield return moveSideBar("out", menuClicked);
        yield return fadeMenuBG(menuClicked, 1f, 0f);
    }






    private IEnumerator moveSideBar(string movement, string menuClicked)
    {

        if (menuClicked == "play"){
            sideBarLeft = sideBarPlayLeft;
            sideBarRight = sideBarPlayRight;
        }
        else if (menuClicked == "options"){
            sideBarLeft = sideBarOptionsLeft;
            sideBarRight = sideBarOptionsRight;
        }
        else if (menuClicked == "info"){
            sideBarLeft = sideBarInfoLeft;
            sideBarRight = sideBarInfoRight;
        }
        else if (menuClicked == "exit"){
            sideBarLeft = sideBarExitLeft;
            sideBarRight = sideBarExitRight;
        }

        
        

        float duration = 2f;
        float elapsedTime = 0f;





        
        if (movement == "in")
        {
            sideBarLeft.SetActive(true);
            sideBarRight.SetActive(true);
            /*float childCount = sideBarLeft.transform.hierarchyCount;
            for (int i = 0; i < childCount; i++)
            {
                sideBarLeft.transform.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < childCount; i++)
            {
                sideBarRight.transform.GetChild(i).gameObject.SetActive(true);
            }*/

            while (elapsedTime < duration)
            {
                sideBarLeft.GetComponent<Transform> ().localPosition = Vector2.Lerp(firstPosBar, secondPosBar, elapsedTime / duration);
                sideBarRight.GetComponent<Transform> ().localPosition = Vector2.Lerp(-firstPosBar, -secondPosBar, elapsedTime / duration);

                elapsedTime += Time.deltaTime;                
            }
        }
        else if (movement == "out")
        {
            while (elapsedTime < duration)
            {
                sideBarLeft.GetComponent<Transform> ().localPosition = Vector2.Lerp(secondPosBar, firstPosBar, elapsedTime / duration);
                sideBarRight.GetComponent<Transform> ().localPosition = Vector2.Lerp(-secondPosBar, -firstPosBar, elapsedTime / duration);

                elapsedTime += Time.deltaTime;
            }

            sideBarLeft.SetActive(false);
            sideBarRight.SetActive(false);
        }


        yield return null;

        
        
    }

    private IEnumerator fadeMenuBG(string menuClicked, float colAfter, float colBefore)
    {

        if (menuClicked == "play"){bgElement = bgPlay;}
        else if (menuClicked == "options"){bgElement = bgOptions;}
        else if (menuClicked == "info"){bgElement = bgInfo;}
        else if (menuClicked == "exit"){bgElement = bgExit;}

        if (colBefore == 1f){
            bgElement.SetActive(true);
        }
        float duration = 0.6f;

        
        var originalColor = bgCover.GetComponent<Renderer> ().material.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(colBefore, colAfter, elapsedTime / duration);

            // Update the color's alpha channel
            originalColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            bgCover.GetComponent<Renderer> ().material.color = originalColor;

            yield return null;
        }

        // Ensure the final alpha value is set
        originalColor = new Color(originalColor.r, originalColor.g, originalColor.b, colAfter);
        bgCover.GetComponent<Renderer> ().material.color = originalColor;
        
        if (colBefore == 0f){
            bgElement.SetActive(false);
        }

    }

    public void bgCoverGet(RectTransform bgCoverActive)
    {
        bgCover = bgCoverActive;
    }
}
