using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Consumables/HealAllFriends")]
public class HealAllPlayerCardsConsumable : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int healAmount = 1;

    public void Use(Playfield playfield, AttackSystem attackSystem)
    {
        List<CardSlot> player_card_slots = playfield.GetCardSlots(CardOwnership.Player);
        for (int i = 0; i < player_card_slots.Count; i++)
        {
            Card playerCard = player_card_slots[i].GetCard();
            if (playerCard != null)
                playerCard.DefendDirect(-healAmount); //direct defending negative amount heals
        }
        Debug.Log("Healed all cards");
    }
}