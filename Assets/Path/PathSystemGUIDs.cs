
using System;
using System.Collections.Generic;
using UnityEngine;


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

    public GameObject GetGameObject(string gUID)
    {
        return this.dict[gUID];
    }
}