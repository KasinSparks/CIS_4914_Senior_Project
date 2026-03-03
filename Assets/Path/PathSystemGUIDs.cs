using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Allows for game designer to add a GameObject reference in the editor
 * to the Global Unique Identifying Data (GUID). This will store the
 * GameObjects name as the GUID in a hashtable. GUIDs can be accessed at runtime
 * by calling the GetGameObject function.
 * @bug If you try to get a object by the GUID in the Awake function of another
 * script, the Dictionary object in this GameObject may be null. It is safe to
 * call GetGameObject in the Start function of other scripts.
 */
public class PathSystemGUIDs : MonoBehaviour
{
    [SerializeField]
    //private GameObjectGUID[] gUIDs;
    private GameObject[] gUIDs;

    private Dictionary<string, GameObject> dict;

    void Awake()
    {
        dict = new Dictionary<string, GameObject>();
        for (int i = 0; i < gUIDs.Length; ++i)
        {
            if (dict.ContainsKey(gUIDs[i].name))
            {
                throw new Exception("Tried to added two same GUIDs. STOPPED PathSystemGUIDs.");
            }

            dict.Add(gUIDs[i].name, gUIDs[i]);
        }

        foreach (string s in dict.Keys)
        {
            Debug.Log(s + " : " + this.dict[s]);
        }
    }
    
    /**
     * @brief Gets the GameObject represented by the given GUID
     * @param gUID The GUID of the GameObject you want.
     * @return A reference to the GameObject. If GUID is not in the HashTable,
     * null will be returned instead.
     */
    public GameObject GetGameObject(string gUID)
    {
        if (dict.ContainsKey(gUID))
        {
            return dict[gUID];
        }

        return null;
    }
}