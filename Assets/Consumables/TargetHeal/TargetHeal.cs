using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/HealSingleCard")]
public class HealSingleCardConsumable : ScriptableObject, IConsumableSavable
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
    private class SaveData
    {
        public string consumable_type;
        public string item_name;
        public string image_resource_path;
        public int heal_amount;

        public SaveData(HealSingleCardConsumable consumable)
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
        HealSingleCardConsumable obj =
            ScriptableObject.CreateInstance<HealSingleCardConsumable>();
        obj.icon = Resources.Load<Sprite>("Images/" + save_data.image_resource_path);
        obj.itemName = save_data.item_name;
        obj.healAmount = save_data.heal_amount;
        consumable.consumable = obj;

        return consumable;
    }
}