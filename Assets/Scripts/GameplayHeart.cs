using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayHeart : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartContainer;
    private List<GameObject> hearts = new List<GameObject>();

    public void Initialize(int maxHearts)
    {
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            hearts.Add(heart);
        }
    }

    public void LoseHeart()
    {
        if (hearts.Count > 0)
        {
            Destroy(hearts[hearts.Count - 1]);
            hearts.RemoveAt(hearts.Count - 1);
        }
    }

    public void GainHeart()
    {
        GameObject heart = Instantiate(heartPrefab, heartContainer);
        hearts.Add(heart);
    }
}
