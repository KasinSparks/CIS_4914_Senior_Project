using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/HealSingleCard")]
public class HealSingleCardConsumable : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int healAmount = 2;

    public void Use(Card target)
    {
        if (target == null) {
            return;
        }
        if (target.GetOwnership() != CardOwnership.Player) { //heal only players cards
            return;
        }
        target.DefendDirect(-healAmount);
        Debug.Log("Healed targeted card");
    }
}