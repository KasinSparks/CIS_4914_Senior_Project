using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

//public class PathNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
[Serializable]
public class PathNode : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private PathNodeData data;

    [SerializeField]
    private string[] next_nodes_guids;

    private bool is_selectable;

    private PathSystem path_sys_ref;

    void Start()
    {
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (is_selectable)
        {
            path_sys_ref.SetCurrentNode(this);
            path_sys_ref.SavePath();
            SceneManager.LoadScene(this.data.GetScene().name);
        }
    }

    public void SetSelectable(bool isSelectable)
    {
        this.is_selectable = isSelectable;
    }

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

    public void LoadNode(string file_name)
    {
        StreamReader reader = new StreamReader(file_name);
        string json_str = reader.ReadToEnd();
        JsonUtility.FromJsonOverwrite(json_str, this);
        reader.Close();
    }

    public string[] GetNextNodes()
    {
        return this.next_nodes_guids;
    }
}