using UnityEngine;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{

    public List<Card> cards;

    public GameObject card_starting_mark;
    public GameObject card_ending_mark;

    private const float MAX_CARD_FLARE_ANGLE = 140;

    private Card front_card;
    private float front_card_original_z;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cards = new List<Card>();
        front_card = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCard(Card card)
    {
        // Instantiate card and add it to the hand list
        Card new_card = Instantiate(card, this.transform);

        // Update the newly instantiated card's fields 
        new_card.gameObject.SetActive(true);
        new_card.SetState(CardState.InHand);

        // Add the card to the hand
        this.cards.Add(new_card);

        // Move card to position
        this.FlareCards();

    }

    public void RemoveCard(Card card)
    {
        throw new System.NotImplementedException();
    }

    public void MoveCardFront(Card card)
    {
        if (this.front_card != null)
        {
            // Move the current front card back
            this.front_card.transform.SetPositionAndRotation(
                new Vector3(
                    this.front_card.transform.position.x,
                    this.front_card.transform.position.y,
                    this.front_card_original_z),
                this.front_card.transform.rotation
            );
        }

        // Move the new front card forward
        this.front_card = card;
        this.front_card_original_z = this.front_card.transform.position.z;
        this.front_card.transform.SetPositionAndRotation(
            new Vector3(
                this.front_card.transform.position.x,
                this.front_card.transform.position.y,
                this.card_starting_mark.transform.position.z - 0.1f),
            this.front_card.transform.rotation
        );
    }

    private void FlareCards()
    {
        Vector3 heading = this.card_ending_mark.transform.position
            - this.card_starting_mark.transform.position;

        float distance = heading.magnitude;

        // Divide spacing equally between the cards in the hand
        float spacing = distance / this.cards.Count;

        // Unit vector of the distance vector between start and end marks
        Vector3 unit_vector = heading / distance;

        // Go through each card an update its' position.
        for (int i = 0; i < this.cards.Count; ++i)
        {
            this.cards[i].transform.position =
                this.card_starting_mark.transform.position
                + (unit_vector * spacing * i);
        }

        this.front_card = this.cards[0];
    }
}
