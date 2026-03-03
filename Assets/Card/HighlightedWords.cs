using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct HighlightedWordInfo
{
    public string word;
    [TextArea(3,10)]
    public string info;
}

public class HighlightedWords : MonoBehaviour, IPointerClickHandler
{
    private TextMeshPro description_text;
    private Card card;
    private HighlightedWordInfo[] words;

    private GameObject ui_book;
    private Image book_image;
    private TextMeshProUGUI book_text;
    private TextMeshProUGUI book_text_name;

    public void Initialize(HighlightedWordInfo[] words)
    {
        this.card = this.gameObject.GetComponentInParent<Card>();
        this.description_text = GetComponent<TextMeshPro>();
        this.words = words;

        this.ui_book = GameObject.Find("-----UI-----/UI_Book/Panel");
        this.book_text = GameObject.Find("-----UI-----/UI_Book/Panel/Info").GetComponent<TextMeshProUGUI>();
        this.book_text_name = GameObject.Find("-----UI-----/UI_Book/Panel/Name").GetComponent<TextMeshProUGUI>();
        this.book_image = GameObject.Find("-----UI-----/UI_Book/Panel/Image").GetComponent<Image>();
    }

    void Start() {}

    void Update() {}

    public void OnPointerClick(PointerEventData eventData)
    {
        int word_index = TMP_TextUtilities.FindIntersectingWord(
            this.description_text,
            eventData.pointerCurrentRaycast.worldPosition,
            null
        );
        
        Debug.DrawLine(Camera.main.transform.position,
            eventData.pointerCurrentRaycast.worldPosition,
            Color.red,
            30.0f);

        if (word_index < 0)
        {
            // User did not click on a word in the description, pass the click
            // to the card's OnPointerClick handler
            //Debug.Log("-1");
            this.card.OnPointerClick(eventData);
            return;
        }

        //Debug.Log("User click on word: " + this.description_text.textInfo.wordInfo[word_index].GetWord());
        // See if the word clicked on is in the highlighted list
        // TODO(KASIN): Not the best way to check, but will work for now
        string click_word =
            this.description_text.textInfo.wordInfo[word_index].GetWord().ToUpper();
        foreach (HighlightedWordInfo word in this.words)
        {
            if (word.word.ToUpper().Equals(click_word))
            {
                Debug.Log("Click on highlighted word: " + word.word);
                Debug.Log("Highlighted word info: " + word.info);

                this.ui_book.SetActive(true);
                this.book_text.text = word.info;
                this.book_text_name.text = "<color=blue>" + card.GetCardName() + "</color>";
                this.book_image.sprite = card.GetImage();
            }
        }
    }
}
