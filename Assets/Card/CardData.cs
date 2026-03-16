using UnityEngine;
using System.Collections.Generic;
using System;

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

    public int nektar_given_when_scarificed;

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

        if ((this.card_name.Equals(other.card_name)) && (this.order == other.order)) return true;

        return false;
    }
    
    // TODO(KASIN): Change these hardcoded strings to Constant variables.
    //    If a string in the ToJsonObject doesn't match the string in the
    //    OverrideValuesFromJson, which is easy to do, errors happen.
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
        json_data.value.Add("highlighted_words", json_highlighted_words);

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

        json_data.value.Add("image",
            SaveSystemTable.GetJsonForTexture2D((Texture2D)this.image));

        return json_object;
    }

    public void OverrideValuesFromJson(JsonValue json)
    {
        JsonObject json_data = (JsonObject)json;

        this.card_name   = ((JsonString)(json_data["card_name"])).value;
        this.description = ((JsonString)(json_data["description"])).value;

        this.hp          = ((JsonInt)(json_data["hp"])).value;
        this.attack      = ((JsonInt)(json_data["attack"])).value;
        this.nektar_cost = ((JsonInt)(json_data["nektar_cost"])).value;

        this.order = (CardOrder) ((JsonInt)(json_data["order"])).value;
        this.card_rarity = (CardRarity) ((JsonInt)(json_data["card_rarity"])).value;
        
        JsonArray saved_words = (JsonArray)json_data["highlighted_words"];
        int num_of_saved_words = saved_words.value.Count;
        List<WordInfo> temp_words = new List<WordInfo>(num_of_saved_words);
        for (int i = 0; i < num_of_saved_words; ++i)
        {
            Guid guid = Guid.Parse(((JsonString)saved_words[i]).value);
            WordInfo retrieved_word = SaveSystemTable.Get<WordInfo>(guid);
            temp_words.Add(retrieved_word); 
        }
        this.highlighted_words = temp_words.ToArray();


        JsonArray saved_mods = (JsonArray)json_data["starting_modifiers"];
        int num_of_saved_mods = saved_mods.value.Count;
        this.starting_modifiers = new List<CardModifier>(num_of_saved_mods);
        for (int i = 0; i < num_of_saved_mods; ++i)
        {
            Guid guid = Guid.Parse(((JsonString)saved_mods[i]).value);
            CardModifier retrieved_mod = SaveSystemTable.Get<CardModifier>(guid);
            this.starting_modifiers.Add(retrieved_mod); 
        }

        JsonObject image_data = (JsonObject)json_data["image"];
        this.image = SaveSystemTable.GetTexture2DFromJsonImage(image_data);
    }

}