using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedAspectRatio : MonoBehaviour
{
    public float targetAspect = 16f / 9f; // Desired aspect ratio (e.g., 16:9)

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Calculate the target aspect ratio
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        // Adjust the camera viewport
        Camera camera = GetComponent<Camera>();
        if (scaleHeight < 1.0f)
        {
            // Add letterbox (black bars at the top and bottom)
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            // Add pillarbox (black bars at the sides)
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }
    }
}

