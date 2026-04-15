using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public enum SaveSystemFile
{
    None,
    PlayerDeck,
    PlayerConsumables,
    TotemModifiers,
    TotemModifierNames,
    TotemOrders,
    OpponentDeck1,  // Example
    OpponentDeck2,  // Example, can change this to the name of the opponent
    WordInfo,
    PlayerStats,
    PlayerScene,  // The current scene the player is in
    VolumeData,
    NextOpponent,
    PlayerHP,
    PathSystem
}


public class SaveSystem
{
    private static readonly string SAVES_FOLDER = "SAVES";

    private static readonly string DECK_FOLDER         = "DECKS";
    private static readonly string DECK_SAVE_LOCATION  = Path.Combine(SAVES_FOLDER, DECK_FOLDER);

    private static readonly string CONSUMABLES_FOLDER         = "CONSUMABLES";
    private static readonly string CONSUMABLES_SAVE_LOCATION  = Path.Combine(SAVES_FOLDER, CONSUMABLES_FOLDER);

    private static readonly string TOTEMS_FOLDER = "TOTEMS";
    private static readonly string TOTEMS_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, TOTEMS_FOLDER);

    private static readonly string WORD_INFO_FOLDER        = "WORDS";
    private static readonly string WORD_INFO_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, WORD_INFO_FOLDER);

    private static readonly string PLAYER_STATS_FOLDER        = "PLAYER";
    private static readonly string PLAYER_STATS_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, PLAYER_STATS_FOLDER);
    
    private static readonly string PLAYER_SCENE_SAVE_LOCATION  = Path.Combine(SAVES_FOLDER, PLAYER_STATS_FOLDER);
    private static readonly string PLAYER_VOLUME_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, PLAYER_STATS_FOLDER);
    private static readonly string PLAYER_HP_SAVE_LOCATION     = Path.Combine(SAVES_FOLDER, PLAYER_STATS_FOLDER);

    private static readonly string NEXT_OPPONENT_FOLDER        = "PATH";
    private static readonly string NEXT_OPPONENT_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, NEXT_OPPONENT_FOLDER);

    private static readonly string PLAYER_SYSTEM_FOLDER = "PLAYER_SYSTEM";
    private static readonly string PLAYER_SYSTEM_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, PLAYER_SYSTEM_FOLDER);

    private static readonly string PATH_SYSTEM_FOLDER = "PATH_SYSTEM";
    private static readonly string PATH_SYSTEM_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, PATH_SYSTEM_FOLDER);

    /**
     * @brief Gets the save file name for the given save file type.
     * @param The save file type.
     * @return The actual save file name.
     */
    private static string _GetSaveFileName(SaveSystemFile file)
    {
        switch (file)
        {
            case SaveSystemFile.PlayerDeck:
                return "PLAYER_DECK.json";
            case SaveSystemFile.PlayerConsumables:
                return "PLAYER_CONSUMABLES.json";
            case SaveSystemFile.OpponentDeck1:
                return "OPPONENT1_DECK.json";
            case SaveSystemFile.TotemModifiers:
                return "TOTEM_MODIFIERS.json";
            case SaveSystemFile.TotemOrders:
                return "TOTEM_ORDERS.json";
            case SaveSystemFile.TotemModifierNames:
                return "TOTEM_MODIFIER_NAMES.json";
            case SaveSystemFile.WordInfo:
                return "WORD_INFO.json";
            case SaveSystemFile.PlayerStats:
                return "PLAYER_STATS.json";
            case SaveSystemFile.PlayerScene:
                return "PLAYER_SCENE.json";
            case SaveSystemFile.VolumeData:
                return "VOLUME_DATA.json";
            case SaveSystemFile.NextOpponent:
                return "NEXT_OPPONENT_DATA.json";
            case SaveSystemFile.PlayerHP:
                return "PLAYER_HP.json";
            case SaveSystemFile.PathSystem:
                return "PathSystemSave.json";

            default:
                // TODO(KASIN):
                throw new System.NotImplementedException();
        }
    }
    
    /**
     * @brief Gets the directory path for the file.
     * @param The save file type.
     * @return The Directory Path for the save file.
     */
    private static string _GetSaveFileLocation(SaveSystemFile file)
    {
        switch (file)
        {
            case SaveSystemFile.PlayerDeck:
                return DECK_SAVE_LOCATION;
            case SaveSystemFile.PlayerConsumables:
                return CONSUMABLES_SAVE_LOCATION;
            case SaveSystemFile.OpponentDeck1:
                // TODO
                return "";
            case SaveSystemFile.TotemModifiers:
                return TOTEMS_SAVE_LOCATION;
            case SaveSystemFile.TotemModifierNames:
                return TOTEMS_SAVE_LOCATION;
            case SaveSystemFile.TotemOrders:
                return TOTEMS_SAVE_LOCATION;
            case SaveSystemFile.WordInfo:
                return WORD_INFO_SAVE_LOCATION;
            case SaveSystemFile.PlayerStats:
                return PLAYER_STATS_SAVE_LOCATION;
            case SaveSystemFile.PlayerScene:
                return PLAYER_SCENE_SAVE_LOCATION;
            case SaveSystemFile.VolumeData:
                return PLAYER_VOLUME_SAVE_LOCATION;
            case SaveSystemFile.NextOpponent:
                return NEXT_OPPONENT_SAVE_LOCATION;
            case SaveSystemFile.PlayerHP:
                return PLAYER_HP_SAVE_LOCATION;
            case SaveSystemFile.PathSystem:
                return PATH_SYSTEM_SAVE_LOCATION;

            default:
                // TODO(KASIN):
                throw new System.NotImplementedException();
        }
    }

    private static void _CheckForFolderStructure(SaveSystemFile file)
    {
        // Check if the save folders exists
        if (!Directory.Exists(SAVES_FOLDER))
        {
            Directory.CreateDirectory(SAVES_FOLDER);
        }

        // Check if the path save subfolder exists
        if (!Directory.Exists(_GetSaveFileLocation(file)))
        {
            Directory.CreateDirectory(_GetSaveFileLocation(file));
        }
    }

    /**
     * @brief Gets the directory path including the file name.
     * @param The save file type.
     * @return The entire Directory Path including the file.
     */
    public static string GetFullPath(SaveSystemFile file)
    {
        return Path.Combine(_GetSaveFileLocation(file), _GetSaveFileName(file));
    }

    
    /**
     * @brief Save the JSON string to a file
     * @param json The JSON string.
     * @param file The file type
     * @param mode The writing mode for the file. (Append, create, etc.)
     */
    private static void SaveToJsonFile(string json, SaveSystemFile file, FileMode mode = FileMode.Create)
    {
        // Check if the save folders exists
        if (!Directory.Exists(SAVES_FOLDER))
        {
            Directory.CreateDirectory(SAVES_FOLDER);
        }

        // Check if the path save subfolder exists
        if (!Directory.Exists(_GetSaveFileLocation(file)))
        {
            Directory.CreateDirectory(_GetSaveFileLocation(file));
        }
        
        // TODO(KASIN): This may need error handling
        FileStream fs = new FileStream(GetFullPath(file), mode);
        StreamWriter output = new StreamWriter(fs);
        output.Write(json);
        output.Flush();
        output.Close();

        if (fs != null)
        {
            fs.Close();
        }
    }

    /**
     * @brief Load the JSON string from a file
     * @param file The file type
     * @return The JSON string
     */
    private static string LoadJsonFile(SaveSystemFile file)
    {
        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
        }
        catch (System.IO.FileNotFoundException)
        {
            return null;
        }

        string json = reader.ReadToEnd();
        reader.Close();
        return json;
    }

    /**
     * @brief Save the deck of cards to a file
     * @param cards The cards that compose the deck
     * @param file The deck save file
     */

    public static void SaveDeck(CardData[] cards, SaveSystemFile file)
    {
        SaveToJsonFile(Deck.ToJson(cards), file);
    }

    /**
     * @brief Load the deck of cards from the save file
     * @return The deck of cards
     */
    public static CardData[] LoadDeck(SaveSystemFile file)
    {
        string json = LoadJsonFile(file);
        if (json != null)
        {
            return Deck.FromJson(json);
        }
        return null;
    }
    
    /**
     * @brief Add a card to the deck save.
     * @param card The card to add to the deck save file.
     * @param file The save file type.
     */
    public static void AddCardToDeckSave(CardData card, SaveSystemFile file)
    {
        CardData[] cards = LoadDeck(file);
        CardData[] new_card_list = new CardData[cards.Length + 1];
        for (int i = 0; i < cards.Length; ++i)
        {
            new_card_list[i] = cards[i];
        }
        new_card_list[new_card_list.Length - 1] = card;

        SaveDeck(new_card_list, file);
    }

    /**
     * @brief Removes a the first occurance of a card from the save file.
     * This will only remove one card at most. Call it multiple times to remove
     * more than one card. Does a out-of-order replacment (will change the card
     * ordering).
     * @note Make sure you reload the deck from the save to ensure the new data
     * is loaded.
     * @param card The card to remove.
     * @param file The deck to remove from.
     */
    public static void RemoveCardFromDeckSave(CardData card, SaveSystemFile file)
    {
        CardData[] cards = LoadDeck(file);
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].Compare(card))
            {
                cards[i] = cards[cards.Length - 1];
                cards[cards.Length - 1] = null;
                break;
            } 
        }
        
        if (cards[cards.Length - 1] == null)
        {
            CardData[] new_card_list = new CardData[cards.Length - 1];
            for (int i = 0; i < new_card_list.Length; ++i)
            {
                new_card_list[i] = cards[i];
            }
            SaveDeck(new_card_list, file);
        }
        else
        {
            SaveDeck(cards, file);
        }
    }
    
    /**
     * @brief Check to see if the consumable file exists.
     * @param The file type.
     * @return True if the file already exists. 
     */
    public static bool CheckForConsumableFileExistence(SaveSystemFile file)
    {
        return File.Exists(GetFullPath(file));
    }
    
    /**
     * @brief Remove the file given.
     * @param file The file to delete.
     */
    private static void DeleteSaveFile(SaveSystemFile file)
    {
        File.Delete(GetFullPath(file));
    }

    private class ConsumableSaveData
    {
        public string[] consumable_type;
        public string[] consumables;

        public ConsumableSaveData(IConsumableSavable[] consumables)
        {
            this.consumable_type = new string[consumables.Length];
            this.consumables = new string[consumables.Length];
            for(int i = 0; i < consumables.Length; ++i)
            {
                this.consumable_type[i] = consumables[i].GetType().ToString();
                this.consumables[i] = consumables[i].ToJson(); 
            } 
        }

        public static ConsumableSaveData FromJson(string json)
        {
            return JsonUtility.FromJson<ConsumableSaveData>(json);
        }
    }

    /**
     * @brief Loads all the consumable data from the save file.
     * @param file The SaveSystemFile type for the consumable.
     * @return The consumables as a object with type information inside.
     */
    public static ScriptableObject[] LoadConsumablesFromSaveFile()
    {
        List<ScriptableObject> consumables = new List<ScriptableObject>();

        string json = LoadJsonFile(SaveSystemFile.PlayerConsumables);
        if (json == null)
        {
            return null;
        }

        ConsumableSaveData save_data = ConsumableSaveData.FromJson(json);
        for (int i = 0; i < save_data.consumables.Length; ++i)
        {
            consumables.Add(ScriptableObject.CreateInstance(save_data.consumable_type[i]));
            consumables[i] = ((IConsumableSavable)consumables[i]).FromJson(save_data.consumables[i]).consumable;
        }

        return consumables.ToArray();
    }
    
    /**
     * @brief Save the consumables to the save file. Will overwrite existing
     * data. Will create an empty file if there are no consumables in the
     * array passed in.
     * @param consumables The array of consumables that will be saved
     * @param file The SaveSystemFile type for the consumables.
     */
    public static void SaveConsumablesToFile(IConsumableSavable[] consumables)
    {
        SaveToJsonFile(JsonUtility.ToJson(new ConsumableSaveData(consumables), true),
            SaveSystemFile.PlayerConsumables);
    }

    public static void SaveTotemOrders(CardOrder[] orders, SaveSystemFile file)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < orders.Length; ++i)
        {
            string json_string = JsonUtility.ToJson(orders[i]);
            sb.Append(json_string);
            if (i < orders.Length - 1)
            {
                sb.Append("\n");
            }
        }
        SaveToJsonFile(sb.ToString(), file);
    }

    public static CardOrder[] LoadTotemOrders(SaveSystemFile file)
    {
        List<CardOrder> orders = new List<CardOrder>();

        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
        }
        catch (System.IO.FileNotFoundException)
        {
            return null;
        }

        string line = reader.ReadLine();

        while (line != null)
        {
            CardOrder temp = JsonUtility.FromJson<CardOrder>(line);
            orders.Add(temp);

            line = reader.ReadLine();
        }
        reader.Close();

        return orders.ToArray();
    }

    public static void AddTotemOrderToSaveFile(CardOrder order, SaveSystemFile file)
    {
        CardOrder[] orders = LoadTotemOrders(file);
        CardOrder[] new_orders = new CardOrder[orders.Length + 1];
        for (int i = 0; i < orders.Length; ++i)
        {
            new_orders[i] = orders[i];
        }
        new_orders[new_orders.Length - 1] = order;

        SaveTotemOrders(new_orders, file);
    }

    private class TotemModifierSaveData
    {
        public string[] modifiers;

        public TotemModifierSaveData(CardModifier[] modifiers)
        {
            this.modifiers = new string[modifiers.Length];
            for (int i = 0; i < modifiers.Length; ++i)
            {
                this.modifiers[i] = modifiers[i].ToJson();
            }
        }

        public static CardModifier[] FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            } 

            TotemModifierSaveData save_data = JsonUtility.FromJson<TotemModifierSaveData>(json);

            CardModifier[] modifiers = new CardModifier[save_data.modifiers.Length];

            for (int i = 0; i <  save_data.modifiers.Length; ++i)
            {
                modifiers[i] = CardModifier.FromJson(save_data.modifiers[i]);
            }

            return modifiers;
        }
    }

    public static void SaveTotemModifiers(CardModifier[] modifiers, SaveSystemFile file)
    {
        StringBuilder sb_name = new StringBuilder();
        for (int i = 0; i < modifiers.Length; ++i)
        {
            sb_name.Append(modifiers[i].GetName());

            if (i < modifiers.Length - 1)
            {
                sb_name.Append("\n");
            }
        }

        SaveToJsonFile(
            JsonUtility.ToJson(new TotemModifierSaveData(modifiers), true),
            file
        );
        SaveToJsonFile(sb_name.ToString(), SaveSystemFile.TotemModifierNames);
    }

    public static CardModifier[] LoadTotemModifiers(SaveSystemFile file)
    {
        string json = LoadJsonFile(file);
        CardModifier[] modifiers = TotemModifierSaveData.FromJson(json);
        if (modifiers == null)
        {
            return null;
        }

        foreach (CardModifier modifier in modifiers)
        {
            modifier.name = modifier.GetName();
        }

        return modifiers;
    }

    private static CardModifier LoadTotemModifiersHelper(string name)
    {
        switch (name)
        {
            case "African Derived Queen Defence":
                return ScriptableObject.CreateInstance<AfricanDerivedQueenModifier>();

            case "Queen":
                return ScriptableObject.CreateInstance<QueenModifier>();

            case "Anti-Insect":
                return ScriptableObject.CreateInstance<AntiInsectModifier>();

            case "Armored":
                return ScriptableObject.CreateInstance<ArmoredCardModifier>();

            case "Attack Speed":
                return ScriptableObject.CreateInstance<AttackSpeedCardModifier>();

            case "Chemical Spray":
                return ScriptableObject.CreateInstance<ChemicalSprayCardModifier>();

            case "Dodge":
                return ScriptableObject.CreateInstance<DodgeCardModifier>();

            case "Explode on Death":
                return ScriptableObject.CreateInstance<ExplodeOnDeathModifier>();

            case "Flea Multiplier":
                return ScriptableObject.CreateInstance<QueenModifier>();

            case "Flutter":
                return ScriptableObject.CreateInstance<FlutterModifier>();

            case "Flutter+":
                return ScriptableObject.CreateInstance<FlutterModifier>();

            case "Heal on Attack":
                return ScriptableObject.CreateInstance<HealOnAttackModifier>();

            case "Jump":
                return ScriptableObject.CreateInstance<JumpModifier>();

            case "Locus Swarm":
                return ScriptableObject.CreateInstance<QueenModifier>();

            case "Move to Lane Attacked":
                return ScriptableObject.CreateInstance<MoveToLaneAttackedModifier>();

            case "Nektar Redution":
                return ScriptableObject.CreateInstance<NektarReductionModifier>();

            case "Side Strike":
                return ScriptableObject.CreateInstance<SideStrikeModifier>();

            case "Spawn Child":
                return ScriptableObject.CreateInstance<SpawnChildModifier>();

            case "Stinger Detach":
                return ScriptableObject.CreateInstance<StingerDetachModifier>();

            case "Strength in Numbers":
                return ScriptableObject.CreateInstance<StrengthInNumberModifier>();

            case "Web":
                return ScriptableObject.CreateInstance<ArmoredCardModifier>();

            default:
                // TODO(ALEX):
                throw new System.NotImplementedException();
        }
    }

    public static void AddTotemModifierToSaveFile(CardModifier modifier, SaveSystemFile file)
    {
        CardModifier[] modifiers = LoadTotemModifiers(file);
        CardModifier[] new_modifiers = new CardModifier[modifiers.Length + 1];
        for (int i = 0; i < modifiers.Length; ++i)
        {
            new_modifiers[i] = modifiers[i];
        }
        new_modifiers[new_modifiers.Length - 1] = modifier;

        SaveTotemModifiers(new_modifiers, file);
    }

    public class WordInfoSave
    {
        public string[] words_json;

        public WordInfoSave(WordInfo[] words)
        {
            this.words_json = new string[words.Length];
            for (int i = 0; i < this.words_json.Length; ++i)
            {
                this.words_json[i] = words[i].ToJson();
            }
        }

        public static WordInfoSave FromJson(string json)
        {
            return JsonUtility.FromJson<WordInfoSave>(json);
        }
    }

    public static void SaveWordInfo(WordInfo[] words)
    {
        SaveToJsonFile(JsonUtility.ToJson(new WordInfoSave(words), true),
            SaveSystemFile.WordInfo);
    }

    /**
     * @brief Load the word information data from the save file
     * @return The word info of cards
     */
    public static WordInfo[] LoadWords()
    {
        string json = LoadJsonFile(SaveSystemFile.WordInfo);
        WordInfoSave save_data = WordInfoSave.FromJson(json);

        WordInfo[] ret = new WordInfo[save_data.words_json.Length];
        for (int i = 0; i <  ret.Length; ++i)
        {
            ret[i] = WordInfo.FromJson(save_data.words_json[i]);
        }

        return ret;
    }

    /**
     * @brief Check to see if the file exists.
     * @param The file type.
     * @return True if the file already exists. 
     */
    public static bool CheckForFileExistence(SaveSystemFile file)
    {
        return File.Exists(GetFullPath(file));
    }

    /**
     * @brief Save the player stats to a file
     * @param player_data The stats for the during the game
     */
    public static void _SavePlayerStats(PlayerData player_data)
    {
        SaveToJsonFile(player_data.ToJson(), SaveSystemFile.PlayerStats);
    }

    /**
     * @brief Load the player stats to a file
     * @return Default player stats if file does not exist, otherwise, the
     * player's stats.
     */
    public static PlayerData _LoadPlayerStats()
    {
        string json = LoadJsonFile(SaveSystemFile.PlayerStats);
        if (json == null)
        {
            return new PlayerData();
        }

        return PlayerData.FromJson(json);
    }

    /** 
     * @breif Will determine what scene to load from the start menu 
     * @param scene_name The name of the scene the player was last in
     */
    public static void SavePlayerPathNodeState(string scene_name)
    {
        PlayerPathNodeState s = new PlayerPathNodeState();
        s.curr_scene = scene_name;
        SaveToJsonFile(JsonUtility.ToJson(s), SaveSystemFile.PlayerScene);
    }

    private class PlayerPathNodeState
    {
        public string curr_scene;
    }

    public static string LoadPlayerPathNodeState()
    {
        SaveSystemFile file = SaveSystemFile.PlayerScene;
        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
        }
        catch (System.IO.FileNotFoundException)
        {
            // Return default Scene 
            return "Path";
        }

        string line = reader.ReadLine();
        reader.Close();

        PlayerPathNodeState s = new PlayerPathNodeState();

        // TODO(KASIN): Error checking
        JsonUtility.FromJsonOverwrite(line, s);
        Debug.Log(s);
        return s.curr_scene;
    }

    /**
     * @brief Save the player volume settings to a file
     * @param VolumeData The volume levels set in the menu
     */
    public static void SaveVolumeData(VolumeControl.VolumeData volume_data)
    {
        SaveToJsonFile(JsonUtility.ToJson(volume_data), SaveSystemFile.VolumeData);
    }

    /**
     * @brief Load the deck of cards from the save file
     * @return The deck of cards
     */
    public static VolumeControl.VolumeData LoadVolumeData()
    {
        VolumeControl.VolumeData ret = new VolumeControl.VolumeData();
        SaveSystemFile file = SaveSystemFile.VolumeData;
        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
        }
        catch (System.IO.FileNotFoundException)
        {
            // Return default stats
            Debug.Log("Failed to load Volume Data from file: " + GetFullPath(file));
            return ret;
        }

        string line = reader.ReadLine();
        reader.Close();

        JsonUtility.FromJsonOverwrite(line, ret);
        Debug.Log("Loaded Volume Data from file.");
        return ret;
    }

    /**
     * @brief Save the next Opponent play style and type to a file
     * @param next_opponent The opponent type and attack style
     */
    public static void SaveNextOpponentData(NextOpponentData next_opponent)
    {
        SaveToJsonFile(JsonUtility.ToJson(next_opponent), SaveSystemFile.NextOpponent);
    }

    /**
     * @brief Load the next opponent data 
     * @return The next opponent data 
     */
    public static NextOpponentData LoadNextOpponentData()
    {
        NextOpponentData ret = new NextOpponentData();
        SaveSystemFile file = SaveSystemFile.NextOpponent;
        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
        }
        catch (System.IO.FileNotFoundException)
        {
            // Return default stats
            return ret;
        }

        string line = reader.ReadLine();
        reader.Close();

        JsonUtility.FromJsonOverwrite(line, ret);
        Debug.Log("Loaded Next Opponent data from file.");
        return ret;
    }
    
    // JsonUtility.ToJson wont write out only an int, so need a wrapper class
    // to hold the save data
    private class HPSaveData
    {
        public int hp;

        public HPSaveData(int hp)
        {
            this.hp = hp;
        }
    }

    /**
     * @brief Saves the Player HP data 
     */
    public static void SavePlayerHP(int hp)
    {
        HPSaveData save_data = new HPSaveData(hp); 
        SaveToJsonFile(JsonUtility.ToJson(save_data), SaveSystemFile.PlayerHP); 
    }

    /**
     * @brief Load the Player HP data 
     * @return The Player's HP value that was saved, or -1 on error
     */
    public static int LoadPlayerHP()
    {
        HPSaveData save_data = new HPSaveData(0); 
        SaveSystemFile file = SaveSystemFile.PlayerHP;
        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
        }
        catch (System.IO.FileNotFoundException)
        {
            // Return default stats
            return -1;
        }

        string line = reader.ReadLine();
        reader.Close();

        JsonUtility.FromJsonOverwrite(line, save_data);
        Debug.Log("Loaded Player HP data from file.");
        return save_data.hp;
    }

    private class PathSystemSaveData
    {
        public string end_node_guid;
        public string current_node_guid;

        public PathSystemSaveData()
        {
            end_node_guid = string.Empty;
            current_node_guid = string.Empty;
        }

    }

    public static (string current_node_guid, string end_node_guid) LoadPathSystemSaveData()
    {
        PathSystemSaveData path_data = new PathSystemSaveData();
        SaveSystemFile file = SaveSystemFile.PathSystem;
        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
        }
        catch (System.IO.FileNotFoundException)
        {
            Debug.Log("Failed to load path Data from file");
            return (path_data.current_node_guid, path_data.end_node_guid);
        }

        string line = reader.ReadLine();
        reader.Close();

        JsonUtility.FromJsonOverwrite(line, path_data);
        Debug.Log("Loaded path data from file.");

        return (path_data.current_node_guid, path_data.end_node_guid);
    }
}
