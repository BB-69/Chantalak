using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SideBarInstantiate : MonoBehaviour
{
    public CentralCode centralCode;
    public GameObject imagePrefab;  // Prefab for the image to instantiate
    public GameObject leftSideBar;
    public GameObject rightSideBar;
    public int gridSizex = 1;
    public int gridSizey = 7;
    private Vector2 screenBounds;

    void Start()
    {
        screenBounds = GetScreenBounds();

        Transform transform = imagePrefab.GetComponent<Transform>();
        Vector3 pos = transform.localPosition;
        Vector2 imageSize = transform.localScale / 2;
        pos.x = -screenBounds.x - imageSize.x; // At left edge
        pos.y = transform.localPosition.y;
        pos.z = transform.localPosition.z;

        float spacingx = 0f; //imagePrefab.GetComponent<Transform> ().localScale.x;
        float spacingy = imagePrefab.GetComponent<Transform> ().localScale.y;

        // Loop for both left edge and right edge
        for (int i = 0; i < 2; i++)
        {
            if (i == 1) pos.x = -pos.x;
            // Loop to create a 3x3 grid of images
            for (int x = 0; x < gridSizex; x++)
            {
                for (int y = 0; y < gridSizey; y++)
                {
                    // Calculate the position of the new image
                    Vector3 position = new Vector3(((x-(gridSizex-1)/2)*spacingx)+pos.x, ((y-(gridSizey-1)/2)*spacingy)+pos.y, pos.z);
                    InstantiateImage(position, i);
                }
            }
        }
        
        Destroy(imagePrefab);
    }

    void Update()
    {
        if (imagePrefab != null) if (centralCode.getSideBarSizeDone) Destroy(imagePrefab);
    }

    void InstantiateImage(Vector3 position, int i)
    {
        // Instantiate the image prefab
        if (i == 0) // Left edge
        {
            GameObject imageInstance = Instantiate(imagePrefab, position, Quaternion.identity);
            imageInstance.transform.SetParent(leftSideBar.transform, false);  // Set the parent to the empty parent leftSideBar
        }
        else        // Right edge
        {
            GameObject imageInstance = Instantiate(imagePrefab, position, Quaternion.identity);
            imageInstance.transform.SetParent(rightSideBar.transform, false);  // Set the parent to the empty parent rightSideBar
        }
    }

    private Vector2 GetScreenBounds()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        return canvasRect.sizeDelta / 2;
    }
}