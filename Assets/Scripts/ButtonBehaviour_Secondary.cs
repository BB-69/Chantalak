using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ButtonBehaviour_Secondary : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 normalScale = new Vector3(1f, 1f, 1f); // Default size
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f); // Size on hover
    public Vector3 pressedScale = new Vector3(1.1f, 1.1f, 1f); // Size when pressed

    public AudioSource hoverSound; // AudioSource for hover
    public AudioSource unhoverSound; // AudioSource for unhover
    public AudioSource pressSound; // AudioSource for press
    public AudioSource edgeSound; // AudioSource for edge sound
    public AudioSource movementSound; // AudioSource for movement sound

    public float resizeSpeed = 10f; // Speed of size change
    public float movementSpeed = 1690f; // Speed of movement
    public RectTransform backRect; // Rectangle that will stretch with the button

    private RectTransform rectTransform;
    private Vector3 targetScale;
    // private bool isAnimating = false;
    private bool isReverse = false;

    private Vector2 originalPos;
    private Vector2 firstPos;
    private Vector2 secondPos;
    private Quaternion normalRotation;
    private Vector2 screenBounds;

    public CentralCode centralCode;
    public string thisMenu;

    void Start()
    {
        backRectAlpha(0f);

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("ButtonSizeChanger requires a RectTransform component.");
        }

        

        // Ensure the button starts with its normal scale
        rectTransform.localScale = normalScale;
        targetScale = normalScale;

        screenBounds = GetScreenBounds();
    }

    void Update()
    {

        // Smoothly interpolate towards the target scale
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * resizeSpeed);

        // Stretch the rectangle to simulate the effect
        StretchRectangle(rectTransform.anchoredPosition - originalPos);

        if (!centralCode.isAnimating)
        {
            if (!isReverse)
            {
                originalPos = rectTransform.anchoredPosition;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (centralCode.isAnimating) return;

        if (!isReverse)
        {
            targetScale = hoverScale;
        }
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (centralCode.isAnimating) return;

        if (!isReverse)
        {
            targetScale = normalScale;
        }
        PlaySound(unhoverSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (centralCode.isAnimating) return;

        if (!isReverse)
        {
            targetScale = pressedScale;
        }
        PlaySound(pressSound);

        if (!centralCode.isAnimating)
        {
            if (!isReverse)
            {
                StartCoroutine(AnimateButton());
            }
            else
            {
                StartCoroutine(ReverseAnimateButton());
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = normalScale;
    }

    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    private IEnumerator AnimateButton()
    {
        backRectAlpha(1f);
        centralCode.isAnimating = true;
        centralCode.buttonIsNormal = false;
        centralCode.bgCoverGet(backRect);

        // Move front a layer
        rectTransform.localPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, -200);

        // Store normal rotation
        normalRotation = rectTransform.rotation;

        // Rotate to 0 degrees
        float elapsedTime = 0f;
        float duration = 0.5f; // Time to rotate
        while (elapsedTime < duration)
        {
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, Quaternion.Euler(0, 0, 0), elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // Move to the top edge
        centralCode.touchBlock(true);
        yield return MoveToEdge(Vector2.up);

        // Pause at the top edge
        yield return new WaitForSeconds(0.2f); // Pause duration at the top edge

        // Move to the left edge
        yield return MoveToEdge(Vector2.left);

        centralCode.disableButtons = true;
        isReverse = !isReverse;
        centralCode.isAnimating = false;

        // Pause at the left edge
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(activateMenu());

        // Pause before continue
        yield return new WaitForSeconds(1f);

        centralCode.touchBlock(false);
        rectTransform.localPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, -300f);
    }

    private IEnumerator ReverseAnimateButton()
    {
        rectTransform.localPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, -200f);
        centralCode.isAnimating = true;
        centralCode.disableButtons = false;
        centralCode.touchBlock(true);

        yield return disableMenu();

        yield return new WaitForSeconds(0.6f);

        // Move back from the left edge
        yield return MoveToEdge(Vector2.right);

        // Pause at the top edge
        yield return new WaitForSeconds(0.2f);

        // Move back from the top edge
        yield return MoveToEdge(Vector2.down);

        // Pause at the original position
        yield return new WaitForSeconds(0.2f);

        // Move back a layer
        rectTransform.localPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, 0);

        // Rotate back to normal degrees
        float elapsedTime = 0f;
        float duration = 0.5f; // Time to rotate
        while (elapsedTime < duration)
        {
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, normalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        centralCode.buttonIsNormal = true;
        isReverse = !isReverse;
        centralCode.isAnimating = false;

        // Pause at normal degrees
        yield return new WaitForSeconds(0.4f);
        backRectAlpha(0f);

        centralCode.touchBlock(false);
    }

    private IEnumerator MoveToEdge(Vector2 direction)
    {
        
        Vector2 targetPos = rectTransform.anchoredPosition;

        // Calculate the button size (half-width and half-height) for precise alignment
        Vector2 buttonSize = rectTransform.sizeDelta / 2;

        if (direction == Vector2.up)
        {
            targetPos.y = screenBounds.y - buttonSize.y; // Top edge
            firstPos = rectTransform.anchoredPosition; // Store first position for reverse callback
        }
        else if (direction == Vector2.left)
        {
            targetPos.x = -screenBounds.x + buttonSize.x; // Left edge
            secondPos = rectTransform.anchoredPosition; // Store second position for reverse callback
        }
        else if (direction == Vector2.right)
        {
            targetPos = secondPos;
        }
        else if (direction == Vector2.down)
        {
            targetPos = firstPos;
        }


        FadeInSound(movementSound);

        // Use a loop to smoothly move to the target position
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPos) > 0.1f)
        {
            

            // Calculate movement step based on speed and frame time
            float step = movementSpeed * Time.deltaTime; // Dynamically calculate step

            Vector2 currentPos = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = Vector2.MoveTowards(currentPos, targetPos, step);

            backRect.anchoredPosition = -rectTransform.anchoredPosition + originalPos;

            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;

        if (direction == Vector2.up || direction == Vector2.left)
        {
            PlaySound(edgeSound);
        }
        FadeOutSound(movementSound);
    }

    private Vector2 GetScreenBounds()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        return canvasRect.sizeDelta / 2;
    }

    private void StretchRectangle(Vector2 backRectDeltaPos)
    {
        Vector2 buttonSize = rectTransform.sizeDelta;
        Vector2 backRectSize = backRect.sizeDelta;

        Vector2 targetSize = backRectSize;
        targetSize.x = -backRectDeltaPos.x*2 + buttonSize.x;
        targetSize.y = backRectDeltaPos.y*2*(screenBounds.y/(originalPos.y-(buttonSize.y/2))) + buttonSize.y;

        backRect.localScale = new Vector2(targetSize.x, targetSize.y);
    }

    private void FadeInSound(AudioSource audioSource)
    {
        if (audioSource != null && audioSource != null)
        {
            StartCoroutine(FadeSound(audioSource, 0f, 1f));
        }
    }

    private void FadeOutSound(AudioSource audioSource)
    {
        if (audioSource != null && audioSource != null)
        {
            StartCoroutine(FadeSound(audioSource, 1f, 0f));
        }
    }

    IEnumerator FadeSound(AudioSource audioSource, float startVolume, float endVolume)
    {
        if (startVolume == 0f)
        {
            audioSource.Stop();
        }

        float elapsedTime = 0f;
        float duration = 0.1f; // Time to fade in/out
        audioSource.volume = startVolume;
        audioSource.Play();


        while (elapsedTime < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = endVolume;

        if (endVolume == 0f)
        {
            audioSource.Stop();
            //Destroy(audioSource);
        }
    }

    private void backRectAlpha(float alpha)
    {
        var backRectCol = backRect.GetComponent<Renderer> ().material.color;
        backRectCol = new Color(backRectCol.r, backRectCol.g, backRectCol.b, alpha);
        backRect.GetComponent<Renderer> ().material.color = backRectCol;
    }

    IEnumerator activateMenu()
    {
        if (thisMenu == "play"){
            centralCode.playClicked(false);
        }
        else if (thisMenu == "options"){
            centralCode.optionsClicked(false);
        }
        else if (thisMenu == "info"){
            centralCode.infoClicked(false);
        }
        else if (thisMenu == "exit"){
            centralCode.exitClicked();
        }



        yield return null;
    }

    IEnumerator disableMenu()
    {
        if (thisMenu == "play"){
            centralCode.playClicked(true);
        }
        else if (thisMenu == "options"){
            centralCode.optionsClicked(true);
        }
        else if (thisMenu == "info"){
            centralCode.infoClicked(true);
        }



        yield return null;
    }
}