using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

[Serializable]
public struct PathNodeChances
{
    public PathNodeData node;
    [Range(0.0f, 1.0f)]
    public float        weight;
}

/**
 * @brief A node for the PathSystem
 */
//public class PathNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
[Serializable]
public class PathNode : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    [Tooltip("Don't assign to this unless you want to override the node_types.")]
    private PathNodeData data;

    [SerializeField]
    private string[] next_nodes_guids; /// GUIDs for the next nodes
    
    // TODO(KASIN): Don't need this to be serialized, but want it in the inspector
    [SerializeField]
    private PathNodeChances[] node_types; /// Node types that can be randomly chosen from

    private PathNodeChances[] normalized_node_types;


    private bool is_selectable;

    private PathSystem path_sys_ref;

    void Awake()
    {
        if (this.next_nodes_guids == null)
        {
            this.next_nodes_guids = new string[0];
        }
        this.path_sys_ref = GameObject.Find("PathSystem").GetComponent<PathSystem>();
    }

    public void UpdateImage()
    {
        this.GetComponent<Renderer>().material.mainTexture = this.data.GetImage();
    }

    public bool HasBeenAssignedPathNodeData()
    {
        if (this.data != null)
        {
            return true;
        }

        return false;
    }

    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
    */
    
    /**
     * @brief Will load the scene in stored in the node.
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        if (is_selectable)
        {
            path_sys_ref.SetCurrentNode(this);
            path_sys_ref.SavePath();
            PlayerStats.player_data.AddToNodesTraversed(1);
            PlayerStats.Save();
            SaveSystem.SavePlayerPathNodeState(this.data.GetSceneName());

            string next_scene = this.data.GetSceneName();
            if (next_scene.Equals("Gameplay"))
            {
                Debug.Log("Loading " +
                    Enum.GetName(typeof(CreatedOpponents), this.data.next_opponent.opponent) +
                    " Opponent with difficulty: " +
                    this.data.next_opponent.difficulty
                );
                SaveSystem.SaveNextOpponentData(this.data.next_opponent);
            }

            SceneManager.LoadScene(next_scene);
        }
    }
    
    /**
     * @brief Update this node to either be selectable or not.
     * @param isSelectable Is the node selectable by the player.
     */
    public void SetSelectable(bool isSelectable)
    {
        this.is_selectable = isSelectable;
    }

    private class SaveData
    {
        public string data;
        public string[] next_nodes_guid;


        public SaveData(PathNode data)
        {
            this.data = data.data.ToJson();
            this.next_nodes_guid = data.next_nodes_guids;
        }

        public static SaveData FromJson(string json)
        {
            return JsonUtility.FromJson<SaveData>(json);
        }
    }
    
    public string ToJson()
    {
        return JsonUtility.ToJson(new SaveData(this), true);
    }

    public void FromJson(string json)
    {
        SaveData save_data = SaveData.FromJson(json);
        this.data = PathNodeData.FromJson(save_data.data);
        this.next_nodes_guids = new string[save_data.next_nodes_guid.Length];
        for (int i = 0; i < this.next_nodes_guids.Length; ++i)
        {
            this.next_nodes_guids[i] = save_data.next_nodes_guid[i];
        }
    }

    /**
     * @brief Save the node state to a JSON file.
     * @param The JSON file to store this data.
     */
    public void SaveNode(string file_name)
    {
        //string json_data_node = JsonUtility.ToJson(data);
        string json_node = this.ToJson();
        StreamWriter writer = new StreamWriter(file_name);
        writer.Write(json_node);
        writer.Flush();
        writer.Close();
    }
    
    /**
     * @brief Loads the node's state from a JSON file.
     * @param The file containing this node's state.
     */
    public void LoadNode(string file_name)
    {
        StreamReader reader = new StreamReader(file_name);
        string json_str = reader.ReadToEnd();
        //JsonUtility.FromJsonOverwrite(json_str, this);
        this.FromJson(json_str);
        reader.Close();
        this.UpdateImage();
    }
    
    /**
     * @brief Gets the reference to the next nodes on the path.
     * @return An array of UGIDs to the next nodes. Will return an empty array
     * if there are no next nodes.
     */
    public string[] GetNextNodes()
    {
        return this.next_nodes_guids;
    }

    /**
     * @brief Set the node data
     * @param data The PathNodeData reference
     */
    public void SetPathNode(PathNodeData data)
    {
        this.data = data;
    }
    
    /**
     * @brief Normalize the chances of the nodes being selected. Does a
     * summation of all chances then computes the normalized value by dividing
     * the original chance by the total chance.
     */
    private void NormalizeChances()
    {
        // Normalize percent chance values
        float total_weight = 0.0f;
        for (int i = 0; i < this.node_types.Length; ++i)
        {
            total_weight += this.node_types[i].weight;
        }

        this.normalized_node_types = new PathNodeChances[this.node_types.Length];
        for (int i = 0; i < this.node_types.Length; ++i)
        {
            this.normalized_node_types[i].node = this.node_types[i].node;
            this.normalized_node_types[i].weight =
                this.node_types[i].weight / total_weight;
        }
    }

    /**
     * @brief Get a random PathNodeData type.
     * @return Returns a random PathNodeData type based on the normalized weights.
     * @todo Write a test to verify this produces random nodes that corelate to
     * the weight given.
     */
    public PathNodeData GetRandomPathNode()
    {
        this.NormalizeChances();

        float rand = UnityEngine.Random.Range(0.0f, 0.99f);
        float curr_val = 0.0f;
        for (int i = 0; i < this.normalized_node_types.Length; ++i)
        {
            if (rand >= curr_val && rand < this.normalized_node_types[i].weight + curr_val)
            {
                return this.normalized_node_types[i].node;
            }

            curr_val += this.normalized_node_types[i].weight;
        }
        
        // Should never reach here...
        return this.normalized_node_types[this.normalized_node_types.Length - 1].node;
    }
}