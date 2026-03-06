using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public enum SaveSystemFile
{
    None,
    PlayerDeck,
    PlayerConsumables,
    OpponentDeck1,  // Example
    OpponentDeck2,  // Example, can change this to the name of the opponent
}

public class SaveSystem
{
    private static readonly string SAVES_FOLDER = "SAVES";

    private static readonly string DECK_FOLDER         = "DECKS";
    private static readonly string DECK_SAVE_LOCATION  = Path.Combine(SAVES_FOLDER, DECK_FOLDER);

    private static readonly string CONSUMABLES_FOLDER         = "CONSUMABLES";
    private static readonly string CONSUMABLES_SAVE_LOCATION  = Path.Combine(SAVES_FOLDER, CONSUMABLES_FOLDER);

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

            default:
                // TODO(KASIN):
                throw new System.NotImplementedException();
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
}