using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    private GameObject card_slot;
    private Playfield playfield;
    private bool is_card_placed;
    private Card card_in_slot;

    void Awake()
    {
        // TODO: error handling
        this.playfield = this.GetComponent<Playfield>();

        this.card_in_slot = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.is_card_placed = false;
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

    public void SetPlayfield (Playfield playfield)
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
        return this.card_in_slot;
    }

    public void RemoveCard()
    {
        this.card_in_slot = null;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        playfield.PlaceSelectedCard(this);
    }


}
