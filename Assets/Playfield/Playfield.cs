using UnityEngine;
using System.Collections.Generic;

public class Playfield : MonoBehaviour
{
    [SerializeField] private Opponent opponent;
    [SerializeField] private float card_scale = 1.0f;
    //public Opponent opponent;


    // Player variables
    [SerializeField] private List<CardSlot> player_card_slots;
    [SerializeField] private Hand player_hand;
    [SerializeField] private Card player_selected_card;

    // Opponent variables
    [SerializeField] private List<CardSlot> opponent_card_slots;
    [SerializeField] private Card opponent_selected_card;
    [SerializeField] private List<CardSlot> queue_card_slots;

    public static int NUM_ROWS
    {
        get { return 3; }
    }

    private GameObject sacrifice_ui_button;

    public static int NUM_OF_CARDS_IN_ROW
    {
        get { return 4; }
    }

    private int current_sacrifice_cost;
    private List<Card> current_sacrifice;
    private TurnStates game_state_prior_to_sacrifice;

    [SerializeField] private GameState game_state;

    void Awake()
    {
        this.sacrifice_ui_button = GameObject.Find("-----UI-----/EndTurnCanvas/SacrificeCancelButton");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.player_selected_card = null;
        this.opponent_selected_card = null;
        this.player_card_slots = new List<CardSlot>();
        this.opponent_card_slots = new List<CardSlot>();
        this.queue_card_slots = new List<CardSlot>();
        this.current_sacrifice = new List<Card>();
        this.current_sacrifice_cost = 0;

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
                new_slot.SetSlotIndex(j);
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

        if (owner != CardOwnership.Player && owner != CardOwnership.Opponent)
        {
            UnityEngine.Debug.Log("Non-Player/Opponent: Can not place card.");
            return;
        }
        else if (selected_slot.GetIsCardPlaced())
        {
            UnityEngine.Debug.Log("Can not place card in occupied card slot.");
            return;
        }
        else if (this.player_selected_card == null && this.opponent_selected_card == null)
        {
            UnityEngine.Debug.Log("No card selected.");
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
                UnityEngine.Debug.Log("Player Card");
                this.player_selected_card.SetState(CardState.OnPlayfield);
                // sets playfield as parent for easier scaling
                this.player_selected_card.transform.SetParent(this.transform);
                this.player_selected_card.transform.SetPositionAndRotation(
                    new Vector3(selected_slot.transform.position.x,
                    selected_slot.transform.position.y + 0.0001f * this.player_selected_card.transform.localScale.x,
                    selected_slot.transform.position.z),
                    Quaternion.Euler(0, 0, 0)
                );
                this.player_selected_card.transform.localScale = new Vector3(card_scale, card_scale, card_scale);

                selected_slot.SetCard(this.player_selected_card);
                player_hand.RemoveCard(this.player_selected_card);
                this.player_selected_card.SetSlot(selected_slot);
                break;

            case (CardOwnership.Opponent):
                UnityEngine.Debug.Log("Opponent Card");
                this.opponent_selected_card.SetState(CardState.OnPlayfield);
                // sets playfield as parent for easier scaling
                this.opponent_selected_card.transform.SetParent(this.transform);

                this.opponent_selected_card.transform.SetPositionAndRotation(
                    new Vector3(selected_slot.transform.position.x,
                    selected_slot.transform.position.y + 0.0001f * this.opponent_selected_card.transform.localScale.x,
                    selected_slot.transform.position.z),
                    Quaternion.Euler(0, 0, 0)
                );

                this.opponent_selected_card.transform.localScale = new Vector3(card_scale, card_scale, card_scale);
                selected_slot.SetCard(this.opponent_selected_card);
                opponent.RemoveCard(this.opponent_selected_card);
                this.opponent_selected_card.SetSlot(selected_slot);
                break;
        }
        
        // Send the OnPlayfieldUpdate to the cards for the given owner
        this.RegisterPlayfieldUpdate(owner);
        
        SetSelectedCard(owner, null);
    }

    public void RegisterPlayfieldUpdate(CardOwnership owner)
    {
        int num_of_hymenopteras = 0;
        List<CardSlot> slots = this.GetCardSlots(owner);
        foreach (CardSlot slot in slots)
        {
            if (slot.GetCard() == null)
            {
                continue;
            }

            if (slot.GetCard().GetOrder() == CardOrder.Hymenoptera)
            {
                num_of_hymenopteras++;
            }
        }
         
        foreach (CardSlot slot in slots)
        {
            if (slot.GetCard() == null)
            {
                continue;
            }

            slot.GetCard().OnPlayfieldUpdate(num_of_hymenopteras);
        }
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

    public void AddSacrificeCard(Card card)
    {
        // Add the card to the current sacrifice data structure
        this.current_sacrifice.Add(card);

        // Check if nektar cost requirement has been met
        if (this.HasSacrificeRequirementBeenMet())
        {
            // Sacarifice the cards
            foreach (Card c in this.current_sacrifice)
            {
                c.OnSacrifice();
            }

            this.SetSacrificeButtonActive(false);
        }

    }

    public void SetCurrentSacrificeCost(int cost)
    {
        this.current_sacrifice_cost = cost;
        if (this.current_sacrifice_cost > 0)
        {
            this.game_state_prior_to_sacrifice = this.game_state.GetCurrentState();

            // Set the turn state to sacrifice mode
            this.game_state.UpdateTurnState(TurnStates.PlayerSacrifice);

            // Update the card state
            this.player_selected_card.card_state = CardState.RequireSacrifice;

            // Add a button to the UI so the player can cancel the sacrifice
            this.sacrifice_ui_button.SetActive(true);
        }
    }

    public bool HasSacrificeRequirementBeenMet()
    {
        int current_nektar_amount_proposed = 0;
        foreach (Card c in this.current_sacrifice)
        {
            current_nektar_amount_proposed += c.GetCardData().nektar_given_when_scarificed;
        }

        return current_nektar_amount_proposed >= this.current_sacrifice_cost;
    }

    public void ResetSacrificeRequirements()
    {
        this.current_sacrifice.Clear();
        this.current_sacrifice_cost = 0;
    }

    public void CancelSacrifice()
    {
        foreach (Card c in this.current_sacrifice)
        {
            c.SetToBeSacrificed(false);
        }

        this.player_selected_card.card_state = CardState.InHand;

        this.player_hand.SetSelectedCard(CardOwnership.Player, null);

        this.game_state.UpdateTurnState(this.GetTurnStatePriorToSacrifice());

        this.ResetSacrificeRequirements();

        this.sacrifice_ui_button.SetActive(false);
    }

    public TurnStates GetTurnStatePriorToSacrifice()
    {
        return this.game_state_prior_to_sacrifice;
    }

    public void SetSacrificeButtonActive(bool active)
    {
        this.sacrifice_ui_button.SetActive(active);
    }
}
