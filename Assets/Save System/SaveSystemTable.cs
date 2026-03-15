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

        foreach (Guid key in save_info_table.Keys)
        {
            ((JsonObject)ast.value).value.Add(
                key.ToString(),
                save_info_table[key].ToJsonObject()
            );
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

        foreach (string key in json_dict.value.Keys) {
            Guid guid_key = Guid.Parse(key);
            JsonObject obj_info = ((JsonObject)json_dict.value[key]);
            Type obj_type = Type.GetType(((JsonString)obj_info.value["Type"]).value);
            JsonObject obj_data = ((JsonObject)obj_info.value["Data"]);

            Debug.Log("obj_type: " + obj_type);
            ISavable obj_with_type_info = CastToISavable(obj_type.Name);
            if (obj_type.IsAssignableFrom(typeof(ISavable)))
            {
                throw new SaveSystemException("Error loading Reference Table; Only types that implement ISavable can be stored in the save file.");
            }
            
            obj_with_type_info.OverrideValuesFromJson(obj_data);
            save_info_table.Add(guid_key, obj_with_type_info);
            unityid_to_guid_table.Add(
                ((UnityEngine.Object)obj_with_type_info).GetInstanceID(),
                guid_key
            );
        }
    }

    public static ISavable CastToISavable(string type)
    {
        // Disable the warning about Instantiate of Unity Objects.
        // Here it should be fine because we do not want the object to be an
        // object in the game scene. Rather, it is used inside of other object
        // as data.
        ISavable ret = null;
        switch (type)
        {
            case "WordInfo":
                ret = ScriptableObject.CreateInstance<WordInfo>();
                break;
            case "CardModifier":
                ret = ScriptableObject.CreateInstance<CardModifier>();
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
        return tex;
    }
}

public class SaveSystemException : System.Exception
{
    public SaveSystemException(string  message) : base(message) { }
}