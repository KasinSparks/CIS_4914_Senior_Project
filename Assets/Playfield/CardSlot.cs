using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameState gameState;
    [SerializeField] private GameObject card_slot;
    [SerializeField] private Playfield playfield;
    [SerializeField] private bool is_card_placed;
    [SerializeField] private Card card_in_slot;
    [SerializeField] private CardOwnership card_ownership;
    private int slot_index;

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
        if (this.card_ownership != CardOwnership.Player
            || !(gameState.current_turn_state == TurnStates.PlayerTurn
            || gameState.current_turn_state == TurnStates.PlayerDrawCard)) return;
        playfield.PlaceSelectedCard(this.card_ownership, this);
    }


}
