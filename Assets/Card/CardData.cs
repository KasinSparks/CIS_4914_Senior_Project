using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Card/Card")]
public class CardData : ScriptableObject
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

    public AudioClip attack_audio;
    
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

    public class CardDataSaveData
    {
        public string card_name;
        public string description;

        public CardRarity card_rarity;
        public CardOrder  order;

        public int hp;
        public int attack;
        public int nektar_cost;
        public int nektar_given_when_scarificed;

        public string image_resource_path;
        public string attack_audio_resource_path;

        public string[] highlighted_words;

        public string[] starting_modifiers;

        public CardDataSaveData(CardData data)
        {
            this.card_name   = data.card_name;
            this.description = data.description;
            
            this.card_rarity = data.card_rarity;
            this.order       = data.order;

            this.hp          = data.hp;
            this.attack      = data.attack;
            this.nektar_cost = data.nektar_cost;
            this.nektar_given_when_scarificed = data.nektar_given_when_scarificed;

            this.image_resource_path = data.image.name;

            if (data.attack_audio != null)
            {
                this.attack_audio_resource_path = data.attack_audio.name;
            }

            this.highlighted_words = new string[data.highlighted_words.Length];
            for (int i = 0; i < highlighted_words.Length; ++i)
            {
                this.highlighted_words[i] = data.highlighted_words[i].ToJson();
            }

            this.starting_modifiers = new string[data.starting_modifiers.Count];
            for (int i = 0; i < starting_modifiers.Length; ++i)
            {
                this.starting_modifiers[i] = data.starting_modifiers[i].ToJson();
            }
        }

        public static CardDataSaveData FromJson(string json)
        {
            return JsonUtility.FromJson<CardDataSaveData>(json);
        }
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(new CardDataSaveData(this), true);
    }

    public static CardData FromJson(string json)
    {
        CardData ret = ScriptableObject.CreateInstance<CardData>();

        CardDataSaveData raw_save_data = CardDataSaveData.FromJson(json);

        ret.card_name   = raw_save_data.card_name;
        ret.description = raw_save_data.description;

        ret.card_rarity = raw_save_data.card_rarity;
        ret.order       = raw_save_data.order;

        ret.hp          = raw_save_data.hp;
        ret.attack      = raw_save_data.attack;
        ret.nektar_cost = raw_save_data.nektar_cost;
        ret.nektar_given_when_scarificed = raw_save_data.nektar_given_when_scarificed;

        ret.image = Resources.Load<Texture>("Images/" + raw_save_data.image_resource_path);
        if (raw_save_data.attack_audio_resource_path != null &&
            raw_save_data.attack_audio_resource_path != "")
        {
            ret.attack_audio = Resources.Load<AudioClip>("Sound Effects/" + raw_save_data.attack_audio_resource_path);
        }

        ret.highlighted_words = new WordInfo[raw_save_data.highlighted_words.Length];
        for (int i = 0; i < ret.highlighted_words.Length; ++i)
        {
            ret.highlighted_words[i] = WordInfo.FromJson(raw_save_data.highlighted_words[i]);
        }

        ret.starting_modifiers = new List<CardModifier>();
        for (int i = 0; i < raw_save_data.starting_modifiers.Length; ++i)
        {
            ret.starting_modifiers.Add(CardModifier.FromJson(raw_save_data.starting_modifiers[i]));
        }

        foreach (CardModifier modifier in ret.starting_modifiers)
        {
            if (modifier.defered_for_spawn_card)
            {
                switch (modifier.GetType().Name)
                {
                    case "QueenModifier":
                        ((QueenModifier)modifier).spawn_card = ret;
                        break;
                    default:
                        throw new System.Exception("Should not have a defered for spawn card on a non-queen modifier.");
                }
            }
        }

        return ret;
    }
}