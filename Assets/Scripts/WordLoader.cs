using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Unity.VisualScripting;

public class WordLoader : MonoBehaviour
{
    public List<(string[] words, GameManager.ButtonPressed[] values)> wordList = new List<(string[], GameManager.ButtonPressed[])>();
    public List<(string imageName, string[] options)> chantalakWordList = new List<(string, string[])>();

    // Define custom mappings
    private readonly Dictionary<string, GameManager.ButtonPressed> valueMappings = new Dictionary<string, GameManager.ButtonPressed>
    {
        { "ครุ", GameManager.ButtonPressed.Left },
        { "ลหุ", GameManager.ButtonPressed.Right },
        { "left", GameManager.ButtonPressed.Left },
        { "right", GameManager.ButtonPressed.Right }
    };

    public void LoadWords(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string[] lines = System.IO.File.ReadAllLines(filePath);
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            string[] words = parts.Take(parts.Length / 2).ToArray();
            GameManager.ButtonPressed[] values = parts.Skip(parts.Length / 2).Select(ParseValue).ToArray();

            wordList.Add((words, values));
        }
    }

    public void LoadChantalakWords(string fileName, string folderPath)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string[] lines = System.IO.File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            string imageName = parts[0].Trim();
            string[] options = parts.Skip(1).Select(o => o.Trim()).ToArray();

            if (File.Exists(Path.Combine(folderPath, imageName + ".png")))
            {
                chantalakWordList.Add((imageName, options));
            }
            else
            {
                Debug.LogWarning($"Image {imageName} not found in folder {folderPath}.");
            }
        }
    }

    private GameManager.ButtonPressed ParseValue(string value)
    {
        value = value.Trim().ToLower();

        if (valueMappings.TryGetValue(value, out GameManager.ButtonPressed buttonPressed))
        {
            return buttonPressed;
        }
        else
        {
            Debug.LogWarning($"Unrecognized value: {value}. Defaulting to Right.");
            return GameManager.ButtonPressed.Right; // Default fallback
        }
    }
}
