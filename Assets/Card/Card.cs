using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Card : MonoBehaviour
{
    // NOTE: Changes to these fields in the editor will only appear when the game is in a RUNNING state.
    public string card_name;
    public string description;

    public int hp;
    public int attack;
    public int play_cost;

    public List<CardModifier> modifiers;

    //public Material card_image;
    public Texture card_image;

    public CardRarity card_rarity;

    private CardState card_state;

    private CardOwnership card_ownership;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the card state to the default value
        this.card_state = CardState.None;

        // Initialize the card ownership to the default value
        this.card_ownership = CardOwnership.None;


        // Card name text set
        this.SetCardTextField("card_name_text", this.card_name);

        // Card description text set
        this.SetCardTextField("card_description_text", this.description);

        // Card HP Text set
        this.SetCardTextField("card_health_text", this.hp.ToString());

        // Card Attack Text set
        this.SetCardTextField("card_attack_text", this.attack.ToString());

        // Card Cost Text set
        this.SetCardTextField("card_cost_text", this.play_cost.ToString());

        // Card Image set
        if (this.card_image == null)
        {
            throw new MissingComponentException("Unable to find image for " + this.card_image.name);
        }

        this.gameObject.transform.Find("card_image").gameObject.GetComponent<Renderer>().material.mainTexture = card_image;

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse entered.");
    }

    void OnMouseOver()
    {
        //Debug.Log("Mouse Over.");
    }

    void OnMouseDrag()
    {
        //Debug.Log("Mouse Drag.");
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse Exited.");
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse Down.");
    }

    public int Attack()
    {
        throw new NotImplementedException();
    } 

    public void Defend(int opponent_attack_amount)
    {
        throw new NotImplementedException();
    }

    public void AttachModifier(CardModifier modifier)
    {
        throw new NotImplementedException();
    }

    public void RemoveModifier(CardModifier modifier)
    {
        throw new NotImplementedException();
    }

    private void SetCardTextField(String text_obj_name, String text)
    {
        // Card text set
        TextMeshPro card_text_mesh = this.gameObject.transform.Find(text_obj_name).gameObject.GetComponent<TextMeshPro>();
        if (card_text_mesh  == null)
        {
            throw new System.Exception("Unable to find the TextMeshPro Compnent for the Card's " + text_obj_name + ".");
        }

        card_text_mesh.text = text;
    }
}
