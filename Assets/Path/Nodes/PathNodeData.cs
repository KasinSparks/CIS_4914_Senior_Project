
using UnityEditor;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Path/Node")]
[Serializable]
public class PathNodeData : ScriptableObject{
    [SerializeField]
    private SceneAsset scene;

    [SerializeField]
    private Texture2D image;
    
    public SceneAsset GetScene()
    {
        return this.scene;
    }

    public Texture2D GetImage()
    {
        return this.image;
    }
}
