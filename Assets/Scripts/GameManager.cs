using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject[] sequenceButtons;
    public GameObject[] optionButtons; // Buttons for 4-choice gameplay
    public UnityEngine.UI.Image imageDisplay; // To show images from ChantalakList
    public GameObject imageDisplayObject;
    public GameObject textSequenceObject;
    public GameObject screenEffect;

    [Header("Game Settings")]
    public int initialHearts = 3;
    public int baseScore = 100;

    private Queue<(string word, GameManager.ButtonPressed[] values)> wordQueue = new Queue<(string, GameManager.ButtonPressed[])>();
    private Queue<(string word, GameManager.ButtonPressed[] values)> chantalakQueue = new Queue<(string, GameManager.ButtonPressed[])>();
    private Dictionary<string, Sprite> chantalakImages = new Dictionary<string, Sprite>();
    public List<(string imageName, GameManager.ButtonPressed[] options)> chantalakWordList = new List<(string, GameManager.ButtonPressed[])>();

    private int currentScore = 0;
    private int heartsRemaining;
    private bool isGameOver = false;

    private string currentChantalakWord;
    private ButtonPressed[] currentChantalakValue;
    private bool inChantalakMode = false;
    private bool inChantalakSequenceMode = false;

    public enum ButtonPressed { None, Left, Right }

    private List<ButtonPressed> userInputSequence = new List<ButtonPressed>(); // Track user's button sequence
    private Vector2 textSequenceOriginPos;
    private Vector2 textSequenceCurrentPos;
    private Vector2 textSequenceTargetPos;
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
        SceneManager.LoadScene("CentralManager", LoadSceneMode.Additive);

        Debug.Log("Game starting...");
        InitializeHearts();
        wordLoader.LoadWords("WordList.ini");
        wordLoader.LoadChantalakWords("ChantalakWordList.ini");
        LoadChantalakImages();
        PopulateWordQueue();
        LoadNextWord();
        textSequenceCurrentPos = textSequenceObject.GetComponent<RectTransform>().anchoredPosition;
        textSequenceTargetPos = textSequenceCurrentPos;
        StartCoroutine(screenEffectAlpha(0f, 0f));
        

        /*if (wordQueue.Count > 0)
        {
            LoadNextWord();
        }
        else
        {
            Debug.LogError("Word queue is empty at game start!");
            GameOver();
        }*/
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

        if (Vector2.Distance(textSequenceCurrentPos, textSequenceTargetPos) > 0.1f){
            textSequenceCurrentPos = Vector2.MoveTowards(textSequenceCurrentPos, textSequenceTargetPos,
                Vector2.Distance(textSequenceCurrentPos, textSequenceTargetPos)*0.3f);;
            textSequenceObject.GetComponent<RectTransform>().anchoredPosition = textSequenceCurrentPos;
        }
        if (Vector2.Distance(textSequenceCurrentPos, textSequenceTargetPos) <= 0.1f && textSequenceCurrentPos!=textSequenceTargetPos){
            textSequenceCurrentPos = textSequenceTargetPos;
            textSequenceObject.GetComponent<RectTransform>().anchoredPosition = textSequenceCurrentPos;
        }
        textSequenceCurrentPos = textSequenceObject.GetComponent<RectTransform>().anchoredPosition;
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

    private void LoadChantalakImages()
    {
        var images = Resources.LoadAll<Sprite>("ChantalakList");
        foreach (var image in images)
        {
            chantalakImages[image.name] = image;
        }
    }

    public void LoadNextWord()
    {
        if (wordQueue.Count > 0)
        {
            if (Random.Range(0f, 1f) > 0.5f) // 50% chance to enter Chantalak mode
            {
                EnterChantalakMode();
            }
            else
            {
                var nextWord = wordQueue.Dequeue();
                Debug.Log($"Loading next word sequence: {nextWord.word}");

                wordBlock.Initialize(nextWord.word, nextWord.values);

                expectedSequence = nextWord.values; // Store expected button sequence
                userInputSequence.Clear(); // Clear previous user input
            }
        }
        else
        {
            Debug.Log("No more words in queue.");
            GameOver();
        }

        if (wordQueue.Count < 2) PopulateWordQueue();
    }

    private void EnterChantalakMode()
    {
        userInputSequence.Clear();

        if (chantalakWordList.Count == 0)
        {
            Debug.LogError("ChantalakWordList is empty. Cannot enter Chantalak mode.");
            LoadNextWord(); // Fallback to a normal word sequence
            return;
        }
        
        inChantalakMode = true;

        // Select a random image and corresponding word
        var selectedChantalakEntry = chantalakWordList[Random.Range(0, chantalakWordList.Count)];
        currentChantalakWord = selectedChantalakEntry.imageName;
        currentChantalakValue = selectedChantalakEntry.options;
        imageDisplay.sprite = chantalakImages[currentChantalakWord];

        // Prepare options for 4 buttons
        var options = new List<string> { currentChantalakWord };
        while (options.Count < 4)
        {
            var randomWord = chantalakWordList[Random.Range(0, chantalakWordList.Count)].imageName;
            if (!options.Contains(randomWord))
            {
                options.Add(randomWord);
            }
        }

        options = options.OrderBy(_ => Random.value).ToList();
        for (int i = 0; i < optionButtons.Length; i++)
        {
            var button = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            button.text = options[i];

            var index = i; // Capture index for the lambda
            optionButtons[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnChantalakButtonClicked(options[index]));
        }

        foreach (var button in optionButtons)
        {
            button.SetActive(true);
        }
        imageDisplayObject.SetActive(true);
        foreach (var button in sequenceButtons)
        {
            button.SetActive(false);
        }
        wordBlock.gameObject.SetActive(false);
    }

    private void ExitChantalakMode()
    {
        inChantalakSequenceMode = false;

        imageDisplayObject.SetActive(false);
        wordBlock.gameObject.SetActive(true);
    }

    private void EnterChantalakSequenceMode()
    {
        userInputSequence.Clear();
        
        inChantalakMode = false;
        inChantalakSequenceMode = true;

        chantalakQueue.Enqueue((currentChantalakWord, currentChantalakValue));
        var nextWord = chantalakQueue.Dequeue();
        expectedSequence = nextWord.values;
        userInputSequence.Clear();

        foreach (var button in optionButtons)
        {
            button.SetActive(false);
            button.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        }
        foreach (var button in sequenceButtons)
        {
            button.SetActive(true);
        }
    }

    private void OnChantalakButtonClicked(string selectedWord)
    {
        if (selectedWord == currentChantalakWord)
        {
            Debug.Log("Correct Chantalak button pressed!");
            AddScore(1);
            EnterChantalakSequenceMode(); // Move to two-button sequence mode
        }
        else
        {
            Debug.LogWarning("Incorrect Chantalak button pressed!");
            LoseHeart();
        }
    }

    public void HandleButtonPress(ButtonPressed button)
    {
        if (isGameOver) return;

        Debug.Log($"Button pressed: {button}");
        userInputSequence.Add(button); // Add button press to user input sequence

        if (userInputSequence.Count > 0) textSequenceTargetPos.y = textSequenceOriginPos.y + 120*Mathf.Floor((userInputSequence.Count-1)/8);
    }

    public void OnLeftButtonClicked()
    {
        if (inChantalakMode) return;
        HandleButtonPress(ButtonPressed.Left);
    }

    public void OnRightButtonClicked()
    {
        if (inChantalakMode) return;
        HandleButtonPress(ButtonPressed.Right);
    }

    public void OnConfirmButtonClicked()
    {
        if (isGameOver) return;

        Debug.Log("Confirm button pressed. Validating sequence...");

        if (inChantalakMode){

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
        }





        else{

            if (expectedSequence == null || expectedSequence.Length == 0)
            {
                Debug.LogError("Expected sequence is null or empty! Check word list or word queue.");
                GameOver();
                return;
            }

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

            if (inChantalakSequenceMode){
                ExitChantalakMode();
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
                StartCoroutine(screenEffectAlpha(1f, 1f));
                GameOver();
            }
            StartCoroutine(screenEffectAlpha(0.7f, 0f));
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

    private IEnumerator screenEffectAlpha(float alphaBefore, float alphaAfter)
    {
        if (alphaBefore != 0f){
            screenEffect.SetActive(true);
        }

        var screenCol = screenEffect.GetComponent<Renderer>().material.color;
        screenCol = new Color(screenCol.r, screenCol.g, screenCol.b, alphaBefore);

        if (alphaBefore == alphaAfter) yield break;

        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration){
            screenCol.a = Mathf.Lerp(alphaBefore, alphaAfter, elapsedTime/duration);
            screenEffect.GetComponent<Renderer>().material.color = screenCol;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        screenCol.a = alphaAfter;
        screenEffect.GetComponent<Renderer>().material.color = screenCol;

        if (alphaAfter == 0f){
            screenEffect.SetActive(false);
        }
    }
}
