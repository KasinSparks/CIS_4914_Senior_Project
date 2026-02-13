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

    private bool is_card_placed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playfield = GetComponent<Playfield>();


    }


    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Selected slot");
        this.is_card_placed = true;
    }

    private void AddPhysics2DRaycaster()
    {
        //Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        //if (physicsRaycaster == null)
        //{
        //    Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        //}
    }

}
