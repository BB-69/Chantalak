using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;

public class WordLoader : MonoBehaviour
{
    public List<(string[] words, GameManager.ButtonPressed[] values)> wordList = new List<(string[], GameManager.ButtonPressed[])>();

    public void LoadWords(string filePath)
    {
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

    private GameManager.ButtonPressed ParseValue(string value)
    {
        return value.Trim().ToLower() == "left" ? GameManager.ButtonPressed.Left : GameManager.ButtonPressed.Right;
    }
}
