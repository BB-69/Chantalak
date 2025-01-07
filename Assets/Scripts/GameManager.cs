using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI heartText; // Add a TextMeshProUGUI to display hearts as text
    public GameplayWordBlock wordBlock; // Reference to the single reusable word block
    public WordLoader wordLoader; // Reference to WordLoader

    [Header("Game Settings")]
    public int initialHearts = 3;
    public int baseScore = 100;
    public float scoreIncrementInterval = 15f;

    private Queue<(string word, GameManager.ButtonPressed[] values)> wordQueue = new Queue<(string, GameManager.ButtonPressed[])>();
    private int currentScore = 0;
    private int scoreIncrement = 100;
    private int heartsRemaining;  // Track hearts as an integer
    private bool isGameOver = false;

    public enum ButtonPressed { None, Left, Right }
    private int currentWordIndex = 0;  // Track which word in the sequence is being validated

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
    }

    private void Start()
    {
        Debug.Log("Game starting...");
        InitializeHearts();
        wordLoader.LoadWords("Assets\\Scripts\\WordList.ini"); // Ensure words are loaded first
        PopulateWordQueue(); // Populate the word queue from the loaded word list

        if (wordQueue.Count > 0)
        {
            LoadNextWord();
        }
        else
        {
            Debug.LogError("Word queue is empty at game start!");
            GameOver();
        }
    }

    private void InitializeHearts()
    {
        heartsRemaining = initialHearts;  // Initialize heart count
        UpdateHeartText();  // Display the heart count on the screen
    }

    private void PopulateWordQueue()
    {
        foreach (var wordData in wordLoader.wordList)
        {
            foreach (var word in wordData.words)
            {
                wordQueue.Enqueue((word, wordData.values));
            }
        }
    }

    public void LoadNextWord()
    {
        if (wordQueue.Count > 0)
        {
            var nextWord = wordQueue.Dequeue();
            string combinedWord = string.Join(" ", nextWord.word);  // Combine words for display
            Debug.Log($"Loading next word sequence: {combinedWord}");  // Show combined word sequence for debugging
            wordBlock.Initialize(combinedWord, nextWord.values);  // Initialize word block with the combined words and values
            currentWordIndex = 0; // Reset index for the next word sequence
        }
        else
        {
            Debug.Log("No more words in queue.");
            GameOver();
        }
    }

    public void HandleButtonPress(ButtonPressed button)
    {
        Debug.Log($"Button pressed: {button}");

        if (isGameOver) return;

        // Get the current word and its expected values
        var currentWordData = wordBlock.GetCurrentWordData();
        var currentWord = currentWordData.word;
        var expectedValues = currentWordData.values;

        bool isCorrect = (expectedValues[currentWordIndex] == button); // Check if the button press is correct for the current word

        if (isCorrect)
        {
            AddScore(1);

            currentWordIndex++;  // Move to the next word in the sequence

            if (currentWordIndex >= currentWord.Length) // If all words in the sequence are complete
            {
                currentWordIndex = 0; // Reset to start with the first word
                LoadNextWord(); // Load the next word sequence
            }
        }
        else
        {
            LoseHeart(); // Incorrect press leads to losing a heart
        }
    }

    public void OnLeftButtonClicked()
    {
        HandleButtonPress(ButtonPressed.Left);
    }

    public void OnRightButtonClicked()
    {
        HandleButtonPress(ButtonPressed.Right);
    }

    private void AddScore(int multiplier)
    {
        int scoreToAdd = scoreIncrement * multiplier;
        currentScore += scoreToAdd;
        scoreText.text = currentScore.ToString();
    }

    private void LoseHeart()
    {
        if (heartsRemaining > 0)
        {
            heartsRemaining--;  // Decrease the heart count
            UpdateHeartText();  // Update the heart display text

            if (heartsRemaining == 0)
            {
                GameOver();
            }
        }
    }

    private void UpdateHeartText()
    {
        heartText.text = $"Hearts = {heartsRemaining}";  // Update the UI text with current heart count
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game over!");
        // Implement game over logic
    }
}
