using UnityEngine;
using System.Collections.Generic;

public class BookWordInitialize : MonoBehaviour
{
    [SerializeField]
    private HighlightedWords word_obj;

    void Awake()
    {
        // Load the word info if it exist
        if (SaveSystem.CheckForFileExistence(SaveSystemFile.WordInfo))
        {
            WordInfo[] words = SaveSystem.LoadWords();
            word_obj.Initialize(words);
            Debug.Log("Loaded word information from file");
        }
        else
        {
            WordInfo[] default_words = word_obj.GetDefaultWords();
            SaveSystem.SaveWordInfo(default_words);
            Debug.Log("Loaded default word information");
        }
    }
}