using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// NOTE(Kasin): See https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.IPointerDownHandler.html
//              for documentation regarding the Input EventSystem and PointerHandlers.

// NOTE(Kasin): Scene must have an EventSystem object, and the camera object must have
//              a Phisics Raycaster entity attached to it for the Input system to work.
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
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
    
    // For debuging, this is set to public for now
    //private CardState card_state;
    public CardState card_state;

    private CardOwnership card_ownership;

    private GameState game_state;
    private GameObject hand_start_marker;
    private float card_hover_hand_z_offset;

    private float card_original_z_hand_offset;

    private Hand player_hand;

    private bool is_in_card_creation_scene = false;

    private void Awake()
    {
        // Initialize the card state to the default value
        this.card_state = CardState.None;

        // Initialize the card ownership to the default value
        this.card_ownership = CardOwnership.None;

        Scene current_scene = SceneManager.GetActiveScene();
        if (current_scene != null && current_scene.name.Equals("CardCreator"))
        {
            this.is_in_card_creation_scene = true;
        }

        // Got what is needed for the card creation scene, exit early
        if (this.is_in_card_creation_scene)
        {
            return;
        }

        // TODO: error handling
        this.game_state = GameObject.Find("GameState").GetComponent<GameState>();

        // TODO: error handling
        this.player_hand = GameObject.Find("Hand").GetComponent<Hand>();

        // TODO: error handling
        this.hand_start_marker = GameObject.Find("start_mark");
        card_hover_hand_z_offset = this.hand_start_marker.transform.position.z - 0.1f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Card Creation Scene does not require Mouse events; exit early
        if (this.is_in_card_creation_scene)
        {
            return;
        }
        //Debug.Log("Mouse entered.");

        if (this.card_state == CardState.InHand)
        {
            // Tell the hand to handle this
            this.player_hand.MoveCardFront(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Mouse Drag.");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Mouse Drag.");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("Mouse Drag.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse Exited.");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Mouse Click.");
        this.player_hand.SetSelectedCard(this);
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

    public void SetState(CardState state)
    {
        this.card_state = state;
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
