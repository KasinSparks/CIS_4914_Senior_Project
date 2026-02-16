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

    void Awake()
    {
        // TODO: error handling
        this.playfield = this.GetComponent<Playfield>();
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



    public void OnPointerClick(PointerEventData eventData)
    {
        playfield.PlaceSelectedCard(this);
    }


}
