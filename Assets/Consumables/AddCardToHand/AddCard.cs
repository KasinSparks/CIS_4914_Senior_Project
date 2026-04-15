using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/AddCardToHand")]
public class AddCardToHandConsumable : ScriptableObject, IConsumableSavable //in order to add any card, just instatiate a new scriptable object and drag in the desired card
{
    public string itemName;
    public Sprite icon;
    public CardData cardToAdd; //assign card, can be used for basic card like ant or flower card once added

    public void Use(Hand playerHand)
    {
        playerHand.AddCard(cardToAdd, CardOwnership.Player);
        Debug.Log("Added to hand: " + cardToAdd.card_name);
    }
    private class SaveData
    {
        public string consumable_type;
        public string item_name;
        public string image_resource_path;
        public string card_to_add;

        public SaveData(AddCardToHandConsumable consumable)
        {
            this.consumable_type = consumable.GetType().ToString();
            this.item_name = consumable.itemName;
            this.image_resource_path = consumable.icon.name;
            this.card_to_add = consumable.cardToAdd.ToJSON();
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
        AddCardToHandConsumable obj =
            ScriptableObject.CreateInstance<AddCardToHandConsumable>();
        obj.icon = Resources.Load<Sprite>("Images/" + save_data.image_resource_path);
        obj.itemName = save_data.item_name;
        obj.cardToAdd = CardData.FromJson(save_data.card_to_add);
        consumable.consumable = obj;

        return consumable;
    }
}