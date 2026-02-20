using UnityEngine;
using System.Collections.Generic;

public class Playfield : MonoBehaviour
{
    public List<CardSlot> card_slot;
    private Card selected_card;

    private Hand player_hand;

    public static int NUM_OF_CARDS_IN_ROW
    {
        get { return 4; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform parent = this.transform.GetChild(0);
        this.card_slot = new List<CardSlot>();

        // Adds each card slot in PlayerRow to cardSlot list
        for (int i = 0; i < NUM_OF_CARDS_IN_ROW; i++)
        {
            Transform slot = parent.GetChild(i);
            CardSlot new_slot = slot.GetComponent<CardSlot>();

            new_slot.SetPlayfield(this);
            new_slot.SetCardSlot(slot.gameObject);

            card_slot.Add(new_slot);

        }

        this.selected_card = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelectedCard(Card selected_card)
    {
        this.selected_card = selected_card;
        if (selected_card != null)
        {
            Debug.Log("Set selected card: " + selected_card.card_name);
        }
        
    }

    public void SetHand(Hand player_hand)
    {
        this.player_hand = player_hand;
    }

    public void PlaceSelectedCard(CardSlot selected_slot)
    {
        Debug.Log("Selected slot: " + selected_slot.GetCardSlot().name);
        if (selected_slot.GetIsCardPlaced())
        {
            Debug.Log("Player can not place card in occupied card slot.");
            return;
        } else if (this.selected_card == null)
        {
            Debug.Log("No card selected.");
            return;
        }
        selected_slot.SetIsCardPlaced(true);
        this.selected_card.transform.SetPositionAndRotation(
            selected_slot.transform.position,
            Quaternion.Euler(0, 0, 90)
        );
        selected_slot.SetCard(this.selected_card);

        player_hand.RemoveCard(this.selected_card);
        SetSelectedCard(null);
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
        if (!waitingForCardTarget || activeTargetedConsumable == null) {
            return;
        }
        activeTargetedConsumable.Use(target);
        waitingForCardTarget = false;
        //now that item is used tell button to disable icon
        activeButton.HideOverlay();
        activeTargetedConsumable = null;
    }
}
