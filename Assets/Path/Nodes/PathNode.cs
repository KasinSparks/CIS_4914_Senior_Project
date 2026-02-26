using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

/**
 * @brief A node for the PathSystem
 */
//public class PathNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
[Serializable]
public class PathNode : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private PathNodeData data;

    [SerializeField]
    private string[] next_nodes_guids; /// GUIDs for the next nodes

    private bool is_selectable;

    private PathSystem path_sys_ref;

    void Start()
    {
        if (this.next_nodes_guids == null)
        {
            this.next_nodes_guids = new string[0];
        }
        this.path_sys_ref = GameObject.Find("PathSystem").GetComponent<PathSystem>();
        this.GetComponent<Renderer>().material.mainTexture = this.data.GetImage();
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
            SceneManager.LoadScene(this.data.GetSceneName());
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

    /**
     * @brief Save the node state to a JSON file.
     * @param The JSON file to store this data.
     */
    public void SaveNode(string file_name)
    {
        //string json_data_node = JsonUtility.ToJson(data);
        string json_node = JsonUtility.ToJson(this);
        Debug.Log(json_node);
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
        JsonUtility.FromJsonOverwrite(json_str, this);
        reader.Close();
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
}