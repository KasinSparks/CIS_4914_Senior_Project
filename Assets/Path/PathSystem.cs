using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct PathNodeChances
{
    public PathNodeData node;
    [Range(0.0f, 1.0f)]
    public float        weight;
}

/**
 * @brief A GameObject to represent the path system in a given scene
 * @todo Change this to a Scriptable Object, or have the data be a be a
 * Scriptable Object.
 */
[Serializable]
public class PathSystem : MonoBehaviour 
{
    private static readonly string SAVE_FOLDER_NAME = "SAVES";
    private static readonly string PATH_SAVE_FOLDER_NAME = "PATH_SYSTEM";
    private readonly string PATH_SAVE_LOCATION = Path.Combine(SAVE_FOLDER_NAME, PATH_SAVE_FOLDER_NAME);

    [SerializeField]
    private PathSystemType type;

    [SerializeField]
    private string path_name;

    [SerializeField]
    private string end_node_guid; /// The GUID of the end node

    [SerializeField]
    private string[] start_node_guids; /// The GUIDs of the start nodes

    [SerializeField]
    private PathNodeChances[] node_types; /// Node types that can be randomly chosen from

    private PathNodeChances[] normalized_node_types;
    
    [SerializeField, HideInInspector]
    private string current_node_guid = null;

    private Camera _camera;
    private PathSystemGUIDs gUIDs;

    void Start()
    {
        // Check if the save folders exists
        if (!Directory.Exists(SAVE_FOLDER_NAME))
        {
            Directory.CreateDirectory(SAVE_FOLDER_NAME);
        }

        // Check if the path save subfolder exists
        if (!Directory.Exists(PATH_SAVE_LOCATION))
        {
            Directory.CreateDirectory(PATH_SAVE_LOCATION);
        }


        _camera = Camera.main;
        gUIDs = GameObject.Find("PathSystemGUIDs").GetComponent<PathSystemGUIDs>();
        
        // See if we need to load the data from disk or generate a new path.
        if (CheckIfSavePathExist()) {
            this.LoadPath();
            // Set the nodes that are selectable
            if (this.current_node_guid == null || this.current_node_guid.Equals(""))
            {
                // TODO(KASIN): this may be redundant due to the starting nodes
                //    being assigned in the GeneratePath function.

                // Set the starting nodes as selectable since there is no
                // current node
                foreach (string guid in this.start_node_guids)
                {
                    PathNode node = this.gUIDs.GetGameObject(guid).GetComponent<PathNode>();
                    node.SetSelectable(true);
                }
            }
            else
            {
                // Set the current node's next nodes as selectable
                PathNode node = this.gUIDs.GetGameObject(this.current_node_guid).GetComponent<PathNode>();
                string[] node_guids = node.GetNextNodes();
                foreach (string ng in node_guids)
                {
                    this.gUIDs.GetGameObject(ng).GetComponent<PathNode>().SetSelectable(true);
                }
            }
        }
        else
        {
            // TODO(KASIN): Move this to a separate function
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

            // Create a new path
            this.GeneratePath();
        }
    }
    
    /**
     * @brief Updates the current node.
     * @param node the node you want to set to the current node.
     */
    public void SetCurrentNode(PathNode node)
    {
        this.current_node_guid = node.gameObject.name;
    }
    
    /**
     * @brief Generates a new path using the nodes place by a game designer.
     */
    private void GeneratePath()
    {
        // Make sure the starting nodes are selectable
        foreach (string guid in this.start_node_guids)
        {
            PathNode node = this.gUIDs.GetGameObject(guid).GetComponent<PathNode>();
            node.SetSelectable(true);
        }

        // TODO(KASIN):
        // Randomly assign node a type
        
        List<string> visited = new List<string>();
        Queue<string> node_queue = new Queue<string>();
        foreach (string guid in this.start_node_guids)
        {
            node_queue.Enqueue(guid);
        }

        while (node_queue.Count > 0)
        {
            // Assign node type
            string guid = node_queue.Dequeue();
            PathNode node = this.gUIDs.GetGameObject(guid).GetComponent<PathNode>();
            PathNodeData path_node = this.GetRandomPathNode();
            node.SetPathNode(path_node);
            node.UpdateImage();

            // Add the node to the visited list
            visited.Add(guid);
            // Add the children to the queue
            string[] child_nodes = node.GetNextNodes();
            for (int i = 0; i < child_nodes.Length; ++i)
            {
                if (!(visited.Contains(child_nodes[i])))
                {
                    node_queue.Enqueue(child_nodes[i]);
                }

            }
        }

        this.SavePath();
    }
    
    /**
     * @brief Get a random PathNodeData type.
     * @return Returns a random PathNodeData type based on the normalized weights.
     * @todo Write a test to verify this produces random nodes that corelate to
     * the weight given.
     */
    private PathNodeData GetRandomPathNode()
    {
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
    
    /**
     * @brief Will move the Main Camera to the current node.
     * @todo Implement function
     */
    public void MoveCamaeraToCurrentNode()
    {
        throw new NotImplementedException();
    }
    
    /**
     * @brief Saves the current path and nodes to a JSON file.
     */
    public void SavePath()
    {
        string json_path_system = JsonUtility.ToJson(this);
        StreamWriter output = new StreamWriter(Path.Combine(PATH_SAVE_LOCATION, "PathSystemSave.json"));
        output.Write(json_path_system);
        output.Flush();
        output.Close();
        
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            Transform node = this.transform.GetChild(i);
            string node_name = node.gameObject.name;
            node.GetComponent<PathNode>().SaveNode(Path.Combine(PATH_SAVE_LOCATION, node_name + ".json"));
        }
    }
    
    /**
     * @brief Determines if the save path for the PathSystem already exists.
     * @return True if file already exist, false otherwise.
     */
    private bool CheckIfSavePathExist()
    {
        // TODO(KASIN): When game quit, delete the save files
        if (File.Exists(Path.Combine(PATH_SAVE_LOCATION, "PathSystemSave.json")))
        {
            return true;
        }
        
        return false;
    }

    /**
     * @brief Loads the PathSystem data from disk.
     */

    public void LoadPath()
    {
        StreamReader reader = new StreamReader(Path.Combine(PATH_SAVE_LOCATION, "PathSystemSave.json"));
        string json_str = reader.ReadToEnd();
        JsonUtility.FromJsonOverwrite(json_str, this);
        reader.Close();

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            Transform node = this.transform.GetChild(i);
            string node_name = node.gameObject.name;
            node.GetComponent<PathNode>().LoadNode(Path.Combine(PATH_SAVE_LOCATION, node_name + ".json"));
        }
    }

}