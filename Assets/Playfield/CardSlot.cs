using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    
    [SerializeField] private GameObject card_slot;
    [SerializeField] private bool is_card_placed;
    [SerializeField] private Card card_in_slot;
    [SerializeField] private CardOwnership card_ownership;
    [SerializeField] private PlayfieldUpgrade playfieldUpgrade; //for upgrading
    private int slot_index;
    [SerializeField] private GameState gameState;
    [SerializeField] private Playfield playfield;

    void Awake()
    {
        this.playfield = this.GetComponent<Playfield>();

        this.is_card_placed = false;

        this.card_in_slot = null;

        this.card_ownership = CardOwnership.None;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void SetCardSlot(GameObject card_slot)
    {
        this.card_slot = card_slot;
    }

    public GameObject GetCardSlot()
    {
        return this.card_slot;
    }

    public void SetPlayfield(Playfield playfield)
    {
        this.playfield = playfield;
    }

    public void SetPlayfieldUpgrade(PlayfieldUpgrade playfieldfUpgrade) //for sacrafice playfield
    {
        this.playfieldUpgrade = playfieldfUpgrade;
    }

    public bool GetIsCardPlaced()
    {
        return this.is_card_placed;
    }

    public void SetIsCardPlaced(bool is_card_placed)
    {
        this.is_card_placed = is_card_placed;
    }

    public void SetCard(Card card)
    {
        this.card_in_slot = card;
    }

    // Returns null if there is not a card currently in the slot
    public Card GetCard()
    {
        if (this.card_in_slot == null)
        {
            return null;
        }
        return this.card_in_slot;
    }

    public void ResetCardSlot()
    {
        Debug.Log("Card reset");
        this.is_card_placed = false;
        this.card_in_slot = null;
    }

    public void SetCardOwnership(CardOwnership owner)
    {
        this.card_ownership = owner;
    }

    public CardOwnership GetCardOwnership()
    {
        return this.card_ownership;
    }

    public void SetSlotIndex(int index)
    {
        this.slot_index = index;
    }

    public int GetSlotIndex()
    {
        return this.slot_index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playfieldUpgrade != null) //if there was a upgrade playfield use that instead
        {
            if (!is_card_placed && playfieldUpgrade.selectedUpgradeCard != null)
            {
                playfieldUpgrade.PlaceCard(playfieldUpgrade.selectedUpgradeCard, this);
            }
            return;
        }

        if (this.card_ownership != CardOwnership.Player)
        {
            return;
        }

        // Check to see if nektar cost requirement has been met
        if (!playfield.HasSacrificeRequirementBeenMet())
        {
            // TODO(KASIN): Change this to show in the UI or something...
            Debug.Log("Nekar requirement has not been met to place this card.");
            return;
        }
        else
        {
            if (this.gameState.GetCurrentState() == TurnStates.PlayerSacrifice)
            {
                // Reset the nektar cost
                this.playfield.ResetSacrificeRequirements();
                
                // Update the turn state
                this.gameState.UpdateTurnState(playfield.GetTurnStatePriorToSacrifice());

                // Hide the cancel sacrifice button
                this.playfield.SetSacrificeButtonActive(false);
            }    
        }

        if (!(gameState.GetCurrentState() == TurnStates.PlayerTurn
            || gameState.GetCurrentState() == TurnStates.PlayerDrawCard)) return;

        playfield.PlaceSelectedCard(this.card_ownership, this);
    }


}
