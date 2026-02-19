using UnityEngine;
using System.Collections.Generic;

public class Playfield : MonoBehaviour
{
    //[SerializeField] private GameState gameState;
    [SerializeField] private Opponent opponent;

    // Player variables
    private List<CardSlot> player_card_slots;
    private Hand player_hand;
    private Card player_selected_card;

    // Opponent variables
    private List<CardSlot> opponent_card_slots;
    public Card opponent_selected_card;
    private List<CardSlot> queue_card_slots;

    private const int NUM_ROWS = 3;

    public static int NUM_OF_CARDS_IN_ROW
    {
        get { return 4; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.player_selected_card = null;
        this.opponent_selected_card = null;
        this.player_card_slots = new List<CardSlot>();
        this.opponent_card_slots = new List<CardSlot>();
        this.queue_card_slots = new List<CardSlot>();
        
        // Sets up each row (player, opponent, queue)
        for (int i = 0; i < NUM_ROWS; i++)
        {
            Transform parent = this.transform.GetChild(i);

            // Adds each card slot in row to cardSlot list
            for (int j = 0; j < NUM_OF_CARDS_IN_ROW; j++)
            {
                Transform slot = parent.GetChild(j);
                CardSlot new_slot = slot.GetComponent<CardSlot>();

                new_slot.SetPlayfield(this);
                new_slot.SetCardSlot(slot.gameObject);
                switch (i)
                {
                    case 0:
                        new_slot.SetCardOwnership(CardOwnership.Player);
                        player_card_slots.Add(new_slot);
                        break;

                    case 1:
                        new_slot.SetCardOwnership(CardOwnership.Opponent);
                        opponent_card_slots.Add(new_slot);
                        break;

                    case 2:
                        new_slot.SetCardOwnership(CardOwnership.Queue);
                        queue_card_slots.Add(new_slot);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<CardSlot> GetCardSlots(CardOwnership owner)
    {
        switch (owner)
        {
            case (CardOwnership.Player):
                return this.player_card_slots;
                break;

            case (CardOwnership.Opponent):
                return this.opponent_card_slots;
                break;

            case (CardOwnership.Queue):
                return this.queue_card_slots;
                break;

            default: return null;
        }
    }

    public CardSlot GetSpecificCardSlot(CardOwnership owner, int card_slot_index)
    {
        switch (owner)
        {
            case (CardOwnership.Player):
                return this.player_card_slots[card_slot_index];
                break;

            case (CardOwnership.Opponent):
                return this.opponent_card_slots[card_slot_index];
                break;

            case (CardOwnership.Queue):
                return this.queue_card_slots[card_slot_index];
                break;

            default: return null;
        }
    }

    public void SetSpecificCardSlot(CardOwnership owner, int card_slot_index, CardSlot card_slot)
    {
        switch (owner)
        {
            case (CardOwnership.Player):
                this.player_card_slots[card_slot_index] = card_slot;
                break;

            case (CardOwnership.Opponent):
                this.opponent_card_slots[card_slot_index] = card_slot;
                break;

            case (CardOwnership.Queue):
                this.queue_card_slots[card_slot_index] = card_slot;
                break;
        }
        
    }

    public void SetSelectedCard(CardOwnership owner, Card selected_card)
    {
        switch (owner)
        {
            case (CardOwnership.Player):
                this.player_selected_card = selected_card;
                break;

            case (CardOwnership.Opponent):
                this.opponent_selected_card = selected_card;
                break;

            default:
                Debug.Log("Can not set selected card if not player or opponent");
                return;
        }
    }

    //public void GetSelectedCard(CardOwnership owner)
    //{
    //    switch (owner)
    //    {
    //        case (CardOwnership.Player):
    //            return this.player_selected_card;
    //            break;

    //        case (CardOwnership.Opponent):
    //            return this.opponent_selected_card;
    //            break;

    //        default:
    //            Debug.Log("Can not get selected card if not player or opponent");
    //            return;
    //    }
    //}

    public void SetHand(CardOwnership owner, Hand player_hand)
    {
        if (owner != CardOwnership.Player)
        {
            Debug.Log("Non-Player can not set hand.");
            return;
        }

        this.player_hand = player_hand;
    }

    public void PlaceSelectedCard(CardOwnership owner, CardSlot selected_slot)
    {
        float scale = .01f;
        if (owner != CardOwnership.Player && owner != CardOwnership.Opponent)
        {
            Debug.Log("Non-Player/Opponent: Can not place card.");
            return;
        }
        else if (selected_slot.GetIsCardPlaced())
        {
            Debug.Log("Can not place card in occupied card slot.");
            return;
        }
        else if (this.player_selected_card == null)
        {
            Debug.Log("No card selected.");
            return;
        }
        //else if (owner != selected_slot.GetCardOwnership())
        //{
        //    Debug.Log("Card owner and slot owner do not match");
        //    return;
        //}

        selected_slot.SetIsCardPlaced(true);
        
        switch (owner)
        {
            case (CardOwnership.Player):
                this.player_selected_card.SetState(CardState.OnPlayfield);
                this.player_selected_card.transform.SetPositionAndRotation(
                    selected_slot.transform.position,
                    Quaternion.Euler(0, 0, 90)
                
                );
                selected_slot.SetCard(this.player_selected_card);
                player_hand.RemoveCard(this.player_selected_card);
                break;

            case (CardOwnership.Opponent):
                this.opponent_selected_card.SetState(CardState.OnPlayfield);
                this.opponent_selected_card.transform.SetPositionAndRotation(
                    selected_slot.transform.position,
                    Quaternion.Euler(0, 0, 90)
                );
                this.opponent_selected_card.transform.localScale = new Vector3(scale, scale, scale);
                selected_slot.SetCard(this.opponent_selected_card);
                opponent.RemoveCard(this.opponent_selected_card);
                break;
        }
        
        SetSelectedCard(owner, null);
    }

    //Below is for consumables
    private ConsumableButton activeButton;
    private HealSingleCardConsumable activeTargetedConsumable;
    private bool waitingForCardTarget = false;

    public void ActivateConsumable(HealSingleCardConsumable consumable, ConsumableButton button)
    {
        activeTargetedConsumable = consumable;
        activeButton = button;
        waitingForCardTarget = true;
        Debug.Log("Select card");
    }

    public bool IsWaitingForTarget()
    {
        return waitingForCardTarget;
    }

    public void ResolveTarget(Card target)
    {
        if (!waitingForCardTarget || activeTargetedConsumable == null)
        {
            return;
        }
        activeTargetedConsumable.Use(target);
        waitingForCardTarget = false;
        //now that item is used tell button to disable icon
        activeButton.HideOverlay();
        activeTargetedConsumable = null;
    }
}
