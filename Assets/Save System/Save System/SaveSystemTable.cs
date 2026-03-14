using System;
using System.Collections.Generic;
using System.IO;

public static class SaveSystemTable
{
    static Dictionary<Guid, ISavable> save_info_table =
        new Dictionary<Guid, ISavable>();
    static Dictionary<int, Guid> unityid_to_guid_table = new Dictionary<int, Guid>();

    static Dictionary<Guid, object> load_info_table = new Dictionary<Guid, object>();

    //static Dictionary<Guid, int> guid_to_unityid_table = new Dictionary<Guid, int>();

    /*
    private static ISavable<object> Find(Guid id)
    {
        if (save_info_table.ContainsKey(id))
        {
            return save_info_table[id];
        }

        return null;
    }
    */

    public static Guid FindGuid(int unity_id)
    {
        if (unityid_to_guid_table.ContainsKey(unity_id))
        {
            return unityid_to_guid_table[unity_id];
        }

        return Guid.Empty;
    }

    /*
    public static int FindUnityId(Guid guid)
    {
        if (guid_to_unityid_table.ContainsKey(guid))
        {
            return guid_to_unityid_table[guid];
        }

        return 0;
    }
    */

    public static Guid Add(ISavable obj, int unity_id)
    {
        Guid guid = FindGuid(unity_id);
        if (guid == Guid.Empty)
        {
            guid = Guid.NewGuid();
            unityid_to_guid_table.Add(unity_id, guid);
        }

        return guid;
    }

    public static T Get<T>(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            throw new SaveSystemException("Tried to get an object from the save data with a guid of zero.");
        }

        return (T) load_info_table[guid];
    }

    public static void SaveToJsonFile(string file, ISavable obj)
    {
        // TODO(KASIN): Error checking
        StreamWriter output = new StreamWriter(file);
        output.Write(obj.ToJsonString());
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
        throw new NotImplementedException();
    }
}

public class SaveSystemException : System.Exception
{
    public SaveSystemException(string  message) : base(message) { }
}