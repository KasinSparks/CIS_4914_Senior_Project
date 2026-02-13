using UnityEngine;
using System.Collections.Generic;

public class Playfield : MonoBehaviour
{
    public List<CardSlot> cardslot;
    private Card selected_card;
    private int numRow;

    //[SerializeField] private PlaceCard placeCard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cardslot = new List<CardSlot>();
        this.selected_card = null;
        this.numRow = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceCard(Card card)
    {
        // Instantiate card and add it to the hand list
        //Card new_card = Instantiate(card, this.transform);

        //// Update the newly instantiated card's fields 
        //new_card.gameObject.SetActive(true);
        //new_card.SetState(CardState.InHand);

        //// Add the card to the hand
        //this.cards.Add(new_card);

        //// Move card to position
        //this.FlareCards();

    }

    public void SetSelectedCard(Card selected_card)
    {
        this.selected_card = selected_card;
        Debug.Log("Set selected card: " + selected_card.card_name);
    }

    public void PlaceSelectedCard()
    {
        for (int i = 0; i < numRow - 1; i++)
        {
            foreach (Transform child in this.transform)
            {
                //if child.name ==
                //Whatever w = childTransform.GetComponent<Whatever>();
                //w.DoThing();
            }
        }

    }
}
