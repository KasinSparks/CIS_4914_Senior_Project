using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SaveSystemFile
{
    None,
    PlayerDeck,
    OpponentDeck1,  // Example
    OpponentDeck2,  // Example, can change this to the name of the opponent
}

public class SaveSystem
{
    static readonly string SAVES_FOLDER        = "SAVES";
    static readonly string DECK_FOLDER         = "DECKS";
    static readonly string DECK_SAVE_LOCATION  = Path.Combine(SAVES_FOLDER, DECK_FOLDER);

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
            reader = new StreamReader(Path.Combine(DECK_SAVE_LOCATION, GetSaveFileName(file)));
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
     * @breif Gets the save file name for the given save file type.
     * @param The save file type.
     * @return The actual save file name.
     */
    private static string GetSaveFileName(SaveSystemFile file)
    {
        switch (file)
        {
            case SaveSystemFile.PlayerDeck:
                return "PLAYER_DECK.json";
            case SaveSystemFile.OpponentDeck1:
                return "OPPONENT1_DECK.json";

            default:
                // TODO(KASIN):
                throw new System.NotImplementedException();
        }
    }

    private static void SaveToJsonFile(string json, SaveSystemFile file)
    {
        string file_name = GetSaveFileName(file);

        // Check if the save folders exists
        if (!Directory.Exists(SAVES_FOLDER))
        {
            Directory.CreateDirectory(SAVES_FOLDER);
        }

        // Check if the path save subfolder exists
        if (!Directory.Exists(DECK_SAVE_LOCATION))
        {
            Directory.CreateDirectory(DECK_SAVE_LOCATION);
        }


        StreamWriter output = new StreamWriter(Path.Combine(DECK_SAVE_LOCATION, file_name));
        output.Write(json);
        output.Flush();
        output.Close();
    }
}