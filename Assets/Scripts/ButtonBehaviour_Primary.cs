using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonBehaviour_Primary : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 normalScale = new Vector3(1f, 1f, 1f); // Default size
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f); // Size on hover
    public Vector3 pressedScale = new Vector3(1.1f, 1.1f, 1f); // Size when pressed

    public AudioSource hoverSound; // Sound effect for hover
    public AudioSource unhoverSound; // Sound effect for unhover
    public AudioSource pressSound; // Sound effect for press

    public RectTransform primaryButton;
    public Vector3 primaryButtonPointA; // Original position
    public Vector3 primaryButtonPointB; // Shifted position

    public float resizeSpeed = 10f; // Speed of size change
    public float moveSpeed = 5f; // Speed of movement

    public RectTransform otherButton1;
    public RectTransform otherButton2;
    public RectTransform otherButton3;
    public RectTransform otherButton4;

    public TMP_Text otherButton1Text;
    public TMP_Text otherButton2Text;
    public TMP_Text otherButton3Text;
    public TMP_Text otherButton4Text;

    public Vector3 otherButton1PointA; // Starting position for button 1
    public Vector3 otherButton1PointB; // Target position for button 1
    public Vector3 otherButton2PointA; // Starting position for button 2
    public Vector3 otherButton2PointB; // Target position for button 2
    public Vector3 otherButton3PointA; // Starting position for button 3
    public Vector3 otherButton3PointB; // Target position for button 3
    public Vector3 otherButton4PointA; // Starting position for button 4
    public Vector3 otherButton4PointB; // Target position for button 4

    private RectTransform rectTransform;
    private AudioSource audioSource;
    private Vector3 targetScale;
    private bool isPressed = false;
    private bool isAnimating = false; // Flag to indicate if animation is in progress

    // Add Button references
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button primaryButtonButton; // Reference to the primary button itself

    public CentralCode centralCode;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("ButtonSizeChanger requires a RectTransform component.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure the button starts with its normal scale
        rectTransform.localScale = normalScale;
        targetScale = normalScale;

        // Ensure text is hidden initially
        SetTextVisibility(false);

        // Disable the other buttons initially
        DisableOtherButtons();

        // Disable the primary button initially
        primaryButtonButton.interactable = false;
    }

    void Update()
    {
        // Smoothly interpolate towards the target scale
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * resizeSpeed);

        if (centralCode.isAnimating)
        {
            targetScale = normalScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (centralCode.isAnimating) return;

        targetScale = hoverScale;
        hoverSound.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (centralCode.isAnimating) return;

        targetScale = normalScale;
        unhoverSound.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (centralCode.isAnimating) return;

        targetScale = pressedScale;
        pressSound.Play();

        // Prevent button press if animation is running
        if (isAnimating) return;

        ToggleButtonPosition();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (centralCode.isAnimating) return;

        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, null))
        {
            targetScale = hoverScale;
        }
        else
        {
            targetScale = normalScale;
        }
    }

    private void ToggleButtonPosition()
    {
        isPressed = !isPressed;

        // If already animating, prevent further interactions
        if (isAnimating) return;

        // Start the animation flag
        isAnimating = true;

        // Disable all buttons to prevent rapid clicking during movement and text animation
        DisableOtherButtons();
        primaryButtonButton.interactable = false;

        // Move primary button
        StartCoroutine(MovePrimaryButton(isPressed ? primaryButtonPointB : primaryButtonPointA));

        // Move other buttons with distinct positions
        MoveOtherButton(otherButton1, isPressed ? otherButton1PointB : otherButton1PointA, otherButton1Text, isPressed);
        MoveOtherButton(otherButton2, isPressed ? otherButton2PointB : otherButton2PointA, otherButton2Text, isPressed);
        MoveOtherButton(otherButton3, isPressed ? otherButton3PointB : otherButton3PointA, otherButton3Text, isPressed);
        MoveOtherButton(otherButton4, isPressed ? otherButton4PointB : otherButton4PointA, otherButton4Text, isPressed);

        // Wait until animations are finished
        StartCoroutine(EnableAllButtonsAfterAnimation());
    }

    private IEnumerator MovePrimaryButton(Vector3 targetPosition)
    {
        Vector3 startPosition = primaryButton.anchoredPosition;
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * moveSpeed;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            primaryButton.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);
            yield return null;
        }

        primaryButton.anchoredPosition = targetPosition; // Ensure precision
    }

    private void MoveOtherButton(RectTransform button, Vector3 targetPosition, TMP_Text text, bool showText)
    {
        if (button == null) return;
        StartCoroutine(MoveAndToggleText(button, targetPosition, text, showText));
    }

    private IEnumerator MoveAndToggleText(RectTransform button, Vector3 targetPosition, TMP_Text text, bool showText)
    {
        Vector3 startPosition = button.anchoredPosition;
        float progress = 0f;

        // Hide text before starting movement
        if (!showText && text != null)
        {
            text.gameObject.SetActive(false);
        }

        while (progress < 1f)
        {
            progress += Time.deltaTime * moveSpeed;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            button.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);
            yield return null;
        }

        button.anchoredPosition = targetPosition; // Ensure precision

        // Show text after movement is complete
        if (showText && text != null)
        {
            text.gameObject.SetActive(true);
        }
    }

    private void SetTextVisibility(bool visible)
    {
        if (otherButton1Text != null) otherButton1Text.gameObject.SetActive(visible);
        if (otherButton2Text != null) otherButton2Text.gameObject.SetActive(visible);
        if (otherButton3Text != null) otherButton3Text.gameObject.SetActive(visible);
        if (otherButton4Text != null) otherButton4Text.gameObject.SetActive(visible);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Disable the other buttons and the primary button
    private void DisableOtherButtons()
    {
        if (button1 != null) button1.interactable = false;
        if (button2 != null) button2.interactable = false;
        if (button3 != null) button3.interactable = false;
        if (button4 != null) button4.interactable = false;
    }

    // Enable all buttons after the movement and animation are completed
    private IEnumerator EnableAllButtonsAfterAnimation()
    {
        // Wait until the movements and animations are finished
        yield return new WaitForSeconds((Mathf.Max(moveSpeed, resizeSpeed) / 50f));

        // Enable the buttons after the action is done
        if (isPressed)
        {
            EnableOtherButtons();
            primaryButtonButton.interactable = true;
        }

        // End the animation flag
        isAnimating = false;
    }

    // Enable the other buttons and the primary button
    private void EnableOtherButtons()
    {
        if (button1 != null) button1.interactable = true;
        if (button2 != null) button2.interactable = true;
        if (button3 != null) button3.interactable = true;
        if (button4 != null) button4.interactable = true;
    }
}