using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class GameplayButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 normalScale = new Vector3(1f, 1f, 1f); // Default size
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f); // Size on hover
    public Vector3 pressedScale = new Vector3(1.1f, 1.1f, 1f); // Size when pressed

    public AudioSource hoverSound; // AudioSource for hover
    public AudioSource unhoverSound; // AudioSource for unhover
    public AudioSource pressSound; // AudioSource for press

    public float resizeSpeed = 10f; // Speed of size change

    private RectTransform rectTransform;
    private Vector3 targetScale;
    // private bool isAnimating = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("ButtonSizeChanger requires a RectTransform component.");
        }

        rectTransform.localScale = normalScale;
        targetScale = normalScale;
    }

    void Update()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * resizeSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;        
        PlaySound(unhoverSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = pressedScale;
        PlaySound(pressSound);
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
}