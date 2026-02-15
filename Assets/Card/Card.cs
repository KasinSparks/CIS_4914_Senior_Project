using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private int defense_bonus = 0;
    private float dodge_chance = 0.0f; // 0.0 to 1.0 inclusive.

    public int attack;
    private int attack_damage_bonus = 0;
    public int play_cost;

    private int num_of_attacks_per_turn = 1;

    // NOTE: This list is only used to add modifiers in the editor. If you need to get
    //       modifiers on this card during game runtime, use the GetModifiers function.
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

    private Hand player_hand;

    private bool is_in_card_creation_scene = false;

    private Transform modifier_start_mark;
    private const float MODIFIER_SPACE = 0.05f;

    private GameObject modifierInfoCanvas;
    private GameObject modifierInfoUIWidget;


    private void Awake()
    {
        // Initialize the card state to the default value
        this.card_state = CardState.None;

        // Initialize the card ownership to the default value
        this.card_ownership = CardOwnership.None;

        Scene current_scene = SceneManager.GetActiveScene();
        if (current_scene != null &&
            (current_scene.name.Equals("CardCreator") || current_scene.name.Equals("ModifierCreator")))
        {
            this.is_in_card_creation_scene = true;
        }

        this.modifier_start_mark = this.transform.Find("card_modifiers");
        for (int i = 0; i < this.modifiers.Count; ++i)
        {
            this.AttachModifier(this.modifiers[i]);
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Card name text set
        this.SetCardTextField("card_name_text", this.card_name);

        // Card description text set
        this.SetCardTextField("card_description_text", this.description);

        this.UpdateCardTextStats();

        // Card Image set
        if (this.card_image == null)
        {
            throw new MissingComponentException("Unable to find image for " + this.card_image.name);
        }

        this.gameObject.transform.Find("card_image").gameObject.GetComponent<Renderer>().material.mainTexture = card_image;


        modifierInfoCanvas = GameObject.Find("UI_ModifierDisplay");
        modifierInfoUIWidget   = GameObject.Find("UI_ModifierDisplay/UI_ModifierInfoRef");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Have a UI Pop-up to show the modifier details
        List<CardModifier> mods = this.GetModifiers();
        for (int i = 0; i < mods.Count; ++i)
        {
            GameObject modifier_info_widget = Instantiate(this.modifierInfoUIWidget, modifierInfoCanvas.transform);
            UIModifierInfo modifier_info_widget_data = modifier_info_widget.GetComponent<UIModifierInfo>();
            modifier_info_widget_data.SetName(mods[i].name);
            modifier_info_widget_data.SetDescription(mods[i].GetDisplayDescription());
            modifier_info_widget_data.SetImage(mods[i].image);

            Vector2 widget_size = modifier_info_widget_data.GetRectSize();
            modifier_info_widget.transform.SetPositionAndRotation(
                new Vector3(
                    modifierInfoUIWidget.transform.position.x,
                    modifierInfoUIWidget.transform.position.y
                        - (widget_size.y * (i + 1) + (16 * (i + 1))),
                    modifierInfoUIWidget.transform.position.z
                ),
                modifierInfoUIWidget.transform.rotation
            );
        }

        if (this.card_state == CardState.InHand)
        {
            // Move the card in ontop of the other cards in the hand
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
        // Remove the UI pop-ups for the modifiers
        for (int i = 0; i < this.modifierInfoCanvas.transform.childCount; ++i)
        {
            Transform obj = this.modifierInfoCanvas.transform.GetChild(i);
            if (!obj.name.Equals("UI_ModifierInfoRef"))
            {
                Destroy(obj.gameObject);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse Click.");
    }
    
    // Don't call this externally... Call Attack() instead.
    public void _BaseAttack(Card opponent_card)
    {
        // TODO(KASIN): If opponent_card is NULL, deal the damage to the opponent directly
        if (opponent_card == null)
        {
            // TODO
        } else
        {
            opponent_card.Defend(this.attack + this.attack_damage_bonus);
        }
    }

    // To be used by the AttackDamage Modifier(s)
    public void _AddAttackBonusDamage(int amount)
    {
        this.attack_damage_bonus += amount;
    }
    public void _RemoveAttackBonusDamage(int amount)
    {
        this.attack_damage_bonus -= amount;
    }

    // Deals the attack damage with attack modifier(s) applied to the opponent
    // card given. If opponent_card is NULL, deal the damage to the opponent
    // directly.
    public void Attack(Card opponent_card)
    {
        // Apply attack modifiers
        // NOTE: if you need a certain modifier to apply before others, use a
        //     similar structure as this to call it before this. Similarly,
        //     if a modifier needs to be applied after, move it out and create a 
        //     similar pattern below.
        List<CardModifier> mods = this.GetModifiers(ModifierType.Attack);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].ApplyModifier(this, opponent_card);
                    break;

                default:
                    break;
            }
        }

        for (int i = 0; i < this.num_of_attacks_per_turn; ++i)
        {
            // Base Attack
            this._BaseAttack(opponent_card);
        }

        // TODO(KASIN): Move this to the attack system and handle it there
        /*
        // Check to see if the card needs to attack the opposing left and right
        // cards.
        mods.Clear();
        mods = this.GetModifiers(ModifierType.AttacksNeighbors);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].ApplyModifier(this);
                    
                    // Attack left and right opposing cards
                    // TODO(KASIN):

                    // Reset the applied state for next turn
                    mods[i].modifier_state = ModifierState.ReadyToApply;
                    break;

                default:
                    break;
            }
        }
        */
    }
    
    // To be used by the Defense Modifier(s)
    public void _AddDefenseBonus(int amount)
    {
        this.defense_bonus += amount;
    }
    public void _RemoveDefenseBonus(int amount)
    {
        this.defense_bonus -= amount;
    }

    // Don't call this externally... Use Defend() instead.
    public void _BaseDefend(int opponent_attack_amount)
    {
        int amt = opponent_attack_amount - this.defense_bonus;
        if (amt > 0)
        {
            this.hp -= amt;
        }
    }

    // Applies the opponent attack damage with defense modifier(s) applied
    public void Defend(int opponent_attack_amount)
    {
        // Apply defense bonus modifiers
        List<CardModifier> mods = this.GetModifiers(ModifierType.Defense);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }

        // Handle the incoming attack damage
        // Check to see if dodge is succussful
        if (UnityEngine.Random.Range(0.0f, 1.0f) > this.dodge_chance)
        {
            // Dodge was not successful
            this._BaseDefend(opponent_attack_amount);
        }

        // Update the Card UI elements
        this.UpdateCardTextStats();

        // Check to see if the card has died
        if (this.hp <= 0)
        {
            this.Death();
        }
    }

    public void DefendDirect(int opponent_attack_amount)
    {
        this.hp -= opponent_attack_amount;

        // Update the Card UI elements
        this.UpdateCardTextStats();

        // Check to see if the card has died
        if (this.hp <= 0)
        {
            this.Death();
        }
    }

    public void Placed()
    {
        // Apply modifiers that trigger on the initial placement of a card
        List<CardModifier> mods = this.GetModifiers(ModifierType.OnPlace);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }
    }

    public void Moved()
    {
        // Apply modifiers that trigger on the movement of a card
        List<CardModifier> mods = this.GetModifiers(ModifierType.OnMove);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }
    }
    
    // Warning: This destorys the GameObject. Do not try to call methods on 
    //    this card after calling this function, it will be NULL.
    private void Death()
    {
        // Apply modifiers that trigger on the death of a card
        List<CardModifier> mods = this.GetModifiers(ModifierType.OnDeath);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }

        // Destroy the card
        Destroy(this.gameObject);
    }

    public void OnTurnStart()
    {
        // Update any modifiers with given states below
        List<CardModifier> mods = this.GetModifiers(ModifierState.Cooldown);
        for (int i = 0; i < mods.Count; ++i)
        {
            // TODO(KASIN): Update the cooldown count
        }

        mods.Clear();
        mods = this.GetModifiers(ModifierState.SetToReadyNextTurn);
        for (int i = 0; i < mods.Count; ++i)
        {
            mods[i].modifier_state = ModifierState.ReadyToApply;
        }


        mods.Clear();
        mods = this.GetModifiers(ModifierType.OnTurnStart);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }

    }

    public void AttachModifier(CardModifier base_modifier)
    {
        // Do not modifier base_modifier. It is only here to be copied.
        // Alter the copied version (attached_mod).
        CardModifier attached_mod = Instantiate(base_modifier, modifier_start_mark);
        // TODO(KASIN): FIX this scaling
        //Vector3 parent_scale = this.transform.lossyScale;
        Vector3 parent_scale = this.transform.localScale;
        attached_mod.transform.localScale = new Vector3(
            (1.0f / parent_scale.x) / 10.0f,
            (1.0f / parent_scale.y) / 10.0f,
            (1.0f / parent_scale.z) / 10.0f 
        );
        /*
        attached_mod.transform.localScale = new Vector3(
            parent_scale.x * attached_mod.transform.localScale.x,
            parent_scale.z * attached_mod.transform.localScale.y,
            parent_scale.y * attached_mod.transform.localScale.z 
        );
        */

        Vector3 start_mark_position = this.modifier_start_mark.position;
        attached_mod.transform.SetPositionAndRotation(
            new Vector3(
                start_mark_position.x + 
                ((attached_mod.transform.Find("ModifierImage").GetComponent<MeshRenderer>().bounds.size.y
                    + MODIFIER_SPACE) * (this.GetModifiers().Count - 1)),
                start_mark_position.y,
                start_mark_position.z
            ),
            this.modifier_start_mark.rotation
        );

        // If the modifier is a passive type, go ahead and apply it
        switch (attached_mod.modifier_type)
        {
            case ModifierType.Passive:
                attached_mod.ApplyModifier(this, null);
                break;

            default:
                break;
        }

        this.UpdateCardTextStats();
    }


    // TODO(KASIN): I might change the GetModifiers functions to return an
    //     array type instead of a List.

    // Returns a list of references to the CardModifiers on this card.
    public List<CardModifier> GetModifiers()
    {
        return this.GetModifiers(ModifierType.All);
    }

    public List<CardModifier> GetModifiers(ModifierType modifierType)
    {
        return this.GetModifiers(new ModifierType[] {modifierType});
    }

    // TODO: Unit tests
    // Returns a list of references to the CardModifiers on this card with the
    // give ModifierType. ModifierTypes can be OR together to get a list containing
    // all ModifierTypes specified.
    public List<CardModifier> GetModifiers(ModifierType[] modifierType)
    {
        List<CardModifier> ret = new List<CardModifier>();

        Transform card_modifiers_parent = this.transform.Find("card_modifiers");
        for (int i = 0; i < card_modifiers_parent.childCount; ++i)
        {
            CardModifier card_mod = card_modifiers_parent.GetChild(i).gameObject.GetComponent<CardModifier>();
            foreach (ModifierType mt in modifierType)
            {
                if (mt.Equals(ModifierType.All))
                {
                    // Add all the modifiers, regardless of type
                    ret.Add(card_mod);
                }
                else if (card_mod.modifier_type.Equals(mt))
                {
                    // Add only the modifier that matches the type supplied
                    ret.Add(card_mod);
                }
            }
        }

        return ret;
    }


    public List<CardModifier> GetModifiers(ModifierState modifierState)
    {
        return this.GetModifiers(new ModifierState[] {modifierState});
    }

    public List<CardModifier> GetModifiers(ModifierState[] modifierState)
    {
        List<CardModifier> ret = new List<CardModifier>();

        Transform card_modifiers_parent = this.transform.Find("card_modifiers");
        for (int i = 0; i < card_modifiers_parent.childCount; ++i)
        {
            CardModifier card_mod = card_modifiers_parent.GetChild(i).gameObject.GetComponent<CardModifier>();
            foreach (ModifierState ms in modifierState)
            {
                if (card_mod.modifier_state.Equals(ms))
                {
                    // Add only the modifier that matches the state supplied
                    ret.Add(card_mod);
                }
            }
        }

        return ret;
    }
    
    // TODO(KASIN): This method has not been tested.
    // Use GetModifiers to get the reference of the modifier you wish to remove.
    public void RemoveModifier(CardModifier modifier)
    {
        // Unapply any modifiers that have been applied to this card
        List<CardModifier> mods = this.GetModifiers();
        for (int i = 0; i < mods.Count; ++i)
        {
            if (mods[i].modifier_state.Equals(ModifierState.Applied))
            {
                mods[i].UnapplyModifier(this, null);
            }
        }
        
        // Get rid of the gameobject in Unity
        Destroy(modifier.gameObject);
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

    public void _SetNumAdditionalAttacks(int num)
    {
        this.num_of_attacks_per_turn = num;
    }

    public int _GetNumAdditionalAttacks()
    {
        return this.num_of_attacks_per_turn;
    }

    public float _GetDodgeChance()
    {
        return this.dodge_chance;
    }

    public void _AddDodgeChance(float amt)
    {
        this.dodge_chance += amt;
    }
    public void _RemoveDodgeChance(float amt)
    {
        this.dodge_chance -= amt;
    }

    private void UpdateCardTextStats()
    {
        // Card HP Text set
        this.SetCardTextField("card_health_text",
            (this.hp + this.defense_bonus).ToString());

        // Card Attack Text set
        this.SetCardTextField("card_attack_text",
            (this.attack + this.attack_damage_bonus).ToString());

        // Card Cost Text set
        this.SetCardTextField("card_cost_text", this.play_cost.ToString());
    }

    public CardOwnership GetOwnership()
    {
        return this.card_ownership;
    }

    public void SetOwnership(CardOwnership owner)
    {
        this.card_ownership = owner;
    }
}
