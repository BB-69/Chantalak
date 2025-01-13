using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPage : MonoBehaviour
{
    public GameObject PlayUI;
    //public GameObject Content;

    private Vector2 firstPos, secondPos;

    // Start is called before the first frame update
    void Start()
    {
        firstPos = PlayUI.GetComponent<RectTransform>().anchoredPosition;
        secondPos = new Vector2(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator playOpen()
    {
        PlayUI.SetActive(true);
        
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        { 
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = EaseOutCubic(t);
            Vector2 targetPos = Vector2.Lerp(firstPos, secondPos, t);
            PlayUI.GetComponent<RectTransform>().anchoredPosition = targetPos;

            yield return null;
        }
    }

    public IEnumerator playClose()
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector2 currentPos = PlayUI.GetComponent<RectTransform>().anchoredPosition;
        //Vector2 contentCurrentPos = Content.GetComponent<RectTransform>().anchoredPosition;
        //Vector2 contentTargetPos = new Vector2(contentCurrentPos.x, 0);

        while (elapsedTime < duration)
        { 
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = EaseOutCubic(t);
            Vector2 targetPos = Vector2.Lerp(currentPos, firstPos, t);
            PlayUI.GetComponent<RectTransform>().anchoredPosition = targetPos;
            //Vector2 contentPos = Vector2.Lerp(contentCurrentPos, contentTargetPos, t);
            //Content.GetComponent<RectTransform>().anchoredPosition = contentPos;

            yield return null;
        }

        PlayUI.SetActive(false);
    }

    private float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
}
