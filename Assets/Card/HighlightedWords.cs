using System;
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


    private GameObject ui_book;
    private TextMeshProUGUI book_text;
    private Image book_image;
    private TextMeshProUGUI book_text_name;

    public void Initialize(WordInfo[] words)
    {
        this.words = words;
        this.description_text.text = this.AddColorTags(this.description_text.text, words);
    }

    void Start()
    {
        this.ui_book = GameObject.Find("-----UI-----/UI_Book/Panel");
        this.book_text = GameObject.Find("-----UI-----/UI_Book/Panel/Info").GetComponent<TextMeshProUGUI>();
        this.book_text_name = GameObject.Find("-----UI-----/UI_Book/Panel/Name").GetComponent<TextMeshProUGUI>();
        this.book_image = GameObject.Find("-----UI-----/UI_Book/Panel/Image").GetComponent<Image>();
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
            Debug.Log("word index: " + word_index);
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
        // TODO(KASIN): Not the best way to check, but will work for now
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

        foreach (WordInfo word in this.words)
        {
            if (word.GetWord().ToUpper().Equals(click_word))
            {
                Debug.Log("Click on highlighted word: " + word.GetWord());
                Debug.Log("Highlighted word info: " + word.GetInfo());

                this.ui_book.SetActive(true);
                this.book_text.text = AddColorTags(word.GetInfo(), this.book_text.transform.GetComponent<HighlightedWords>().GetWordInfos());
                this.book_text_name.text = "<color=blue>" + word.GetWord().ToUpper() + "</color>";
                this.book_image.sprite = word.GetSprite();
                break;
            }
        }
    }

    private string AddColorTags(string str, WordInfo[] wi)
    {
        if (wi == null)
        {
            wi = this.words;
        }
        
        StringBuilder sb = new StringBuilder();
        // TODO(KASIN): Make it go fast...
        // TODO(KASIN): Parse better...
        string[] words = str.Split(" ");
        foreach (string word in words)
        {
            bool consumed = false;

            foreach (WordInfo word_info in wi)
            {
                if (word_info.GetWord().ToUpper().Equals(word.ToUpper()))
                {
                    Debug.Log("here");
                    sb.Append("<color=\"blue\">");
                    sb.Append(word);
                    sb.Append("</color> ");
                    Debug.Log(sb.ToString());
                    consumed = true;
                }
            }

            if (!consumed)
            {
                sb.Append(word);
                sb.Append(" ");
            }
        }

        Debug.Log(sb.ToString());
        return sb.ToString();
    }

    public WordInfo[] GetWordInfos()
    {
        return this.words;
    }
}
