using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
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
        public List<int> occupied_slots;
        public List<int> unoccupied_slots;

        public RowStatus()
        {
            occupied_count = 0;
            unoccupied_count = 0;
            occupied_slots = new List<int>();
            unoccupied_slots = new List<int>();
        }

        public void AddOccupiedSlot(int index)
        {
            occupied_slots.Add(index);
            occupied_count = occupied_slots.Count;

            if (unoccupied_slots.Remove(index)) unoccupied_count = unoccupied_slots.Count;
        }

        public void AddUnoccupiedSlot(int index)
        {
            unoccupied_slots.Add(index);
            unoccupied_count = unoccupied_slots.Count;

            if (occupied_slots.Remove(index)) occupied_count = occupied_slots.Count;
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
                player_status.AddOccupiedSlot(i);
            }
            else
            {
                player_status.AddUnoccupiedSlot(i);
            }

            CardSlot opponent_slot = playfield.GetCardSlots(CardOwnership.Opponent)[i];
            if (opponent_slot.GetIsCardPlaced())
            {
                opponent_status.AddOccupiedSlot(i);
            }
            else
            {
                opponent_status.AddUnoccupiedSlot(i);
            }
            CardSlot queue_slot = playfield.GetCardSlots(CardOwnership.Queue)[i];
            if (queue_slot.GetIsCardPlaced())
            {
                queue_status.AddOccupiedSlot(i);
            } else
            {
                queue_status.AddUnoccupiedSlot(i);
            }
        }

        // Logic
        for (int i = 0; i < this.place_amount; i++)
        {
            if (this.hand.Count == 0 || queue_status.unoccupied_count == 0) return;

            // Calls specific logic function based on attack_style
            switch (this.attack_style)
            {
                case OpponentAttackStyle.Random:
                    LogicRandom(queue_status);
                    break;
                case OpponentAttackStyle.Defensive:
                    LogicDefensive(player_status, opponent_status, queue_status);
                    break;
                case OpponentAttackStyle.Aggressive:
                    LogicAggressive(player_status, opponent_status, queue_status);
                    break;
                case OpponentAttackStyle.Balanced:
                    LogicBalanced(player_status, opponent_status, queue_status);
                    break;
            }
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
     * @param queue_status use to track cards placed in queue slots, updated here
     * @param handIndex index of card in hand to be moved to playfield
     * @param queueIndex index of cardslot in queue for card to be placed
     */
    private void HandToPlayfield (RowStatus queue_status, int handIndex, int queueIndex)
    {
        CardSlot card_slot_ref = playfield.GetCardSlots(CardOwnership.Queue)[queueIndex];
        if (card_slot_ref.GetIsCardPlaced()) return;

        // Update queue_status
        queue_status.AddOccupiedSlot(queueIndex);

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
     * @brief Randomly chooses from hand and places card in random open queue slot
     * 
     * @param queue_status use to track cards placed in queue slots
     */
    private void LogicRandom(RowStatus queue_status)
    {
        int hand_index;
        int queue_status_index;

        // Randomly choose between hand cards
        hand_index = Random.Range(0, this.hand.Count);

        // Randomly choose between open queue slots
        queue_status_index = Random.Range(0, queue_status.unoccupied_count);

        // Place card 
        HandToPlayfield(queue_status, hand_index, queue_status.unoccupied_slots[queue_status_index]);
    }

    /**
     * @brief Chooses cards with highest hp from hand and places card in front of players card with highest attack,
     * wants to block player cards as much as possible
     * 
     * Opponent will place cards with highest hp in front of player card with highest attack. If there isn't a player
     * card on the field, the opponent will spread out their cards as much as possible; otherwise, they will place cards
     * in random queue row with player cards in the same column. If there is not a queue row spot open with a player
     * card in the same column, they will place cards in any random open queue slot
     * 
     * @param player_status use to track cards placed in player slots
     * 
     * @param opponent_status use to track cards placed in opponent slots
     * 
     * @param queue_status use to track cards placed in queue slots
     */
    private void LogicDefensive(RowStatus player_status, RowStatus opponent_status, RowStatus queue_status)
    {
        int hand_index;
        int queue_index;

        // Get hand card with highest hp, breaks tie based on attack
        Card highest_HP = this.hand[0];
        hand_index = 0;
        for (int i = 0; i < this.hand.Count; i++)
        {
            Card card = this.hand[i];
            if (card.GetCurrentHP() > highest_HP.GetCurrentHP() ||
                (card.GetCurrentHP() == highest_HP.GetCurrentHP() && card.GetBaseAttack() > highest_HP.GetBaseAttack()))
            {
                highest_HP = card;
                hand_index = i;
            }
        }

        var unoccupied_slots = queue_status.unoccupied_slots.Intersect(opponent_status.unoccupied_slots);
        var valid_slots = player_status.occupied_slots.Intersect(unoccupied_slots);
        var backup_slots = player_status.occupied_slots.Intersect(queue_status.unoccupied_slots);

        // If no player card on field 
        if (player_status.occupied_count == 0)
        {
            // If exists column with queue and opponent slots open, place in random queue slot without opponent
            if (unoccupied_slots.Any())
            {
                queue_index = unoccupied_slots.ElementAt(Random.Range(0, unoccupied_slots.Count()));
            } // else, place in random queue slot
            else 
            {
                queue_index = queue_status.unoccupied_slots[Random.Range(0, queue_status.unoccupied_count)];
            }
        } // If player card on field but not in column with queue and opponent slots open
        else if (player_status.occupied_count != 0 && !valid_slots.Any())
        {
            // If exists column with queue and player slots open, place random there
            if (backup_slots.Any())
            {
                queue_index = backup_slots.ElementAt(Random.Range(0, backup_slots.Count()));
            } // If exists column with queue and opponent slots open, place in random queue slot without opponent
            else if (unoccupied_slots.Any())
            {
                queue_index = unoccupied_slots.ElementAt(Random.Range(0, unoccupied_slots.Count()));
            } // else, place in random queue slot
            else
            {
                queue_index = queue_status.unoccupied_slots[Random.Range(0, queue_status.unoccupied_count)];
            }
        } // If player card on field and exists column with queue and opponent slots open
        else if (valid_slots.Any())
        {
            // Get slot index with highest player attack and open player and queue, breaks tie based on HP
            queue_index = valid_slots.ElementAt(0);
            foreach (int index in valid_slots)
            {
                Card card = playfield.GetSpecificCardSlot(CardOwnership.Player, index).GetCard();
                Card highest_attack = playfield.GetSpecificCardSlot(CardOwnership.Player, queue_index).GetCard();
                if (card.GetBaseAttack() > highest_attack.GetBaseAttack()
                    || (card.GetBaseAttack() == highest_attack.GetBaseAttack() && card.GetCurrentHP() > highest_attack.GetCurrentHP()))
                {
                    queue_index = index;
                }
            }
        } // else, place in random queue slot
        else
        {
            queue_index = queue_status.unoccupied_slots[Random.Range(0, queue_status.unoccupied_count)];
        }
        
        HandToPlayfield(queue_status, hand_index, queue_index);
    }

    /**
     * @brief Chooses cards with highest attack from hand and places card in front of players card with highest HP,
     * wants to attack player directly as much as possible
     * 
     * Opponent will place cards with highest attack in front of player card with highest HP. If there isn't a player
     * card on the field, the opponent will spread out their cards as much as possible; otherwise, they will place cards
     * in random queue row with player cards in the same column. If there is not a queue row spot open with a player
     * card in the same column, they will place cards in any random open queue slot
     * 
     * 
     * @param player_status use to track cards placed in player slots
     * 
     * @param opponent_status use to track cards placed in opponent slots
     * 
     * @param queue_status use to track cards placed in queue slots
     */
    private void LogicAggressive(RowStatus player_status, RowStatus opponent_status, RowStatus queue_status)
    {
        int hand_index;
        int queue_index;

        // Get hand card with highest attack, breaks tie based on hp
        Card highest_attack = this.hand[0];
        hand_index = 0;
        for (int i = 0; i < this.hand.Count; i++)
        {
            Card card = this.hand[i];
            if (card.GetBaseAttack() > highest_attack.GetBaseAttack() ||
                (card.GetBaseAttack() == highest_attack.GetBaseAttack() && card.GetCurrentHP() > highest_attack.GetCurrentHP()))
            {
                highest_attack = card;
                hand_index = i;
            }
        }

        var unoccupied_slots = queue_status.unoccupied_slots.Intersect(opponent_status.unoccupied_slots);
        var valid_slots = player_status.occupied_slots.Intersect(unoccupied_slots);
        var backup_slots = player_status.occupied_slots.Intersect(queue_status.unoccupied_slots);

        // If no player card on field 
        if (player_status.occupied_count == 0)
        {
            // If exists column with queue and opponent slots open, place in random queue slot without opponent
            if (unoccupied_slots.Any())
            {
                queue_index = unoccupied_slots.ElementAt(Random.Range(0, unoccupied_slots.Count()));
            } // else, place in random queue slot
            else
            {
                queue_index = queue_status.unoccupied_slots[Random.Range(0, queue_status.unoccupied_count)];
            }
        } // If player card on field but not in column with queue and opponent slots open but exists
          // column with queue and opponent slots open
        else if (player_status.occupied_count != 0 && !valid_slots.Any() && unoccupied_slots.Any())
        {
            // place in random queue slot without opponent
            queue_index = unoccupied_slots.ElementAt(Random.Range(0, unoccupied_slots.Count()));
            
        } // If player card on field and exists column with queue and opponent slots open
        else if (valid_slots.Any())
        {
            // Get slot index with highest player HP and open player and queue, breaks tie based on attack
            queue_index = valid_slots.ElementAt(0);
            foreach (int index in valid_slots)
            {
                Card card = playfield.GetSpecificCardSlot(CardOwnership.Player, index).GetCard();
                Card highest_HP = playfield.GetSpecificCardSlot(CardOwnership.Player, queue_index).GetCard();
                if (card.GetCurrentHP() > highest_HP.GetCurrentHP()
                    || (card.GetCurrentHP() == highest_HP.GetCurrentHP() && card.GetBaseAttack() > highest_HP.GetBaseAttack()))
                {
                    queue_index = index;
                }
            }
        } // else, place in random queue slot
        else
        {
            queue_index = queue_status.unoccupied_slots[Random.Range(0, queue_status.unoccupied_count)];
        }

        HandToPlayfield(queue_status, hand_index, queue_index);
    }

    /**
     * @brief Chooses cards with highest attack from hand and places card in front of players card with highest HP,
     * wants to attack player directly as much as possible
     * 
     * Opponent will place cards with highest attack in front of player card with highest HP. If there isn't a player
     * card on the field, the opponent will spread out their cards as much as possible; otherwise, they will place cards
     * in random queue row with player cards in the same column. If there is not a queue row spot open with a player
     * card in the same column, they will place cards in any random open queue slot
     * 
     * 
     * @param player_status use to track cards placed in player slots
     * 
     * @param opponent_status use to track cards placed in opponent slots
     * 
     * @param queue_status use to track cards placed in queue slots
     */
    private void LogicBalanced(RowStatus player_status, RowStatus opponent_status, RowStatus queue_status)
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

