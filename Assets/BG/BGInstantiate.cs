using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BGInstantiate : MonoBehaviour
{
    public GameObject imagePrefab;  // Prefab for the image to instantiate
    public int gridSize = 3;       // Size of the grid (3x3) (Odd number only)

    void Start()
    {
        float spacingx = imagePrefab.GetComponent<Transform> ().localScale.x;
        float spacingy = imagePrefab.GetComponent<Transform> ().localScale.y;
        float posz = imagePrefab.GetComponent<Transform> ().localPosition.z;

        // Loop to create a 3x3 grid of images
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Calculate the position of the new image
                Vector3 position = new Vector3((x-(gridSize-1)/2) * spacingx, (y-(gridSize-1)/2) * spacingy, posz);
                InstantiateImage(position);
            }
        }
    }

    void InstantiateImage(Vector3 position)
    {
        // Instantiate the image prefab
        GameObject imageInstance = Instantiate(imagePrefab, position, Quaternion.identity);
        imageInstance.transform.SetParent(this.transform, false);  // Set the parent to the ImageSpawner object
    }
}
