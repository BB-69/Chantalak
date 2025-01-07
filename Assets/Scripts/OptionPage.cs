using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPage : MonoBehaviour
{
    public GameObject OptionUI;
    public GameObject Content;

    private Vector2 firstPos, secondPos;

    // Start is called before the first frame update
    void Start()
    {
        firstPos = OptionUI.GetComponent<RectTransform>().anchoredPosition;
        secondPos = new Vector2(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator optionOpen()
    {
        OptionUI.SetActive(true);

        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        { 
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = EaseOutCubic(t);
            Vector2 targetPos = Vector2.Lerp(firstPos, secondPos, t);
            OptionUI.GetComponent<RectTransform>().anchoredPosition = targetPos;

            yield return null;
        }
    }

    public IEnumerator optionClose()
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector2 currentPos = OptionUI.GetComponent<RectTransform>().anchoredPosition;
        Vector2 contentCurrentPos = Content.GetComponent<RectTransform>().anchoredPosition;
        Vector2 contentTargetPos = new Vector2(contentCurrentPos.x, 0);

        while (elapsedTime < duration)
        { 
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = EaseOutCubic(t);
            Vector2 targetPos = Vector2.Lerp(currentPos, firstPos, t);
            OptionUI.GetComponent<RectTransform>().anchoredPosition = targetPos;
            Vector2 contentPos = Vector2.Lerp(contentCurrentPos, contentTargetPos, t);
            Content.GetComponent<RectTransform>().anchoredPosition = contentPos;

            yield return null;
        }

        OptionUI.SetActive(false);
    }

    private float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
}
