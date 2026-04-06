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
    
    private static readonly string PLAYER_SCENE_SAVE_LOCATION = Path.Combine(SAVES_FOLDER, PLAYER_STATS_FOLDER);

    /**
     * @breif Gets the save file name for the given save file type.
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
    private static string GetFullPath(SaveSystemFile file)
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
     * @brief Save the deck of cards to a file
     * @param cards The cards that compose the deck
     * @param file The deck save file
     */

    public static void SaveDeck(CardData[] cards, SaveSystemFile file)
    {
        // TODO(KASIN): For now, each line will represent a different card
        //    However, this may need to be changed later.
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cards.Length; ++i)
        {
            string json_string = JsonUtility.ToJson(cards[i]);
            sb.Append(json_string);
            if (i < cards.Length - 1)
            {
                sb.Append("\n");
            }
        }
        SaveToJsonFile(sb.ToString(), file);
    }

    /**
     * @brief Load the deck of cards from the save file
     * @return The deck of cards
     */
    public static CardData[] LoadDeck(SaveSystemFile file)
    {
        List<CardData> cards = new List<CardData>();

        // TODO(KASIN): See if this throws an execption if file does not exist...
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
            cards.Add(ScriptableObject.CreateInstance<CardData>());
            JsonUtility.FromJsonOverwrite(line, cards[cards.Count - 1]);
            cards[cards.Count - 1].name = cards[cards.Count - 1].card_name;
            line = reader.ReadLine();
        }

        reader.Close();

        return cards.ToArray();
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

    /**
     * @brief Adds the consumable data to the end of the file.
     * @param consumable The consumable object.
     * @param file The Consumable SaveSystemFile type.
     * @param add_newline Set to True if this should this add a newline at the end.
     */
    public static void AppendConsumableToSaveFile(ScriptableObject consumable, SaveSystemFile file, bool add_newline = true)
    {
        string json_string = JsonUtility.ToJson(consumable);
        StringBuilder sb = new StringBuilder();
        sb.Append(consumable.GetType().ToString());
        sb.Append(" : ");
        sb.Append(json_string);
        if (add_newline)
        {
            sb.Append("\n");
        }
        SaveToJsonFile(sb.ToString(), file, FileMode.Append);
    }
    
    /**
     * @brief Loads all the consumable data from the save file.
     * @param file The SaveSystemFile type for the consumable.
     * @return The consumables as a object with type information inside.
     */
    public static ScriptableObject[] LoadConsumablesFromSaveFile(SaveSystemFile file)
    {
        List<ScriptableObject> consumables = new List<ScriptableObject>();

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
            ScriptableObject obj = null;
            if (line != null && line != "")
            {
                string[] parts = line.Split(" : ");
                // LHS has the original class type information
                obj = ScriptableObject.CreateInstance(parts[0]);
                // RHS will be the scriptable object data
                JsonUtility.FromJsonOverwrite(parts[1], obj);
                consumables.Add(obj);
            }

            line = reader.ReadLine();
        }

        reader.Close();

        return consumables.ToArray();
    }
    
    /**
     * @brief Save the consumables to the save file. Will overwrite existing
     * data. Will create an empty file if there are no consumables in the
     * array passed in.
     * @param consumables The array of consumables that will be saved
     * @param file The SaveSystemFile type for the consumables.
     */
    public static void SaveConsumablesToFile(ScriptableObject[] consumables, SaveSystemFile file)
    {
        // Clear existing save file
        SaveSystem.DeleteSaveFile(file);
        File.Create(GetFullPath(file)).Close();
        
        // Write the new data to the save file
        bool add_newline = true;
        for (int i = 0; i < consumables.Length; ++i)
        {
            if (i == consumables.Length - 1)
            {
                add_newline = false;
            }

            AppendConsumableToSaveFile(consumables[i], file, add_newline);
        }
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

    public static void SaveTotemModifiers(CardModifier[] modifiers, SaveSystemFile file)
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder sb_name = new StringBuilder();
        for (int i = 0; i < modifiers.Length; ++i)
        {
            string json_string = JsonUtility.ToJson(modifiers[i]);
            sb.Append(json_string);

            sb_name.Append(modifiers[i].GetName());

            if (i < modifiers.Length - 1)
            {
                sb.Append("\n");
                sb_name.Append("\n");
            }
        }
        SaveToJsonFile(sb.ToString(), file);
        SaveToJsonFile(sb_name.ToString(), SaveSystemFile.TotemModifierNames);
    }

    public static CardModifier[] LoadTotemModifiers(SaveSystemFile file)
    {
        List<CardModifier> modifiers = new List<CardModifier>();
        List<string> modifier_names = new List<string>();

        StreamReader reader = null;
        StreamReader reader_name = null;
        try
        {
            _CheckForFolderStructure(file);
            reader = new StreamReader(GetFullPath(file));
            reader_name = new StreamReader(GetFullPath(SaveSystemFile.TotemModifierNames));
        }
        catch (System.IO.FileNotFoundException)
        {
            return null;
        }

        string line = reader.ReadLine();
        string line_name = reader_name.ReadLine();

        while (line != null)
        {
            CardModifier mod = LoadTotemModifiersHelper(line_name);
            modifiers.Add(mod);
            JsonUtility.FromJsonOverwrite(line, modifiers[modifiers.Count - 1]);
            modifiers[modifiers.Count - 1].name = modifiers[modifiers.Count - 1].GetName();

            line = reader.ReadLine();
            line_name = reader_name.ReadLine();
        }
        reader.Close();
        reader_name.Close();

        return modifiers.ToArray();
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

    public static void SaveWordInfo(WordInfo[] words)
    {
        // TODO(KASIN): For now, each line will represent a different word.
        //    However, this may need to be changed later.
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < words.Length; ++i)
        {
            string json_string = JsonUtility.ToJson(words[i]);
            sb.Append(json_string);
            if (i < words.Length - 1)
            {
                sb.Append("\n");
            }
        }
        SaveToJsonFile(sb.ToString(), SaveSystemFile.WordInfo);
    }

    /**
     * @brief Load the word information data from the save file
     * @return The word info of cards
     */
    public static WordInfo[] LoadWords()
    {
        List<WordInfo> words = new List<WordInfo>();

        // TODO(KASIN): See if this throws an execption if file does not exist...
        StreamReader reader = null;
        try
        {
            _CheckForFolderStructure(SaveSystemFile.WordInfo);
            reader = new StreamReader(GetFullPath(SaveSystemFile.WordInfo));
        }
        catch (System.IO.FileNotFoundException)
        {
            return null;
        }

        string line = reader.ReadLine();
        while (line != null)
        {
            words.Add(ScriptableObject.CreateInstance<WordInfo>());
            JsonUtility.FromJsonOverwrite(line, words[words.Count - 1]);
            line = reader.ReadLine();
        }

        reader.Close();

        return words.ToArray();
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
        SaveToJsonFile(JsonUtility.ToJson(player_data), SaveSystemFile.PlayerStats);
    }

    /**
     * @brief Load the deck of cards from the save file
     * @return The deck of cards
     */
    public static PlayerData _LoadPlayerStats()
    {
        PlayerData ret = new PlayerData();
        SaveSystemFile file = SaveSystemFile.PlayerStats;
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
        return ret;
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

}