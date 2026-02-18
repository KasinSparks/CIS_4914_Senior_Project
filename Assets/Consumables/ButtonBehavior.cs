using UnityEngine;
using UnityEngine.UI;

public class ConsumableButton : MonoBehaviour
{
    public Button button; //attach to button
    public Image overlayImage; //to display icon
    //List all consumables so they can be referenced
    public DamageAllOpponentsConsumable damageConsumable;
    public HealAllPlayerCardsConsumable healConsumable;
    private AttackSystem attackSystem;
    private GameState gameState;

    private void Awake()
    {
        gameState = FindFirstObjectByType<GameState>();
        attackSystem = FindFirstObjectByType<AttackSystem>();
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
            damageConsumable.Use(attackSystem); //uses attack system to effect health of cards
            damageConsumable = null;  //remove item
        }
        else if (healConsumable != null)
        {
            healConsumable.Use(attackSystem);
            healConsumable = null;
        }
        overlayImage.enabled = false; //hide icon
    }

    //for demoing drag scriptable object into slot
    public void SetDamageConsumable(DamageAllOpponentsConsumable newConsumable)
    {
        damageConsumable = newConsumable;
        healConsumable = null; //naive check to make sure no two items can be in same slot
        overlayImage.sprite = newConsumable.icon;
        overlayImage.enabled = true;
    }

    public void SetHealConsumable(HealAllPlayerCardsConsumable newConsumable)
    {
        healConsumable = newConsumable;
        damageConsumable = null;
        overlayImage.sprite = newConsumable.icon;
        overlayImage.enabled = true;
    }
}
