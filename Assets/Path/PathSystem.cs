using System;
using System.IO;
using UnityEngine;


[Serializable]
public class PathSystem : MonoBehaviour 
{
    private readonly string SAVE_FOLDER = Path.Combine("SAVES", "PATH_SYSTEM");

    [SerializeField]
    private PathSystemType type;

    [SerializeField]
    private string path_name;

    [SerializeField]
    private string end_node_guid;

    [SerializeField]
    private string[] start_node_guids;

    [SerializeField]
    private PathNodeData[] node_types;

    //private int current_level = 0;
    [SerializeField, HideInInspector]
    private string current_node_guid = null;

    private Camera _camera;
    private PathSystemGUIDs gUIDs;

    void Start()
    {
        _camera = Camera.main;
        gUIDs = GameObject.Find("PathSystemGUIDs").GetComponent<PathSystemGUIDs>();

        if (CheckIfSavePathExist()) {
            this.LoadPath();
            // Set the nodes that are selectable
            if (this.current_node_guid == null || this.current_node_guid.Equals(""))
            {
                foreach (string guid in this.start_node_guids)
                {
                    PathNode node = this.gUIDs.GetGameObject(guid).GetComponent<PathNode>();
                    node.SetSelectable(true);
                }
            }
            else
            {
                PathNode node = this.gUIDs.GetGameObject(this.current_node_guid).GetComponent<PathNode>();
                string[] node_guids = node.GetNextNodes();
                foreach (string ng in node_guids)
                {
                    this.gUIDs.GetGameObject(ng).GetComponent<PathNode>().SetSelectable(true);
                }
            }
        } else
        {
            this.GeneratePath();
        }
    }

    public void SetCurrentNode(PathNode node)
    {
        this.current_node_guid = node.gameObject.name;
    }

    public void GeneratePath()
    {
        foreach (string guid in this.start_node_guids)
        {
            PathNode node = this.gUIDs.GetGameObject(guid).GetComponent<PathNode>();
            node.SetSelectable(true);
        }

        // Randomly assign node a type
        // Don't assign the same type twice in a continuous path

        this.SavePath();
    }

    public void MoveCamaeraToCurrentNode()
    {

    }
    
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

    public bool CheckIfSavePathExist()
    {
        // TODO(KASIN): When game quit, delete the save files
        if (File.Exists(Path.Combine(SAVE_FOLDER, "PathSystemSave.json")))
        {
            return true;
        }
        
        return false;
    }

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