using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightedWords : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshPro description_text;

    [SerializeField]
    private Card card;

    [SerializeField]
    private WordInfo[] words;

    private Dictionary<string, WordInfo> dict_word_info;


    private GameObject ui_book;
    private TextMeshProUGUI book_text;
    private Image book_image;
    private TextMeshProUGUI book_text_name;
    
    /**
     * @brief Initialize the dictionary to track words and info.
     */
    private void InitDict()
    {
        this.dict_word_info = new Dictionary<string, WordInfo>();
        foreach (WordInfo word_info in this.words)
        {
            foreach (string word in word_info.GetWords())
            {
                this.dict_word_info[word.ToUpper()] = word_info;
            }
        }
    }
    
    /**
     * @brief Use this method when attached to card to initialize this class.
     */
    public void Initialize(WordInfo[] words)
    {
        this.words = words;
        this.InitDict();
        this.description_text.text = this.AddColorTags(this.description_text.text, this.dict_word_info);
    }

    void Awake()
    {
        this.ui_book = GameObject.Find("-----UI-----/UI_Book/Panel");
        this.book_text = GameObject.Find("-----UI-----/UI_Book/Panel/Info").GetComponent<TextMeshProUGUI>();
        this.book_text_name = GameObject.Find("-----UI-----/UI_Book/Panel/Name").GetComponent<TextMeshProUGUI>();
        this.book_image = GameObject.Find("-----UI-----/UI_Book/Panel/Image").GetComponent<Image>();
        this.InitDict();
    }

    void Update() {}

    public void OnPointerClick(PointerEventData eventData)
    {
        // CLEANUP(KASIN): 
        int word_index = -1;
        if (this.description_text != null)
        {
            word_index = TMP_TextUtilities.FindIntersectingWord(
                this.description_text,
                eventData.pointerCurrentRaycast.screenPosition,
                Camera.main
            );
        }
        else
        {
            word_index = TMP_TextUtilities.FindIntersectingWord(
                this.book_text,
                eventData.pointerCurrentRaycast.screenPosition,
                null
            );
        }

        if (word_index < 0)
        {
            // User did not click on a word in the description, pass the click
            // to the card's OnPointerClick handler
            if (this.card != null)
            {
                this.card.OnPointerClick(eventData);
            }
            return;
        }

        // See if the word clicked on is in the highlighted list
        // CLEANUP(KASIN):
        string click_word = "";
        if (this.description_text != null)
        {
            click_word = this.description_text.textInfo.wordInfo[word_index].GetWord().ToUpper();
        }
        else
        {
            click_word = this.book_text.textInfo.wordInfo[word_index].GetWord().ToUpper();
        }
        
        if (this.dict_word_info.ContainsKey(click_word.ToUpper()))
        {
            WordInfo word_info = this.dict_word_info[click_word.ToUpper()];

            this.ui_book.SetActive(true);
            this.book_text.text = AddColorTags(word_info.GetInfo(), this.book_text.transform.GetComponent<HighlightedWords>().GetDict());
            this.book_text_name.text = "<color=blue><u>" + click_word.ToUpper() + "</u></color>";
            this.book_image.sprite = word_info.GetSprite();
        }
    }
    
    /**
     * @brief Adds color tags around a key word.
     * @param str The string of words to parse through.
     * @param wi The keyword information
     * @return A new string that has the key words colored blue.
     */
    private string AddColorTags(string str, Dictionary<string, WordInfo> dict)
    {
        StringBuilder sb = new StringBuilder();
        // TODO(KASIN): Make it go fast...
        // TODO(KASIN): Parse better...
        List<string> tokens = new List<string>();
        
        StringBuilder curr_token = new StringBuilder();
        foreach (char c in str)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                curr_token.Append(c);
            }
            else
            {
                if (curr_token.Length > 0)
                {
                    tokens.Add(curr_token.ToString());
                    curr_token.Clear();
                }

                tokens.Add(c.ToString());
            }
        }

        if (curr_token.Length > 0)
        {
            // flush
            tokens.Add(curr_token.ToString());
        }

        foreach (string token in tokens)
        {
            if (token.Length == 1)
            {
                sb.Append(token);
                continue;
            }

            if (dict.ContainsKey(token.ToUpper()))
            {
                sb.Append("<color=\"blue\"><u>");
                sb.Append(token);
                sb.Append("</u></color>");
            }
            else
            {
                sb.Append(token);
            }
        }

        return sb.ToString();
    }
    
    /**
     * @brief Get the dictionary with the word information
     * @reutrn The current word information dictionary
     */
    public Dictionary<string, WordInfo> GetDict()
    {
        return this.dict_word_info;
    }

}
