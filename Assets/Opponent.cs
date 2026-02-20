using UnityEngine;
using System.Collections.Generic;
using System;

public class Opponent : MonoBehaviour
{
    [SerializeField] private OpponentAttackStyle attack_style;
    [SerializeField] private List<Card> starting_cards;
    //[SerializeField] private GameState gameState;
    public GameState gameState;

    [SerializeField] private Playfield playfield;

    public List<Card> cards;
    public Queue<Card> card_queue;
    [SerializeField] private List<Card> hand;

    
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

    /**
     * @brief Get's opponent's hand
     * @return Reference to hand list
     */
    public List<Card> GetHand()
    {
        return this.hand;
    }


    /**
     * @brief Draw from deck, places into hand list, update turn state, calls Turn()
     */
    public void DrawCards()
    {
        // Moves queue row to opponent row if space is available
        this.UpdateRows();

        // If cards in deck, draw once and place in hand
        if (this.card_queue.Count != 0)
        {
            this.hand.Add(this.GetNextCard());
        }

        // Update turn state
        gameState.current_turn_state = TurnStates.OpponentTurn;
        this.Turn();
    }

    /**
     * @brief Calls opponent's card placement logic, updates turn state
     * 
     * @todo implement logic in place of TempLogic
     */
    public void Turn()
    {
        // TODO implement logic here
        this.TempLogic();

        // Update turn state
        gameState.current_turn_state = TurnStates.PlayerDrawCard;
    }

    /**
     * @brief Calls remove function in cards script on param, sets param's card state
     * 
     * @param card card to be removed
     */
    public void RemoveCard(Card card)
    {
        this.cards.Remove(card);
        card.SetState(CardState.OnPlayfield);
    }

    /**
     * @brief Moves cards in queue row to opponent row if space is available and resets queue row slots
     */
    private void UpdateRows()
    {
        // Go through each column
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; i++)
        {
            CardSlot opponent_card_slot_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i];
            if (opponent_card_slot_ref.GetIsCardPlaced()) continue;

            CardSlot queue_card_slot_ref = playfield.GetCardSlots(CardOwnership.Queue)[i];
            if (!queue_card_slot_ref.GetIsCardPlaced()) continue;

            // If no card in opponent row and card in queue row
            // Set selected card to queue row card, and place into opponent row
            SetSelectedCard(queue_card_slot_ref.GetCard());
            playfield.PlaceSelectedCard(CardOwnership.Opponent, opponent_card_slot_ref);

            // Reset queue row card slot
            queue_card_slot_ref.ResetCardSlot();
        }
    }

    /**
     * @brief Shuffles all the cards in the deck then puts the result into the
     * card_queue
     * 
     * Uses the Durstenfeld's shuffle algorithm based on Fisher-Yates shuffle
     * https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
     * 
     * @todo Unit tests
    */
    private void Shuffle()
    {
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

    /**
     * @brief Returns the next card in the card_queue, and removes it from the
     * queue.
     * 
     * @return Reference to the next card in the deck queue
     */
    private Card GetNextCard()
    {
        return this.card_queue.Dequeue();
    }

    /**
     * @brief Add a card to the deck.
     * @param card Reference to a card to be added to the deck.
     * 
     * Will create a new copy of the card passed then add the newly created
     * card to the deck.
     */
    private void AddCard(Card card)
    {
        Card new_card = Instantiate(card, this.transform);
        new_card.SetState(CardState.InDeck);
        new_card.gameObject.SetActive(false);
        this.cards.Add(new_card);
    }

    /**
     * @brief Sets opponent_selected_card in playfield using param
     * @param selected_card Reference to a card to set as selected.
     */
    private void SetSelectedCard(Card selected_card)
    {
        playfield.SetSelectedCard(CardOwnership.Opponent, selected_card);
    }

    /**
     * @brief Temporary logic function for opponent card placement
     * 
     * Cards are place from left to right (player's perspective) based
     * on which spot is open in the queue row
     * 
     * @todo UPDATE with logic class
     */
    private void TempLogic()
    {
        // If no cards in hand, don't place anything
        if (hand.Count == 0) return;

        // Set selected card to first in hand
        // Update card to be visable, card state to be in hand, and ownership to opponent
        // Remove from hand
        this.SetSelectedCard(hand[0]);
        hand[0].gameObject.SetActive(true);
        hand[0].SetState(CardState.InHand);
        hand[0].SetOwnership(CardOwnership.Opponent);
        hand.RemoveAt(0);

        // Place card at first available slot in queue row
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; i++)
        {
            CardSlot card_slot_ref = playfield.GetCardSlots(CardOwnership.Queue)[i];
            if (card_slot_ref.GetIsCardPlaced()) continue;

            playfield.PlaceSelectedCard(CardOwnership.Opponent, card_slot_ref);

            return;
        }

    }

}

