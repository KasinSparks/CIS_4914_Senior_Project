using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystemTable
{
    static Dictionary<Guid, ISavable> save_info_table =
        new Dictionary<Guid, ISavable>();
    static Dictionary<int, Guid> unityid_to_guid_table = new Dictionary<int, Guid>();


    public static Guid FindGuid(int unity_id)
    {
        if (unityid_to_guid_table.ContainsKey(unity_id))
        {
            return unityid_to_guid_table[unity_id];
        }

        return Guid.Empty;
    }

    public static Guid Add(ISavable obj, int unity_id)
    {
        Guid guid = FindGuid(unity_id);
        if (guid != Guid.Empty)
        {
            // Overwrite existing
            unityid_to_guid_table[unity_id] = guid;
            save_info_table[guid] = obj;
            return guid;
        }

        guid = Guid.NewGuid();
        unityid_to_guid_table.Add(unity_id, guid);
        save_info_table.Add(guid, obj);
        return guid;
    }

    public static T Get<T>(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            throw new SaveSystemException("Tried to get an object from the save data with a guid of zero.");
        }
        
        return (T) save_info_table[guid];
    }

    public static void _TESTING_SaveToJsonFile(string file, ISavable obj)
    {
        // TODO(KASIN): Error checking
        StreamWriter output = new StreamWriter(file);
        output.Write(obj.ToJsonObject().ToStringJson());
        output.Flush();
        output.Close();
    }

    public static void WriteTableToDisk()
    {
        JsonAST ast = new JsonAST();
        ast.value = new JsonObject();

        Queue<Guid> saved_keys = new Queue<Guid>();
        Queue<Guid> todo_keys = new Queue<Guid>();

        foreach (Guid key in save_info_table.Keys)
        {
            todo_keys.Enqueue(key);
        }

        while (todo_keys.Count > 0)
        {
            while (todo_keys.Count > 0)
            {
                Guid current_key = todo_keys.Dequeue();
                ((JsonObject)ast.value).value.Add(
                    current_key.ToString(),
                    save_info_table[current_key].ToJsonObject()
                );
                saved_keys.Enqueue(current_key);
            }

            foreach (Guid key in save_info_table.Keys)
            {
                if (!saved_keys.Contains(key))
                {
                    todo_keys.Enqueue(key);
                }
            }

        }

        // TODO(KASIN): Error checking
        // TODO(KASIN): File path
        StreamWriter output = new StreamWriter(Path.Combine("SAVES", "reference_table.json"));
        output.Write(ast.ToStringJson());
        output.Flush();
        output.Close();
    }

    public static void ReadTableFromDisk()
    {
        JsonParser json_parser = new JsonParser();
        JsonAST json_ast = null;

        try
        {
            // TODO(KASIN): File path
            json_ast = json_parser.Parse(Path.Combine("SAVES", "reference_table.json"));
        }
        catch (Exception ex)
        {
            // TODO(KASIN): Error checking
            throw ex;
        }

        JsonObject json_dict = (JsonObject)json_ast.value;
        save_info_table.Clear();

        // TODO(KASIN): Dependency order may cause issues here
        // TODO(KASIN): See if there is a better way to handle dependencies 
        //    rather than just defering them. This could be really slow.
        // TODO(KASIN): I have not checked if there is a circular dependency
        Queue<string> keys = new Queue<string>();
        Queue<string> keys_defered = new Queue<string>();

        foreach (string key in json_dict.value.Keys)
        {
            keys.Enqueue(key);
        }
        
        while (keys.Count > 0)
        {
            while (keys.Count > 0)
            {
                string key = keys.Dequeue(); 
                Guid guid_key = Guid.Parse(key);
                JsonObject obj_info = ((JsonObject)json_dict.value[key]);
                Type obj_type = Type.GetType(((JsonString)obj_info.value["Type"]).value);
                JsonObject obj_data = ((JsonObject)obj_info.value["Data"]);

                Debug.Log("Key: " + key);
                Debug.Log("obj_type: " + obj_type);
                ISavable obj_with_type_info = CastToISavable(obj_type.Name);
                Debug.Log("obj_with_type_info: " + obj_with_type_info);
                if (obj_type.IsAssignableFrom(typeof(ISavable)))
                {
                    throw new SaveSystemException("Error loading Reference Table; Only types that implement ISavable can be stored in the save file.");
                }

                try
                {
                    obj_with_type_info.OverrideValuesFromJson(obj_data);
                    save_info_table.Add(guid_key, obj_with_type_info);
                    unityid_to_guid_table.Add(
                        ((UnityEngine.Object)obj_with_type_info).GetInstanceID(),
                        guid_key
                    );
                }
                catch (KeyNotFoundException ex)
                {
                    // Add to defer list and try again later
                    keys_defered.Enqueue(key);
                }
            }
            
            foreach (string key in keys_defered)
            {
                keys.Enqueue(key);
            }
            keys_defered.Clear();
        }
    }

    public static ISavable CastToISavable(string type)
    {
        ISavable ret = null;
        switch (type)
        {
            case "WordInfo":
                ret = ScriptableObject.CreateInstance<WordInfo>();
                break;
            case "ArmoredCardModifier":
                ret = ScriptableObject.CreateInstance<ArmoredCardModifier>();
                break;
            case "AttackSpeedCardModifier":
                ret = ScriptableObject.CreateInstance<AttackSpeedCardModifier>();
                break;
            case "ChemicalSprayCardModifier":
                ret = ScriptableObject.CreateInstance<ChemicalSprayCardModifier>();
                break;
            case "ChemicalSprayEffect":
                ret = ScriptableObject.CreateInstance<ChemicalSprayEffect>();
                break;
            case "DodgeCardModifier":
                ret = ScriptableObject.CreateInstance<DodgeCardModifier>();
                break;
            case "ExplodeOnDeathModifier":
                ret = ScriptableObject.CreateInstance<ExplodeOnDeathModifier>();
                break;
            case "HealOnAttackModifier":
                ret = ScriptableObject.CreateInstance<HealOnAttackModifier>();
                break;
            case "QueenModifier":
                ret = ScriptableObject.CreateInstance<QueenModifier>();
                break;
            case "StrengthInNumberModifier":
                ret = ScriptableObject.CreateInstance<StrengthInNumberModifier>();
                break;
            case "CardData":
                ret = ScriptableObject.CreateInstance<CardData>();
                break;
        }

        return ret;
    }


    // TODO(KASIN): Move this to another file
    public static byte[] ReadImageFile(string file)
    {
        // Try to guess the file extension
        string[] supported_image_extensions = new string[] {
            ".png",
            ".jpg",
            ".jpeg",
        };

        bool found_file = false;
        foreach (string extension in supported_image_extensions)
        {
            if (File.Exists(file + extension))
            {
                file = file + extension;
                found_file = true;
                break;
            }
        }

        if (!found_file)
        {
            throw new SaveSystemException("Unable to find file: " + file);
        }

        return File.ReadAllBytes(file);
        
    }


    // TODO(KASIN): Move this to another file
    public static JsonObject GetJsonForTexture2D(Texture2D texture)
    {
        JsonObject image_data = new JsonObject();

        string image_file_name = texture.name;

        string image_file = Path.Combine(
            new string[] {
                "SAVES",
                "IMAGES",
                image_file_name
            }
        );

        Debug.Log("texture.name: " + texture.name);
        image_data.value.Add("width", new JsonInt() { value = texture.width });
        image_data.value.Add("height", new JsonInt() { value = texture.height });
        image_data.value.Add("file", new JsonString() { value = image_file });

        return image_data;
    }

    public static Texture2D GetTexture2DFromJsonImage(JsonObject image_data)
    {
        int width   = ((JsonInt)image_data["width"]).value;
        int height  = ((JsonInt)image_data["height"]).value;
        string file = ((JsonString)image_data["file"]).value;

        Texture2D tex = new Texture2D(width, height);
        (tex).LoadImage(ReadImageFile(file));
        string[] path_split = file.Split(Path.DirectorySeparatorChar);
        tex.name = path_split[path_split.Length - 1];
        return tex;
    }

    public static void SaveDeck(CardData[] cards)
    {
        JsonAST ast = new JsonAST();
        ast.value = new JsonArray();

        foreach (CardData card in cards)
        {
            ((JsonArray)ast.value).value.Add(card.ToJsonObject());
        }

        File.WriteAllText(Path.Combine("SAVES", "DECKS", "PLAYER_DECK.json"),
            ast.ToStringJson());

        WriteTableToDisk();
    }

    public static CardData[] LoadDeck()
    {
        if (!File.Exists(Path.Combine("SAVES", "DECKS", "PLAYER_DECK.json")))
        {
            return null;
        }

        ReadTableFromDisk();

        JsonParser parser = new JsonParser();
        JsonAST ast = parser.Parse(Path.Combine("SAVES", "DECKS", "PLAYER_DECK.json"));

        JsonArray save_data_array = (JsonArray)ast.value;
        CardData[] ret = new CardData[save_data_array.value.Count];
        for (int i = 0; i < save_data_array.value.Count; ++i)
        {
            JsonObject save_data = (JsonObject)(save_data_array[i]);
            ret[i] = ScriptableObject.CreateInstance<CardData>();
            ret[i].OverrideValuesFromJson(save_data["Data"]);
        }

        return ret;
    }
}

public class SaveSystemException : System.Exception
{
    public SaveSystemException(string  message) : base(message) { }
}