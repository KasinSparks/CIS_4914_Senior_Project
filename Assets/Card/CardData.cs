using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Card")]
public class CardData : ScriptableObject, ISavable
{
    public string card_name;
    public string description;

    public WordInfo[] highlighted_words;

    public CardOrder order;

    public int hp;
    public int attack;
    public int nektar_cost;

    public Texture image;

    public CardRarity card_rarity;

    // NOTE: This list is only used to add modifiers in the editor. If you need to get
    //       modifiers on this card during game runtime, use the GetModifiers function.
    public List<CardModifier> starting_modifiers;
    
    /**
     * @brief A very simple comparison. Compares name and order to determine
     * equality.
     * @param The other card data.
     * @return If the card is the same as the other.
     */
    public bool Compare(CardData other)
    {
        if (other == null) return false;

        if ((this.card_name == other.card_name) && (this.order == other.order)) return true;

        return false;
    }

    public JsonValue ToJsonObject()
    {
        JsonObject json_object = new JsonObject();
        json_object["Data"] = new JsonObject();
        json_object["Type"] = new JsonString() { value = this.GetType().ToString() };

        JsonObject json_data = (JsonObject)json_object["Data"];
        json_data.value.Add("card_name", new JsonString() { value = this.card_name });
        json_data.value.Add("description", new JsonString() { value = this.description });
        json_data.value.Add("order", new JsonInt() { value = (int)this.order});
        json_data.value.Add("card_rarity", new JsonInt() { value = (int)this.card_rarity});
        json_data.value.Add("hp", new JsonInt() { value = this.hp});
        json_data.value.Add("attack", new JsonInt() { value = this.attack});
        json_data.value.Add("nektar_cost", new JsonInt() { value = this.nektar_cost});

        JsonArray json_highlighted_words = new JsonArray(); 
        foreach (WordInfo word_info in this.highlighted_words)
        {
            System.Guid guid = SaveSystemTable.FindGuid(word_info.GetInstanceID());
            if (guid.Equals(System.Guid.Empty))
            {
                // Add the word info to the save table and take the guid to store here
                guid = SaveSystemTable.Add(word_info, word_info.GetInstanceID());
            }

            json_highlighted_words.value.Add(new JsonString() { value = guid.ToString() });
        }
        json_data.value.Add("highlighed_words", json_highlighted_words);

        JsonArray json_starting_modifiers = new JsonArray(); 
        foreach (CardModifier card_mod in this.starting_modifiers)
        {
            System.Guid guid = SaveSystemTable.FindGuid(card_mod.GetInstanceID());
            if (guid.Equals(System.Guid.Empty))
            {
                // Add the word info to the save table and take the guid to store here
                guid = SaveSystemTable.Add(card_mod, card_mod.GetInstanceID());
            }

            json_starting_modifiers.value.Add(new JsonString() { value = guid.ToString() });
        }
        json_data.value.Add("starting_modifiers", json_starting_modifiers);

        return json_object;
    }

    public void OverrideValuesFromJson(JsonValue json)
    {
        throw new System.NotImplementedException();
    }

}