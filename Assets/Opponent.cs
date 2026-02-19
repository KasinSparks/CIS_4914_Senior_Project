using UnityEngine;
using System.Collections.Generic;
using System;

public class Opponent : MonoBehaviour
{
    [SerializeField] private OpponentAttackStyle attack_style;
    [SerializeField] private List<Card> starting_cards;
    [SerializeField] private GameState gameState;
    [SerializeField] private Playfield playfield;

    private List<Card> cards;
    private Queue<Card> card_queue;
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

    public List<Card> GetHand()
    {
        return this.hand;
    }


    /**
     * @brief Add a certain number of cards from the deck to the hand list.
     * @param UPDATATE
     */
    public void DrawCards()
    {

        this.UpdateRows();
        // Check to see if the player is eligible to draw a card
        //if (gameState.current_turn_state != TurnStates.OpponentDrawCard)
        //{
        //    Debug.Log("Opponent can not draw a card if the game is not in the OpponentDrawCardState.");
        //    return;
        //}

        // TODO update with loop and random numbers
        // Send a card from the deck to the hand list
        if (this.card_queue.Count != 0)
        {
            this.hand.Add(this.GetNextCard());
        }
        

        // Player has drawn a card, unset the DrawCard state so the player can not draw another
        // card this turn.
        gameState.current_turn_state = TurnStates.OpponentTurn;
        this.Turn();
    }

    /**
     * @brief Add a certain number of cards from the deck to the hand list.
     * @param UPDATATE
     */
    public void Turn()
    {

        // Check to see if the player is eligible to draw a card
        //if (gameState.current_turn_state != TurnStates.OpponentTurn)
        //{
        //    Debug.Log("Opponent palce a card if the game is not in the OpponentTurn.");
        //    return;
        //}

        // TODO update with loop and random numbers and Logic
        // Send a card from the deck to the hand list
        this.hand.Add(this.GetNextCard());
        this.TempLogic();

        // Player has drawn a card, unset the DrawCard state so the player can not draw another
        // card this turn.
        gameState.current_turn_state = TurnStates.PlayerDrawCard;
    }

    public void RemoveCard(Card card)
    {
        this.cards.Remove(card);
        card.SetState(CardState.OnPlayfield);
    }

    private void UpdateRows()
    {
        // place card at first available slot
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; i++)
        {
            CardSlot opponent_card_slot_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i];
            if (opponent_card_slot_ref.GetIsCardPlaced()) continue;

            CardSlot queue_card_slot_ref = playfield.GetCardSlots(CardOwnership.Queue)[i];
            if (!queue_card_slot_ref.GetIsCardPlaced()) continue;

            SetSelectedCard(queue_card_slot_ref.GetCard());
            playfield.PlaceSelectedCard(CardOwnership.Opponent, opponent_card_slot_ref);
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

    

    private void SetSelectedCard(Card selected_card)
    {
        playfield.SetSelectedCard(CardOwnership.Opponent, selected_card);
    }

    private void TempLogic()
    {
        // set selected card to first card in hand
        this.SetSelectedCard(hand[0]);
        hand[0].gameObject.SetActive(true);
        hand[0].SetState(CardState.InHand);
        hand[0].SetOwnership(CardOwnership.Player);

        // place card at first available slot
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; i++)
        {
            CardSlot card_slot_ref = playfield.GetCardSlots(CardOwnership.Queue)[i];
            if (card_slot_ref.GetIsCardPlaced()) continue;

            playfield.PlaceSelectedCard(CardOwnership.Opponent, card_slot_ref);

            return;
        }

    }

}

