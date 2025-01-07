using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayScore : MonoBehaviour
{
    public int totalScore = 0;
    public TextMeshProUGUI scoreText;
    public GameObject scorePopupPrefab;
    private int scoreIncrement = 100;

    public void AddScore(int multiplier)
    {
        int scoreToAdd = scoreIncrement * multiplier;
        totalScore += scoreToAdd;

        // Update UI
        scoreText.text = totalScore.ToString();

        // Show score popup
        GameObject popup = Instantiate(scorePopupPrefab, scoreText.transform.position, Quaternion.identity, transform);
        popup.GetComponent<TextMeshProUGUI>().text = $"+{scoreToAdd}";
        Destroy(popup, 2f); // Fade out effect managed in animation/extra script
    }

    public void HalveIncrement()
    {
        scoreIncrement /= 2;
    }

    public void IncreaseIncrement()
    {
        scoreIncrement += 20;
    }
}
