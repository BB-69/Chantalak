using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // For UI components

public class GameplayWordBlock : MonoBehaviour
{
    public static GameplayWordBlock Instance;
    public RectTransform rectTransform;
    public TextMeshProUGUI textDisplay; // Reference to the child TMP text component
    private Image imageComponent; // Reference to the Image component
    private int currentIndex = 0;  // Current word index in the sequence
    private string[] words;  // The words that are split for validation
    private GameManager.ButtonPressed[] expectedValues;  // The button presses for each word

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Cache the reference to the Image component
        imageComponent = GetComponent<Image>();
    }

    // Initialize with combined words for display and separate button press values for validation
    public void Initialize(string combinedWords, GameManager.ButtonPressed[] values)
    {
        // Display the combined words (just concatenating them with space)
        textDisplay.text = combinedWords;

        // Split the combined words by spaces to track them individually for validation
        this.words = combinedWords.Split(' ');

        // Store the button press values for each word
        this.expectedValues = values;

        // Reset the index for the current word
        currentIndex = 0;
    }

    // Get current word and its associated button presses for validation
    public (string word, GameManager.ButtonPressed[] values) GetCurrentWordData()
    {
        return (words[currentIndex], expectedValues);
    }

    // Check if the button press is correct for the current word
    public bool CheckPress(GameManager.ButtonPressed button)
    {
        if (currentIndex < words.Length)
        {
            // Validate the button press for the current word in the sequence
            if (expectedValues[currentIndex] == button)
            {
                currentIndex++;  // Move to the next word

                // If we've processed all words, return true
                if (currentIndex >= words.Length)
                {
                    Debug.Log("Correct Animation");
                    PlayRightAnimation();
                    return true;  // Sequence completed correctly
                }

                // Update the display to show the next word
                textDisplay.text = words[currentIndex];
            }
            else
            {
                PlayWrongAnimation();
                return false;  // Incorrect button press
            }
        }

        return false;  // Sequence still in progress
    }

    // Check if all words have been completed correctly
    public bool IsComplete()
    {
        return currentIndex >= words.Length;
    }

    // Play animation or effect for incorrect input
    public void PlayWrongAnimation()
    {
        Color imgCol = imageComponent.color;
        imgCol = new Color(imgCol.r, 0f, 0f, imgCol.a); // Set to red
        imageComponent.color = imgCol;

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            imgCol.g = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            imgCol.b = Mathf.Lerp(0f, 1f, elapsedTime / duration);

            // Update the color
            imageComponent.color = new Color(imgCol.r, imgCol.g, imgCol.b, imgCol.a);

            //yield return null;
        }
    }

    public void PlayRightAnimation()
    {
        Color imgCol = imageComponent.color;
        imgCol = new Color(0f, imgCol.g, 0f, imgCol.a); // Set to green
        imageComponent.color = imgCol;

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            imgCol.r = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            imgCol.b = Mathf.Lerp(0f, 1f, elapsedTime / duration);

            // Update the color
            imageComponent.color = new Color(imgCol.r, imgCol.g, imgCol.b, imgCol.a);

            //yield return null;
        }
    }
}
