using UnityEngine;
using UnityEngine.UI;

public class ConsumableButton : MonoBehaviour
{
    public Button button; //attach to button
    public Image overlayImage; //to display icon
    //List all consumables so they can be referenced
    public DamageAllOpponentsConsumable damageConsumable;
    public HealAllPlayerCardsConsumable healConsumable;
    public AddCardToHandConsumable cardConsumable;
    public HealSingleCardConsumable singleHealConsumable;
    private AttackSystem attackSystem;
    private GameState gameState;
    private Hand playerHand;
    private Playfield playfield;

    private void Awake()
    {
        gameState = FindFirstObjectByType<GameState>();
        attackSystem = FindFirstObjectByType<AttackSystem>();
        playerHand = FindFirstObjectByType<Hand>();
        playfield = FindFirstObjectByType<Playfield>();
        if (damageConsumable != null)
        {
            overlayImage.sprite = damageConsumable.icon;
            overlayImage.enabled = true;
        }
        else if (healConsumable != null)
        {
            overlayImage.sprite = healConsumable.icon;
            overlayImage.enabled = true;
        }
        else if (cardConsumable != null)
        {
            overlayImage.sprite = cardConsumable.icon;
            overlayImage.enabled = true;
        }
        else if (singleHealConsumable != null)
        {
            overlayImage.sprite = singleHealConsumable.icon;
            overlayImage.enabled = true;
        }
        else
        {
            overlayImage.enabled = false;
        }
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (gameState.current_turn_state != TurnStates.PlayerDrawCard && gameState.current_turn_state != TurnStates.PlayerTurn) //can only play on your turn
        {
            return;
        }
        //use consumable attached to button
        if (damageConsumable != null)
        {
            damageConsumable.Use(playfield, attackSystem); //uses attack system to effect health of cards
            damageConsumable = null;  //remove item
            overlayImage.enabled = false; //hide icon, this will not immediatly hide if it is a consumable that requires targeting
        }
        else if (healConsumable != null)
        {
            healConsumable.Use(playfield, attackSystem);
            healConsumable = null;
            overlayImage.enabled = false;
        }
        else if (cardConsumable != null)
        {
            cardConsumable.Use(playerHand);
            cardConsumable = null;
            overlayImage.enabled = false;
        }
        else if (singleHealConsumable != null)
        {
            playfield.ActivateConsumable(singleHealConsumable, this);
            singleHealConsumable = null;
            //overlayImage.enabled = false; cannot be disabled here since it needs to be disabled only after the item is actually used, not just clicked on
        }
    }

    //for demoing drag scriptable object into slot
    public void SetDamageConsumable(DamageAllOpponentsConsumable newConsumable)
    {
        damageConsumable = newConsumable;
        healConsumable = null;
        cardConsumable = null;
        singleHealConsumable = null;
        overlayImage.sprite = newConsumable.icon;
        overlayImage.enabled = true;
    }

    public void SetHealConsumable(HealAllPlayerCardsConsumable newConsumable)
    {
        healConsumable = newConsumable;
        damageConsumable = null;
        cardConsumable = null;
        singleHealConsumable = null;
        overlayImage.sprite = newConsumable.icon;
        overlayImage.enabled = true;
    }

    public void SetCardConsumable(AddCardToHandConsumable newConsumable)
    {
        cardConsumable = newConsumable;
        damageConsumable = null;
        healConsumable = null;
        singleHealConsumable = null;
        overlayImage.sprite = newConsumable.icon;
        overlayImage.enabled = true;
    }

    public void SetSingleHealConsumable(HealSingleCardConsumable newConsumable)
    {
        singleHealConsumable = newConsumable;
        damageConsumable = null;
        healConsumable = null;
        cardConsumable = null;
        overlayImage.sprite = newConsumable.icon;
        overlayImage.enabled = true;
    }

    public void SetConsumableToEmpty()
    {
        singleHealConsumable = null;
        damageConsumable = null;
        healConsumable = null;
        cardConsumable = null;
        this.HideOverlay();
    }

    public void HideOverlay() //for disabling icon in Playfield.cs
    {
        overlayImage.enabled = false;
    }

    /**
     * @brief Get the first consumable that is currently assigned
     * @return The consumable assigned, or null if no consumable is assigned
     */
    public ScriptableObject GetConsumableAssigned()
    {
        if (damageConsumable != null)
        {
            return damageConsumable;
        }
        else if (healConsumable != null)
        {
            return healConsumable;
        }
        else if (cardConsumable != null)
        {
            return cardConsumable;
        }
        else if (singleHealConsumable != null)
        {
            return singleHealConsumable;
        }

        return null;
    }
}
