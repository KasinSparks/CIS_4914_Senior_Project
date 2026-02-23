using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/DamageAllOpponents")]
public class DamageAllOpponentsConsumable : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int damageAmount = 1; //can change for balancing

    public void Use(AttackSystem attackSystem)
    {
        for (int i = 0; i < attackSystem.current_opponent_cards.Length; i++)
        {
            Card enemyCard = attackSystem.current_opponent_cards[i];
            if (enemyCard != null)
                enemyCard.DefendDirect(damageAmount);
        }
        Debug.Log("Damaged all cards");
    }
}
