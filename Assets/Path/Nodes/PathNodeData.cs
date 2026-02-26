using UnityEngine;
using System;

/**
 * @brief A way to create more nodes.
 */
[CreateAssetMenu(menuName = "Path/Node")]
[Serializable]
public class PathNodeData : ScriptableObject{

    [SerializeField]
    /// The scene this node will load when it is clicked.
    private string scene_name;

    [SerializeField]
    /// The Image for the node.
    private Texture2D image;
    
    /**
     * @brief Get the scene the node holds a reference to.
     * @return The Scene asset the node will load when clicked.
     */
    public string GetSceneName()
    {
        return this.scene_name;
    }
    
    /**
     * @brief Get the node's image.
     * @return The Image for the node.
     */
    public Texture2D GetImage()
    {
        return this.image;
    }
}
