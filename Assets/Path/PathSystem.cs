using System;
using System.IO;
using UnityEngine;

/**
 * @brief A GameObject to represent the path system in a given scene
 * @todo Change this to a Scriptable Object, or have the data be a be a
 * Scriptable Object.
 */
[Serializable]
public class PathSystem : MonoBehaviour 
{
    private readonly string SAVE_FOLDER = Path.Combine("SAVES", "PATH_SYSTEM");

    [SerializeField]
    private PathSystemType type;

    [SerializeField]
    private string path_name;

    [SerializeField]
    private string end_node_guid; /// The GUID of the end node

    [SerializeField]
    private string[] start_node_guids; /// The GUIDs of the start nodes

    [SerializeField]
    private PathNodeData[] node_types; /// Node types that can be randomly chosen from
    
    [SerializeField, HideInInspector]
    private string current_node_guid = null;

    private Camera _camera;
    private PathSystemGUIDs gUIDs;

    void Start()
    {
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
        } else
        {
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
        // Don't assign the same type twice in a continuous path

        this.SavePath();
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
        StreamWriter output = new StreamWriter(Path.Combine(SAVE_FOLDER, "PathSystemSave.json"));
        output.Write(json_path_system);
        output.Flush();
        output.Close();
        
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            Transform node = this.transform.GetChild(i);
            string node_name = node.gameObject.name;
            node.GetComponent<PathNode>().SaveNode(Path.Combine(SAVE_FOLDER, node_name + ".json"));
        }
    }
    
    /**
     * @brief Determines if the save path for the PathSystem already exists.
     * @return True if file already exist, false otherwise.
     */
    private bool CheckIfSavePathExist()
    {
        // TODO(KASIN): When game quit, delete the save files
        if (File.Exists(Path.Combine(SAVE_FOLDER, "PathSystemSave.json")))
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
        StreamReader reader = new StreamReader(Path.Combine(SAVE_FOLDER, "PathSystemSave.json"));
        string json_str = reader.ReadToEnd();
        JsonUtility.FromJsonOverwrite(json_str, this);
        reader.Close();

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            Transform node = this.transform.GetChild(i);
            string node_name = node.gameObject.name;
            node.GetComponent<PathNode>().LoadNode(Path.Combine(SAVE_FOLDER, node_name + ".json"));
        }
    }

}