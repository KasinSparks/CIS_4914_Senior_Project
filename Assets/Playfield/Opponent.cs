using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Opponent : MonoBehaviour
{
    [SerializeField] private OpponentAttackStyle attack_style;
    [SerializeField] private Playfield playfield;

    [Header("Draw Settings")]
    [SerializeField] private int draw_amount = 1;
    [Tooltip("When toggled, the draw random settings are applied")]
    [SerializeField] private bool draw_random = false;

    [Header("Draw Random Settings")]
    [Tooltip("Lowest number of cards to potentially draw")]
    [SerializeField] private int draw_random_start = 1;
    [Tooltip("Highest number of cards to potentially draw")]
    [SerializeField] private int draw_random_end = 2;

    [Header("Place Settings")]
    [SerializeField] private int place_amount = 1;
    [Tooltip("When toggled, the place random settings are applied")]
    [SerializeField] private bool place_random = false;

    [Header("Place Random Settings")]
    [Tooltip("Lowest number of cards to potentially place")]
    [SerializeField] private int place_random_start = 1;
    [Tooltip("Highest number of cards to potentially place")]
    [SerializeField] private int place_random_end = 2;

    [Header("Card Settings")]
    [SerializeField] private List<CardData> starting_cards;
    [SerializeField] private List<Card> cards;
    [SerializeField] private Queue<Card> card_queue;
    [SerializeField] private List<Card> hand;

    private class RowStatus
    {
        public int occupied_count;
        public int unoccupied_count;
        public List<CardSlot> occupied_slots;
        public List<CardSlot> unoccupied_slots;

        public RowStatus()
        {
            occupied_count = 0;
            unoccupied_count = 0;
            occupied_slots = new List<CardSlot>();
            unoccupied_slots = new List<CardSlot>();
        }

        public void AddOccupiedSlot(CardSlot slot)
        {
            occupied_slots.Add(slot);
            occupied_count = occupied_slots.Count;

            if (unoccupied_slots.Remove(slot)) unoccupied_count = unoccupied_slots.Count;
        }

        public void AddUnoccupiedSlot(CardSlot slot)
        {
            unoccupied_slots.Add(slot);
            unoccupied_count = unoccupied_slots.Count;

            if (occupied_slots.Remove(slot)) occupied_count = occupied_slots.Count;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cards = new List<Card>();

        foreach (CardData c in this.starting_cards)
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

        // If want to draw random number of cards
        if (this.draw_random && this.draw_random_end != 0)
        {
            this.draw_amount = Random.Range(this.draw_random_start, this.draw_random_end + 1);
        }

        // If cards in deck, draw amount and place in hand
        for (int i = 0; i < this.draw_amount; i++)
        {
            if (this.card_queue.Count == 0) return;

            this.hand.Add(this.GetNextCard());
        }
    }

    /**
     * @brief Calls opponent's card placement logic, updates turn state
     */
    public void Turn()
    {
        // If no cards in hand, don't place anything
        if (this.hand.Count == 0) return;

        // If want to place random number of cards
        if (this.place_random && this.place_random_end != 0)
        {
            this.place_amount = Random.Range(this.place_random_start, this.place_random_end + 1);
        }

        // Track cards placed and their index
        RowStatus player_status = new RowStatus();
        RowStatus opponent_status = new RowStatus();
        RowStatus queue_status = new RowStatus();


        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; i++)
        {
            CardSlot player_slot = playfield.GetCardSlots(CardOwnership.Player)[i];
            if (player_slot.GetIsCardPlaced())
            {
                player_status.AddOccupiedSlot(player_slot);
            }
            else
            {
                player_status.AddUnoccupiedSlot(player_slot);
            }

            CardSlot opponent_slot = playfield.GetCardSlots(CardOwnership.Opponent)[i];
            if (opponent_slot.GetIsCardPlaced())
            {
                opponent_status.AddOccupiedSlot(player_slot);
            }
            else
            {
                player_status.AddUnoccupiedSlot(player_slot);
            }
            CardSlot queue_slot = playfield.GetCardSlots(CardOwnership.Queue)[i];
            if (queue_slot.GetIsCardPlaced())
            {
                queue_status.AddOccupiedSlot(player_slot);
            } else
            {
                queue_status.AddUnoccupiedSlot(player_slot);
            }
        }

        // Logic
        for (int i = 0; i < this.place_amount; i++)
        {
            if (this.hand.Count == 0 || queue_status.unoccupied_count == 0) return;

            this.Logic(queue_status);
        }
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
     * @brief Helper function that moves card from hand to playfield for repeated lines in logic functions
     * 
     * @param handIndex index of card in hand to be moved to playfield
     */
    private void HandToPlayfield (int handIndex, int queueIndex)
    {
        CardSlot card_slot_ref = playfield.GetCardSlots(CardOwnership.Queue)[queueIndex];
        if (card_slot_ref.GetIsCardPlaced()) return;

        // Set selected card
        this.SetSelectedCard(hand[handIndex]);

        // Update card to be visable, card state to be in hand, and ownership to opponent
        hand[handIndex].gameObject.SetActive(true);
        hand[handIndex].SetState(CardState.InHand);
        hand[handIndex].SetOwnership(CardOwnership.Opponent);

        // Remove from hand
        hand.RemoveAt(handIndex);

        // Place selected card
        playfield.PlaceSelectedCard(CardOwnership.Opponent, card_slot_ref);
    }

    /**
     * @brief Calls specific logic function based on attack_style
     * 
     * @param occupied_player_slots reference to list of tuples in Turn() use to track cards placed in player slots
     * and their index
     * 
     * @param occupied_opponent_slots reference to list of tuples in Turn() use to track cards placed in opponent slots
     * and their index
     * 
     * @param occupied_queue_slots reference to list of tuples in Turn() use to track cards placed in queue slots
     * and their index
     */
    private void Logic(RowStatus queue_status)
    {
        switch (attack_style)
        {
            case OpponentAttackStyle.Random:
                LogicRandom(queue_status);
                break;
            case OpponentAttackStyle.Defensive:
                LogicDefensive();
                break;
            case OpponentAttackStyle.Aggressive:
                LogicAggressive();
                break;
            case OpponentAttackStyle.Balanced:
                LogicBalanced();
                break;
        }
    }

    private void LogicRandom(RowStatus queue_status)
    {
        int hand_index;
        int status_index;
        int hand_count = this.hand.Count;

        // Randomly choose between hand cards
        if (hand_count == 1)
        {
            hand_index = 0;
        } else
        {
            hand_index = Random.Range(0, hand_count);
        }

        // Randomly choose between open queue slots
        if (queue_status.unoccupied_count == 1)
        {
            HandToPlayfield(hand_index, queue_status.unoccupied_slots[0].GetSlotIndex());
            return;
        } else
        {
            status_index = Random.Range(0, queue_status.unoccupied_count);
        }

        // Place card 
        HandToPlayfield(hand_index, queue_status.unoccupied_slots[status_index].GetSlotIndex());
    }

    private void LogicDefensive()
    {

    }

    private void LogicAggressive()
    {

    }

    private void LogicBalanced()
    {

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
    private void AddCard(CardData card)
    {
        Card card_prefab = Resources.Load<Card>("Card");
        Card new_card = Instantiate(card_prefab, this.transform);
        new_card.SetCardData(card);
        new_card.gameObject.SetActive(false);
        new_card.SetState(CardState.InHand);
        new_card.SetOwnership(CardOwnership.Opponent);
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

}

