using System.Collections.Generic;
using UnityEngine;

// NOTE(KASIN): The data will only get loaded from the start button on the menu screen

public class PlayerData 
{
    [SerializeField]
    private int player_damage_dealt = 0;
    [SerializeField]
    private int insects_defeated    = 0;
    [SerializeField]
    private int opponents_defeated  = 0;
    [SerializeField]
    private int nodes_traversed     = 0;
    [SerializeField]
    private List<string> _card_names  = new List<string>(); // Do not use. Only for serialization
    [SerializeField]
    private List<CardData> _card_data = new List<CardData>(); // Do not use. Only for serialization

    private Dictionary<string, CardData> cards_discovered =
        new Dictionary<string, CardData>();

    public class PlayerSaveData
    {
        public int player_damage_dealt;
        public int insects_defeated;
        public int opponents_defeated;
        public int nodes_traversed;
        public string[] card_names;
        public string[] cards_discovered;

        public PlayerSaveData(PlayerData data)
        {
            this.player_damage_dealt = data.player_damage_dealt;
            this.insects_defeated    = data.insects_defeated;
            this.opponents_defeated  = data.opponents_defeated;
            this.nodes_traversed     = data.nodes_traversed;

            this.card_names = data._card_names.ToArray();
            this.cards_discovered = new string[data._card_data.Count];
            for (int i = 0; i < this.cards_discovered.Length; ++i)
            {
                this.cards_discovered[i] = data._card_data[i].ToJSON();
            }
        }

        public static PlayerSaveData FromJson(string json)
        {
            return JsonUtility.FromJson<PlayerSaveData>(json);
        }
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(new PlayerSaveData(this), true);
    }

    public static PlayerData FromJson(string json)
    {
        PlayerSaveData save_data = JsonUtility.FromJson<PlayerSaveData>(json);

        PlayerData ret = new PlayerData();
        ret.player_damage_dealt = save_data.player_damage_dealt;
        ret.insects_defeated    = save_data.insects_defeated;
        ret.opponents_defeated  = save_data.opponents_defeated;
        ret.nodes_traversed     = save_data.nodes_traversed;

        for (int i = 0; i < save_data.card_names.Length; ++i)
        {
            ret.cards_discovered[save_data.card_names[i]] =
                CardData.FromJson(save_data.cards_discovered[i]);
        }

        return ret;
    }

    public void AddToDamageDealt(int amount)
    {
        this.player_damage_dealt += amount;
    }

    public int GetDamageDealt()
    {
        return this.player_damage_dealt;
    }

    public void AddToInsectsDefeated(int amount)
    {
        this.insects_defeated += amount;
    }

    public int GetInsectsDefeated()
    {
        return this.insects_defeated;
    }
    
    public void AddToOpponentsDefeated(int amount)
    {
        this.opponents_defeated += amount;
    }

    public int GetOpponentsDefeated()
    {
        return this.opponents_defeated;
    }

    public void AddToNodesTraversed(int amount)
    {
        this.nodes_traversed += amount;
    }

    public int GetNodesTraversed()
    {
        return this.nodes_traversed;
    }

    public Dictionary<string, CardData> GetCardsDiscovered()
    {
        // TODO(KASIN): Consider changing this to not be a reference
        return this.cards_discovered;
    }

    public void AddCardToDiscoveredList(CardData card)
    {
        if (card.card_name.Contains("Blessed") ||
            card.card_name.Contains("Evolved") ||
            card.card_name.Contains("+"))
        {
            return;
        }

        this.cards_discovered.TryAdd(card.card_name, card);
    }

    public void PackCardsDiscovered()
    {
        this.PackDictIntoLists(this._card_names, this._card_data,
            this.cards_discovered);
    }

    public int GetList()
    {
        return this._card_names.Count;
    }

    private void PackDictIntoLists<K,V>(List<K> keys, List<V> values,
        Dictionary<K,V> dict)
    {
        if (dict == null)
        {
            throw new System.Exception("PlayerStats :: dictionary was null. Unable to Pack to Dictionary");
        }
        if (keys == null || values == null)
        {
            throw new System.Exception("PlayerStats :: keys or values was null. Unable to Pack to Dictionary");
        }

        keys.Clear();
        values.Clear();

        foreach (K key in dict.Keys)
        {
            keys.Add(key);
        }

        foreach (V data in dict.Values)
        {
            values.Add(data);
        }
    }

    public void UnpackCardsDiscovered()
    {
        this.cards_discovered = this.UnpackListsIntoDict(this._card_names, this._card_data);
    }

    private Dictionary<K,V> UnpackListsIntoDict<K,V>(List<K> keys, List<V> values)
    {
        Dictionary<K,V> ret = new Dictionary<K,V>();

        if (keys == null || values == null)
        {
            throw new System.Exception("PlayerStats :: keys or values was null. Unable to unpack to Dictionary");
        }

        if (keys.Count != values.Count)
        {
            throw new System.Exception("PlayerStats :: Keys and values list counts are unequal.");
        }
        
        for (int i = 0; i < keys.Count; ++i)
        {
            ret.Add(keys[i], values[i]);
        }

        return ret;
    }
}

public class PlayerStats
{
    public static PlayerData player_data = new PlayerData();
    
    public static void Load()
    {
        player_data = SaveSystem._LoadPlayerStats();
        Debug.Log("Loaded Player Stats");
    }

    public static void Save()
    {
        if (player_data == null)
        {
            // Default data
            player_data = new PlayerData();
        }

        player_data.PackCardsDiscovered();
        SaveSystem._SavePlayerStats(player_data);
        Debug.Log("Saved Player Stats to file.");
    }
}