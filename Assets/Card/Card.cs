using System;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/**
 *  @brief Used to store the modifier object and the gameobject that displays
 *  the information on the card.
 */
struct ModifierTuple
{
    public ModifierTuple(CardModifier mod, GameObject obj)
    {
        this.modifier = mod;
        this.modifer_image_prefab = obj;
    }

    public CardModifier modifier { get; }
    public GameObject   modifer_image_prefab { get; }
}

// NOTE(Kasin): See https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.IPointerDownHandler.html
//              for documentation regarding the Input EventSystem and PointerHandlers.

// NOTE(Kasin): Scene must have an EventSystem object, and the camera object must have
//              a Phisics RaycRaycaster entity attached to it for the Input system to work.
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public enum CardContext //needed for upgrades since gameplay loop needs gamestate which doesnt exist in upgrades
        {
            Gameplay,
            Upgrade,
            Creator
        }

    private CardContext context = CardContext.Gameplay; //default to gameplay


    [SerializeField]
    private CardData card_data;

    private List<ModifierTuple> modifiers;

    private int current_hp;

    // NOTE: Changes to these fields in the editor will only appear when the game is in a RUNNING state.
    public string card_name;
    public string description;

    public int hp;
    private int defense_bonus = 0;
    private float dodge_chance = 0.0f; // 0.0 to 1.0 inclusive.

    public int attack;
    private int attack_damage_bonus = 0;

    private int nektar_cost_amt_modifier;
    
    private int num_of_attacks_per_turn;

    // For debuging, this is set to public for now
    //private CardState card_state;
    public CardState card_state;

    private CardOwnership card_ownership;

    private GameState game_state;
    private GameObject hand_start_marker;

    private Hand player_hand;

    private bool is_in_card_creation_scene = false;

    private Transform modifier_start_mark;
    private const float MODIFIER_SPACE = 0.1f;

    private GameObject modifierInfoCanvas;
    private GameObject modifierInfoUIWidget;

    private CardSlot slot;
    private Playfield playfield_ref;

    public bool isUpgradeMode = false;
    private bool is_being_sacrificed;

    private void Awake()
    {
        this.num_of_attacks_per_turn = 1;
        this.nektar_cost_amt_modifier = 0;
        this.is_being_sacrificed = false;

        this.slot = null;
        // Initialize the card state to the default value
        this.card_state = CardState.None;

        // Initialize the card ownership to the default value
        this.card_ownership = CardOwnership.None;

        this.modifiers = new List<ModifierTuple>();

        this.num_of_attacks_per_turn = 1;

        Scene current_scene = SceneManager.GetActiveScene();
        if (current_scene != null &&
            (current_scene.name.Equals("CardCreator") || current_scene.name.Equals("ModifierCreator")))
        {
            this.is_in_card_creation_scene = true;
        }

        // TODO(KASIN): This can lead to attaching another copy of a modifier
        //     that is already attached. For now, a simple compare is used to
        //     prevent duplication of attached modifiers.
        this.modifier_start_mark = this.transform.Find("card_modifiers");
        for (int i = 0; i < this.modifiers.Count; ++i)
        {
            this.AttachModifier(this.modifiers[i].modifier);
        }


        // Got what is needed for the card creation scene, exit early
        if (this.is_in_card_creation_scene)
        {
            return;
        }
    }

    /**
     * @brief Get a GameObject with a given script attached to it.
     * @return The component.
     * @throws Exception If gameobject can not be found in the scene.
     */
    private T GetObject<T>(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null)
        {
            throw new Exception("Unable to find the GameObject: " + name);
        }

        return obj.GetComponent<T>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         if (context == CardContext.Gameplay) //so this doesnt initalize during upgrades, had to move from awake because order matters
        {
            this.game_state = this.GetObject<GameState>("GameState");
        }

        this.player_hand = this.GetObject<Hand>("Hand");

        this.playfield_ref = this.GetObject<Playfield>("Playfield");

        this.hand_start_marker = this.GetObject<GameObject>("start_mark");

        this.current_hp = this.card_data.hp;

        // TODO(KASIN): This can lead to attaching another copy of a modifier
        //     that is already attached. For now, a simple compare is used to
        //     prevent duplication of attached modifiers.
        this.modifier_start_mark = this.transform.Find("card_modifiers");
        for (int i = 0; i < this.card_data.starting_modifiers.Count; ++i)
        {
            this.AttachModifier(this.card_data.starting_modifiers[i]);
        }

        this.UpdateCardTextStats();

        // Card name text set
        this.SetCardTextField("card_name_text", this.card_data.card_name);

        // Card description text set
        this.SetCardTextField("card_description_text", this.card_data.description);

        this.UpdateCardTextStats();

        // Card Image set
        if (this.card_data.image == null)
        {
            throw new MissingComponentException("Unable to find image for " + this.card_data.image.name);
        }

        this.gameObject.transform.Find("card_image").gameObject.GetComponent<Renderer>().material.mainTexture = this.card_data.image;


        modifierInfoCanvas = GameObject.Find("UI_ModifierDisplay");
        modifierInfoUIWidget = GameObject.Find("UI_ModifierDisplay/UI_ModifierInfoRef");
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnsureModifierUI();
        // Have a UI Pop-up to show the modifier details
        List<ModifierTuple> mods = this.GetModifiers();
        for (int i = 0; i < mods.Count; ++i)
        {
            GameObject modifier_info_widget = Instantiate(this.modifierInfoUIWidget, modifierInfoCanvas.transform);
            UIModifierInfo modifier_info_widget_data = modifier_info_widget.GetComponent<UIModifierInfo>();
            modifier_info_widget_data.SetName(mods[i].modifier.name);
            modifier_info_widget_data.SetDescription(mods[i].modifier.GetDisplayDescription());
            modifier_info_widget_data.SetImage(mods[i].modifier.GetImage());

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
        EnsureModifierUI();
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
        //Debug.Log("Mouse Click.");
        if (context == CardContext.Upgrade)
        {
            PlayfieldUpgrade playfeildUpgrade = FindObjectOfType<PlayfieldUpgrade>();
            if (playfeildUpgrade != null)
            {
                playfeildUpgrade.selectedUpgradeCard = this;
            }

            ShopBehavior shop = FindObjectOfType<ShopBehavior>();
            if (shop != null)
            {
                shop.selectedUpgradeCard = this;
                shop.HighlightSlots();
            }

            return;
        }

        switch (this.card_state)
        {
	        case CardState.InHand:
                if (game_state.current_turn_state == TurnStates.PlayerSacrifice)
                {
                    // Make it so the player can not interrupt the sacrifice.
                    Debug.Log("You are currently placing a card that requires sacrifice. To cancel, click on the Cancel Sacrifice Button.");
                    return;
                }
        
                this.player_hand.SetSelectedCard(this.card_ownership, this);
                break;

            case CardState.OnPlayfield:
                //for consumables, check if there is a consumable waiting to be used
                Playfield playfield = GameObject.FindFirstObjectByType<Playfield>(); //if there is get the playfield and use consumable
                if (playfield != null && playfield.IsWaitingForTarget())
                {
                    playfield.ResolveTarget(this);
                }

                // CLEANUP(KASIN): There is a reference to the playfield here. We
                //     also have a member variable that is a reference to the
                //     playfield.

                // Check to see if the player is trying to sacrifice this card.
                if (game_state.current_turn_state == TurnStates.PlayerSacrifice &&
                    this.card_ownership == CardOwnership.Player &&
                    !this.is_being_sacrificed)
                {
                    // Add this card to the sacrifice list
                    this.playfield_ref.AddSacrificeCard(this);

                    this.SetToBeSacrificed(true);
                }

                break;

            case CardState.RequireSacrifice:

                break;
        }
    }

    // Don't call this externally... Call Attack() instead.
    public void _BaseAttack(Card opponent_card)
    {
        if (opponent_card == null)
        {
            // TODO: this is handled in the Attack function for now.
            return;
        }
        else
        {
            opponent_card.Defend(this, this.card_data.attack + this.attack_damage_bonus);
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
        // TODO(KASIN): If opponent_card is NULL, deal the damage to the opponent
        //    directly. If we want modifier(s) applied before attacking the
        //    player or opponent directly, then this logic needs to go into
        //    the baseAttack function.
        if (opponent_card == null)
        {
            switch (this.card_ownership)
            {
                case CardOwnership.Player:
                    Debug.Log("Attacked the Opponent directly!");
                    break;

                case CardOwnership.Opponent:
                    Debug.Log("Attacked the Player directly!");
                    break;
            }
            return;
        }

        // Apply attack modifiers
        // NOTE: if you need a certain modifier to apply before others, use a
        //     similar structure as this to call it before this. Similarly,
        //     if a modifier needs to be applied after, move it out and create a 
        //     similar pattern below.
        List<ModifierTuple> mods = this.GetModifiers(ModifierType.Attack);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, opponent_card);
                    break;

                default:
                    break;
            }
        }
        
        // Base Attack
        this._BaseAttack(opponent_card);

        // Update the Card UI elements
        this.UpdateCardTextStats();

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
            this.current_hp -= amt;
        }
    }

    // Applies the opponent attack damage with defense modifier(s) applied
    public void Defend(Card other, int opponent_attack_amount)
    {
        // Apply defense bonus modifiers
        List<ModifierTuple> mods = this.GetModifiers(ModifierType.Defense);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, null);
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
        else
        {
            Debug.Log("Card " + this.card_data.card_name + " dodged an attack!");
        }

        // Update the Card UI elements
        this.UpdateCardTextStats();

        // Check to see if the card has died
        if (this.current_hp <= 0)
        {
            this.Death(other);
        }
    }

    public void DefendDirect(int opponent_attack_amount)
    {
        this.DefendDirect(null, opponent_attack_amount);
    }

    public void DefendDirect(Card opponent_card, int opponent_attack_amount)
    {
        this.current_hp -= opponent_attack_amount;

        // Update the Card UI elements
        this.UpdateCardTextStats();

        // Check to see if the card has died
        if (this.current_hp <= 0)
        {
            this.Death(opponent_card);
        }
    }

    public void Placed()
    {
        // Apply modifiers that trigger on the initial placement of a card
        List<ModifierTuple> mods = this.GetModifiers(ModifierType.OnPlace);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }
    }

    public void Moved()
    {
        // Apply modifiers that trigger on the movement of a card
        List<ModifierTuple> mods = this.GetModifiers(ModifierType.OnMove);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }
    }
    
    // Warning: This destorys the GameObject. Do not try to call methods on 
    //    this card after calling this function, it will be NULL.
    private void Death(Card other)
    {
        // Apply modifiers that trigger on the death of a card
        List<ModifierTuple> mods = this.GetModifiers(ModifierType.OnDeath);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, other);
                    break;

                default:
                    break;
            }
        }

        // Destroy the card
        Destroy(this.gameObject);

        // Update card slot
        Debug.Log("SlotL " + this.slot);
        if (this.slot != null)
        {
            this.slot.ResetCardSlot();
        }

        // Playfield has changed
        playfield_ref.RegisterPlayfieldUpdate(this.card_ownership);
    }

    public void OnSacrifice()
    {
        // TODO(KASIN): Not sure if we want the OnDeath modifier to run when a
        //    card gets sacrificed.

        // Apply modifiers that trigger on the sacrifice of this card
        List<ModifierTuple> mods = this.GetModifiers(ModifierType.OnSacrificeNektarGiver);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }

        this.Death(null);
    }

    public void OnTurnStart()
    {
        // Update any modifiers with given states below
        List<ModifierTuple> mods = this.GetModifiers(ModifierState.Cooldown);
        for (int i = 0; i < mods.Count; ++i)
        {
            // TODO(KASIN): Update the cooldown count
        }

        mods.Clear();
        mods = this.GetModifiers(ModifierState.SetToReadyNextTurn);
        for (int i = 0; i < mods.Count; ++i)
        {
            mods[i].modifier.modifier_state = ModifierState.ReadyToApply;
        }


        mods.Clear();
        mods = this.GetModifiers(ModifierType.OnTurnStart);
        for (int i = 0; i < mods.Count; ++i)
        {
            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }

    }

    public void OnPlayfieldUpdate()
    {
        this.OnPlayfieldUpdate(0);
    }
    
    // TODO: Change int val to a struct so it can be added to later if needed
    public void OnPlayfieldUpdate(int val)
    {
        List<ModifierTuple> mods =
            this.GetModifiers(ModifierType.OnPlayfieldChange);
        
        // Apply the modifier
        for (int i = 0; i < mods.Count; ++i)
        {
            // TODO(KASIN): consider changing this
            if (mods[i].modifier.GetType() == typeof(StrengthInNumberModifier))
            {
                // TODO(KASIN): Yeah, not sure how I feel about this...
                ((StrengthInNumberModifier)mods[i].modifier).SetNumberOfHymenopteras(val);
            }

            // Check to see if this modifier has already been applied
            switch (mods[i].modifier.modifier_state)
            {
                case ModifierState.ReadyToApply:
                    mods[i].modifier.ApplyModifier(this, null);
                    break;

                default:
                    break;
            }
        }

        this.UpdateCardTextStats();
    }

    public void AttachModifier(CardModifier base_modifier)
    {
        // Do a simple compare to see if this modifier has already been attached
        List<ModifierTuple> temp_mods = this.GetModifiers();
        for (int i = 0; i < temp_mods.Count; ++i) {
            if (base_modifier.Compare(temp_mods[i].modifier))
            {
                return;
            }
        }

        CardModifier new_mod = Instantiate(base_modifier);
        new_mod.Initialize();

        // TODO(KASIN): Change this so the card modifier list only holds data.
        //    The modifier gameobject can be loaded and destoryed at runtime.
        // Do not modifier base_modifier. It is only here to be copied.
        // Alter the copied version (attached_mod).
        GameObject mod_prefab = Resources.Load<GameObject>("CardModifierPrefab");
        GameObject attached_mod = Instantiate(mod_prefab, modifier_start_mark);

        this.modifiers.Add(new ModifierTuple(new_mod, attached_mod));

        attached_mod.transform.Find("ModifierImage").GetComponent<Renderer>().material.mainTexture = new_mod.image;

        // TODO(KASIN): FIX this scaling
        //Vector3 parent_scale = this.transform.lossyScale;
        Vector3 parent_scale = this.transform.localScale;
        attached_mod.transform.localScale = new Vector3(
            0.07f,
            0.07f,
            0.07f
        );

        Vector3 start_mark_position = this.modifier_start_mark.position;
        attached_mod.transform.SetLocalPositionAndRotation(
            new Vector3(
                ((this.GetModifiers().Count - 1) * -0.02f),
                0.0f,
                0.0f
            ),
            Quaternion.identity
        );

        // If the modifier is a passive type, go ahead and apply it
        switch (base_modifier.GetModifierType())
        {
            case ModifierType.Passive:
                base_modifier.ApplyModifier(this, null);
                break;

            default:
                break;
        }

        this.UpdateCardTextStats();
    }


    // TODO(KASIN): I might change the GetModifiers functions to return an
    //     array type instead of a List.

    // Returns a list of references to the CardModifiers on this card.
    private List<ModifierTuple> GetModifiers()
    {
        return this.GetModifiers(ModifierType.All);
    }

    private List<ModifierTuple> GetModifiers(ModifierType modifierType)
    {
        return this.GetModifiers(new ModifierType[] { modifierType });
    }

    // TODO: Unit tests
    // Returns a list of references to the CardModifiers on this card with the
    // give ModifierType. ModifierTypes can be OR together to get a list containing
    // all ModifierTypes specified.
    private List<ModifierTuple> GetModifiers(ModifierType[] modifierType)
    {
        List<ModifierTuple> ret = new List<ModifierTuple>();

        for (int i = 0; i < this.modifiers.Count; ++i)
        {
            ModifierTuple mod_ref = this.modifiers[i];
            foreach (ModifierType mt in modifierType)
            {
                if (mt.Equals(ModifierType.All))
                {
                    // Add all the modifiers, regardless of type
                    ret.Add(mod_ref);
                }
                else if (mod_ref.modifier.GetModifierType().Equals(mt))
                {
                    // Add only the modifier that matches the type supplied
                    ret.Add(mod_ref);
                }
            }
        }

        return ret;
    }


    private List<ModifierTuple> GetModifiers(ModifierState modifierState)
    {
        return this.GetModifiers(new ModifierState[] { modifierState });
    }

    private List<ModifierTuple> GetModifiers(ModifierState[] modifierState)
    {
        List<ModifierTuple> ret = new List<ModifierTuple>();

        for (int i = 0; i < this.modifiers.Count; ++i)
        {
            ModifierTuple mod_ref = this.modifiers[i];
            foreach (ModifierState ms in modifierState)
            {
                if (mod_ref.modifier.GetModifierState().Equals(ms))
                {
                    // Add only the modifier that matches the type supplied
                    ret.Add(mod_ref);
                }
            }
        }

        return ret;
    }

    public List<CardModifier> GetAllModifierData() //for getting modifiers for sacrafice node
    {
        List<CardModifier> result = new List<CardModifier>();
        foreach (var tuple in this.GetModifiers())
        {
            result.Add(tuple.modifier);
        }
        return result;
    }
    
    // Use GetModifiers to get the reference of the modifier you wish to remove.
    public void RemoveModifier(CardModifier modifier)
    {
        // Unapply any modifiers that have been applied to this card
        List<ModifierTuple> mods = this.GetModifiers();
        for (int i = 0; i < mods.Count; ++i)
        {
            if (mods[i].modifier == modifier)
            {
                if (mods[i].modifier.modifier_state.Equals(ModifierState.Applied))
                {
                    mods[i].modifier.UnapplyModifier(this, null);
                }

                // Get rid of the gameobject in Unity
                if (mods[i].modifer_image_prefab != null)
                {
                    Destroy(mods[i].modifer_image_prefab);
                }

                // Remove it from the list
                this.modifiers.Remove(mods[i]);
            }
        }
    }

    public void SetState(CardState state)
    {
        this.card_state = state;
    }

    private void SetCardTextField(String text_obj_name, String text)
    {
        // Card text set
        TextMeshPro card_text_mesh = this.gameObject.transform.Find(text_obj_name).gameObject.GetComponent<TextMeshPro>();
        if (card_text_mesh == null)
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
            (this.current_hp).ToString());

        // Card Attack Text set
        this.SetCardTextField("card_attack_text",
            (this.card_data.attack + this.attack_damage_bonus).ToString());

        // Card Cost Text set
        this.SetCardTextField("card_cost_text", this.card_data.nektar_cost.ToString());
    }

    public CardOwnership GetOwnership()
    {
        return this.card_ownership;
    }

    public void SetOwnership(CardOwnership owner)
    {
        this.card_ownership = owner;
    }

    public void SetSlot(CardSlot slot)
    {
        this.slot = slot;
    }

    public CardOrder GetOrder()
    {
        return this.card_data.order;
    }

    public int GetBaseAttack()
    {
        return this.card_data.attack;
    }

    public int GetCurrentHP()
    {
        return this.current_hp;
    }

    public void SetCurrentHP(int hp)
    {
        this.current_hp = hp;
    }

    public string GetCardName()
    {
        return this.card_data.card_name;
    }

    public void SetCardData(CardData data)
    {
        if (data != null)
        {
            this.card_data = Instantiate(data);
        }
        this.current_hp = this.card_data.hp;
    }

    public CardData GetCardData() //for upgrading cards
    {
        return this.card_data;
    }

    public void Initialize(CardData data) //initalize to display new card when upgrading
    {
        card_name = data.card_name;
        attack = data.attack;
        hp = data.hp;
        this.modifiers = new List<ModifierTuple>();
        foreach (CardModifier mod in data.starting_modifiers)
        {
            this.AttachModifier(mod);
        }
        this.SetCardTextField("card_name_text", data.card_name); //update name
        this.SetCardTextField("card_description_text", data.description);
        if (data.image != null)
        {
            this.transform.Find("card_image")
                .GetComponent<Renderer>()
                .material.mainTexture = data.image;
        }
        this.UpdateCardTextStats(); //display new stats
    }

    private void EnsureModifierUI() //this is because when creating cards in shop, they did not have the modifiers appear, so i need to make sure the object works
    {
        if (modifierInfoCanvas == null)
            modifierInfoCanvas = GameObject.Find("UI_ModifierDisplay");

        if (modifierInfoUIWidget == null)
            modifierInfoUIWidget = GameObject.Find("UI_ModifierDisplay/UI_ModifierInfoRef");
    }

    public void SetContext(CardContext ctx) //for setting context
    {
        context = ctx;
    }

    public int GetNektarCost()
    {
        return this.card_data.nektar_cost + this.nektar_cost_amt_modifier;
    }

    public void SetToBeSacrificed(bool status)
    {
        // Update the image
        Transform card_sacrifice_image = this.transform.Find("card_sacrifice_image");
        card_sacrifice_image.gameObject.SetActive(status);
        this.is_being_sacrificed = status;
    }
}
