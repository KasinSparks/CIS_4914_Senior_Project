using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Deck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public CardOwnership owner;

    public List<CardData> starting_cards;
    private List<CardData> cards;
    private Queue<CardData> card_queue;

    public GameState gameState;
    public Hand hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cards = new List<CardData>();

        foreach (CardData c in this.starting_cards)
        {
            this.AddCard(c);
        }

        this.card_queue = new Queue<CardData>();

        if (this.cards.Count < 1)
        {
            Debug.LogError("There were no starting cards in the deck.");
        }

        // Copy the starting cards to the queue
        this.Shuffle();
    }

    // Update is called once per frame
    void Update() {}

    public void OnPointerEnter(PointerEventData eventData) {}

    public void OnPointerExit(PointerEventData eventData) {}
    
    /**
     * @brief When a user clicks on their deck, add a card from the deck to the
     * hand.
     * @param eventData
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.owner != CardOwnership.Player)
        {
            return;
        }

        // TODO(KASIN): Change this to use the function call in gameState

        // Check to see if the player is eligible to draw a card
        if (gameState.current_turn_state != TurnStates.PlayerDrawCard)
        {
            Debug.Log("Player can not draw a card if the game is not in the PlayerDrawCardState.");
            return;
        }

        // Send a card from the deck to the Hand Handler
        hand.AddCard(this.GetNextCard(), this.owner);

        // Player has drawn a card, unset the DrawCard state so the player can not draw another
        // card this turn.
        // gameState.current_turn_state = gameState.current_turn_state ^ (~TurnStates.PlayerDrawCard);
        gameState.current_turn_state = TurnStates.PlayerTurn;
    }

    /**
     * @brief Suffles all the cards in the deck then puts the result into the
     * card_queue
     * 
     * Uses the Durstenfeld's shuffle algorithm based on Fisher-Yates shuffle
     * https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
     * 
     * @todo Unit tests
    */
    public void Shuffle()
    {
        CardData[] shuffled_array = cards.ToArray();

        for (int i = shuffled_array.Length - 1; i > 0; --i)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            CardData temp = shuffled_array[j];
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
    
    /**
     * @brief Returns the next card in the card_queue, and removes it from the
     * queue.
     * @return Reference to the next card in the deck queue
     */
    public CardData GetNextCard()
    {
        return this.card_queue.Dequeue();
    }

    /**
     * @brief Returns the next card in the card_queue, but does not remove it
     * from the queue.
     * @return Reference to the next card in the deck queue
     */
    public Card PeekNextCard()
    {
        throw new System.NotImplementedException();
    }

    /**
     * @brief Add a card to the deck.
     * @param card Reference to a card to be added to the deck.
     * 
     * Will create a new copy of the card passed then add the newly created
     * card to the deck.
     */
    public void AddCard(CardData card)
    {
        this.cards.Add(card);
    }
    
    /**
     * @brief Removes a Card from the deck
     * @todo Implement this function
     */
    public void RemoveCard(Card card)
    {
        throw new System.NotImplementedException();
    }


}
