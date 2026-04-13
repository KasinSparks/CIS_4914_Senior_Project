using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Consumables/DamageAllOpponents")]
public class DamageAllOpponentsConsumable : ScriptableObject, IConsumableSavable
{
    public string itemName;
    public Sprite icon;
    public int damageAmount = 1; //can change for balancing

    public void Use(Playfield playfield, AttackSystem attackSystem)
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

    private class SaveData
    {
        public string consumable_type;
        public string item_name;
        public string image_resource_path;
        public int damage_amount;

        public SaveData(DamageAllOpponentsConsumable consumable)
        {
            this.consumable_type = consumable.GetType().ToString();
            this.item_name = consumable.itemName;
            this.image_resource_path = consumable.icon.name;
            this.damage_amount = consumable.damageAmount;
        }

        public static SaveData FromJson(string json)
        {
            return JsonUtility.FromJson<SaveData>(json);
        }
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(new SaveData(this), true);
    }

    public ConsumableHelper FromJson(string json)
    {
        SaveData save_data = SaveData.FromJson(json);
        
        ConsumableHelper consumable = new ConsumableHelper();
        consumable.consumable_type = save_data.consumable_type;
        DamageAllOpponentsConsumable obj =
            ScriptableObject.CreateInstance<DamageAllOpponentsConsumable>();
        obj.icon = Resources.Load<Sprite>("Images/" + save_data.image_resource_path);
        obj.itemName = save_data.item_name;
        obj.damageAmount = save_data.damage_amount;
        consumable.consumable = obj;

        return consumable;
    }
}
