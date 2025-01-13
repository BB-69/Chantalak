using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI heartText;
    public GameplayWordBlock wordBlock;
    public WordLoader wordLoader;
    public TextMeshProUGUI sequenceText;

    [Header("Game Settings")]
    public int initialHearts = 3;
    public int baseScore = 100;

    private Queue<(string word, GameManager.ButtonPressed[] values)> wordQueue = new Queue<(string, GameManager.ButtonPressed[])>();
    private int currentScore = 0;
    private int heartsRemaining;
    private bool isGameOver = false;

    public enum ButtonPressed { None, Left, Right }

    private List<ButtonPressed> userInputSequence = new List<ButtonPressed>(); // Track user's button sequence
    private ButtonPressed[] expectedSequence; // Store the current sequence to match against

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
        wordLoader.LoadWords("Assets\\Scripts\\WordList.ini");
        PopulateWordQueue();

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

    private void Update()
    {
        if (isGameOver) return;

        // Handle Left Button Press from Keyboard
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnLeftButtonClicked();
        }

        // Handle Right Button Press from Keyboard
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnRightButtonClicked();
        }

        // Handle Confirm Button Press from Keyboard
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            OnConfirmButtonClicked();
        }

        sequenceText.text = GetSequenceInThai(userInputSequence);
    }

    private void InitializeHearts()
    {
        heartsRemaining = initialHearts;
        UpdateHeartText();
    }

    private void PopulateWordQueue()
    {
        // Shuffle the word list randomly
        var shuffledWordList = wordLoader.wordList.OrderBy(_ => Random.value).ToList();

        foreach (var wordData in shuffledWordList)
        {
            var combinedWord = string.Join("", wordData.words); // Combine words into one displayable string
            wordQueue.Enqueue((combinedWord, wordData.values));
        }
    }

    public void LoadNextWord()
    {
        if (wordQueue.Count > 0)
        {
            var nextWord = wordQueue.Dequeue();
            Debug.Log($"Loading next word sequence: {nextWord.word}");

            wordBlock.Initialize(nextWord.word, nextWord.values);

            expectedSequence = nextWord.values; // Store expected button sequence
            userInputSequence.Clear(); // Clear previous user input
        }
        else
        {
            Debug.Log("No more words in queue.");
            GameOver();
        }

        if (wordQueue.Count < 2) PopulateWordQueue();
    }

    public void HandleButtonPress(ButtonPressed button)
    {
        if (isGameOver) return;

        Debug.Log($"Button pressed: {button}");
        userInputSequence.Add(button); // Add button press to user input sequence
    }

    public void OnLeftButtonClicked()
    {
        HandleButtonPress(ButtonPressed.Left);
    }

    public void OnRightButtonClicked()
    {
        HandleButtonPress(ButtonPressed.Right);
    }

    public void OnConfirmButtonClicked()
    {
        if (isGameOver) return;

        Debug.Log("Confirm button pressed. Validating sequence...");

        if (userInputSequence.Count != expectedSequence.Length)
        {
            Debug.LogWarning("Sequence length mismatch! Incorrect sequence.");
            GameplayWordBlock.Instance.PlayWrongAnimation();
            userInputSequence.Clear();
            LoseHeart();
            return;
        }

        for (int i = 0; i < userInputSequence.Count; i++)
        {
            if (userInputSequence[i] != expectedSequence[i])
            {
                Debug.LogWarning("Sequence mismatch! Incorrect sequence.");
                GameplayWordBlock.Instance.PlayWrongAnimation();
                userInputSequence.Clear();
                LoseHeart();
                return;
            }
        }

        Debug.Log("Correct sequence entered!");
        GameplayWordBlock.Instance.PlayRightAnimation();
        AddScore(1);
        LoadNextWord();
    }

    private void AddScore(int multiplier)
    {
        int scoreToAdd = baseScore * multiplier;
        currentScore += scoreToAdd;
        scoreText.text = currentScore.ToString();
    }

    private void LoseHeart()
    {
        if (heartsRemaining > 0)
        {
            heartsRemaining--;
            UpdateHeartText();

            if (heartsRemaining == 0)
            {
                GameOver();
            }
        }
    }

    private void UpdateHeartText()
    {
        heartText.text = $"Hearts = {heartsRemaining}";
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game over!");

        SettingsManager.Instance.ChangeSceneToResult();
    }

    private string GetSequenceInThai(List<ButtonPressed> sequence)
    {
        // Map the ButtonPressed values to their Thai equivalents
        Dictionary<ButtonPressed, string> buttonToThai = new Dictionary<ButtonPressed, string>
        {
            { ButtonPressed.Left, "ครุ" },
            { ButtonPressed.Right, "ลหุ" }
        };

        List<string> thaiSequence = new List<string>();
        foreach (var button in sequence)
        {
            if (buttonToThai.TryGetValue(button, out string thaiValue))
            {
                thaiSequence.Add(thaiValue);
            }
        }

        return string.Join("-", thaiSequence); // Join the sequence with hyphens
    }
}
