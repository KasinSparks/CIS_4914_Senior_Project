using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Deck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public List<Card> starting_cards;
    private List<Card> cards;
    private Queue<Card> card_queue;

    public GameState gameState;
    public Hand hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cards = new List<Card>();

        foreach (Card c in this.starting_cards)
        {
            this.AddCard(c);
        }

        this.card_queue = new Queue<Card>();

        if (this.cards.Count < 1)
        {
            Debug.LogError("There were no starting cards in the deck.");
        }

        // Copy the starting cards to the queue
        this.Shuffle();

        // Update the card state to reflect they are currently in the deck.
        foreach (Card c in this.card_queue)
        {
            c.SetState(CardState.InDeck);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check to see if the player is eligible to draw a card
        if (gameState.current_turn_state != TurnStates.PlayerDrawCard)
        {
            Debug.Log("Player can not draw a card if the game is not in the PlayerDrawCardState.");
            return;
        }

        // Send a card from the deck to the Hand Handler
        hand.AddCard(this.GetNextCard());

        // Player has drawn a card, unset the DrawCard state so the player can not draw another
        // card this turn.
        ///gameState.current_turn_state = gameState.current_turn_state ^ (~TurnStates.PlayerDrawCard);
        gameState.current_turn_state = TurnStates.PlayerTurn;
    }


    // Suffles all the cards in the deck then puts the result into the card_queue
    // TODO: Unit tests
    public void Shuffle()
    {
        // Uses the Durstenfeld's shuffle algorithm based on Fisher-Yates shuffle
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

        Card[] shuffled_array = cards.ToArray();

        for (int i = shuffled_array.Length - 1; i > 0; --i)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Card temp = shuffled_array[j];
            shuffled_array[j] = shuffled_array[i];
            shuffled_array[i] = temp;
        }

        // Add the newly shuffled cards into the queue
        this.card_queue.Clear();
        for (int i = 0; i < shuffled_array.Length; ++i)
        {
            this.card_queue.Enqueue(shuffled_array[i]);
        }
    }
    
    // Returns the next card in the card_queue, and removes it from the queue
    public Card GetNextCard()
    {
        return this.card_queue.Dequeue();
    }

    // Returns the next card in the card_queue, but does not remove it from the queue
    public Card PeekNextCard()
    {
        throw new System.NotImplementedException();
    }

    public void AddCard(Card card)
    {
        Card new_card = Instantiate(card, this.transform);
        new_card.SetState(CardState.InDeck);
        new_card.gameObject.SetActive(false);
        this.cards.Add(new_card);
    }

    public void RemoveCard(Card card)
    {
        throw new System.NotImplementedException();
    }


}
