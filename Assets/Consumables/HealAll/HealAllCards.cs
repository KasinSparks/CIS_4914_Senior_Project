using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Consumables/HealAllFriends")]
public class HealAllPlayerCardsConsumable : ScriptableObject, IConsumableSavable
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

    private class SaveData
    {
        public string consumable_type;
        public string item_name;
        public string image_resource_path;
        public int heal_amount;

        public SaveData(HealAllPlayerCardsConsumable consumable)
        {
            this.consumable_type = consumable.GetType().ToString();
            this.item_name = consumable.itemName;
            this.image_resource_path = consumable.icon.name;
            this.heal_amount = consumable.healAmount;
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
        HealAllPlayerCardsConsumable obj =
            ScriptableObject.CreateInstance<HealAllPlayerCardsConsumable>();
        obj.icon = Resources.Load<Sprite>("Images/" + save_data.image_resource_path);
        obj.itemName = save_data.item_name;
        obj.healAmount = save_data.heal_amount;
        consumable.consumable = obj;

        return consumable;
    }
}