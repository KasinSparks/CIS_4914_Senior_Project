using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/HealAllFriends")]
public class HealAllPlayerCardsConsumable : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int healAmount = 1;

    public void Use(AttackSystem attackSystem)
    {
        for (int i = 0; i < attackSystem.columns.Length; i++)
        {
            Card playerCard = attackSystem.columns[i].player_cardholder.GetCard();
            if (playerCard != null)
                playerCard.DefendDirect(-healAmount); //direct defending negative amount heals
        }
        Debug.Log($"Healed all cards");
    }
}