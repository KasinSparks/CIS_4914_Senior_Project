using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Consumables/DamageAllOpponents")]
public class DamageAllOpponentsConsumable : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int damageAmount = 1; //can change for balancing
    public Playfield playfield;

    public void Use(AttackSystem attackSystem)
    {
        List<CardSlot> player_card_slots = playfield.GetCardSlots(CardOwnership.Opponent);
        for (int i = 0; i < player_card_slots.Count; i++)
        {
            Card enemyCard = player_card_slots[i].GetCard();
            if (enemyCard != null)
                enemyCard.DefendDirect(damageAmount);
        }
        Debug.Log("Damaged all cards");
    }
}
