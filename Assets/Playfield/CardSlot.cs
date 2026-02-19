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
<<<<<<< Updated upstream:Assets/CardSlot.cs
=======
    private Card card_in_slot;
    private CardOwnership card_ownership;
>>>>>>> Stashed changes:Assets/Playfield/CardSlot.cs

    void Awake()
    {
        // TODO: error handling
        this.playfield = this.GetComponent<Playfield>();
<<<<<<< Updated upstream:Assets/CardSlot.cs
=======

        this.card_in_slot = null;

        this.card_ownership = CardOwnership.None;
>>>>>>> Stashed changes:Assets/Playfield/CardSlot.cs
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
<<<<<<< Updated upstream:Assets/CardSlot.cs

=======

    public void SetCard(Card card)
    {
        this.card_in_slot = card;
    }

    // Returns null if there is not a card currently in the slot
    public Card GetCard()
    {
        return this.card_in_slot;
    }

    public void ResetCardSlot()
    {
        this.card_slot = null;
        this.is_card_placed = false;
        this.card_in_slot = null;
    }
>>>>>>> Stashed changes:Assets/Playfield/CardSlot.cs

    public void SetCardOwnership(CardOwnership owner)
    {
        this.card_ownership = owner;
    }

    public CardOwnership GetCardOwnership()
    {
        return this.card_ownership;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.card_ownership != CardOwnership.Player) return;
        playfield.PlaceSelectedCard(this.card_ownership, this);
    }


}
