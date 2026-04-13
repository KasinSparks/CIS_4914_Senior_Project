using UnityEngine;
using System;


[Serializable]
public class NextOpponentData
{
    public CreatedOpponents opponent = CreatedOpponents.None;
    public OpponentAttackStyle opponent_attack_style = OpponentAttackStyle.Random;
    /// For now, this only limits the number of cards that are initially added
    /// to the opponent's deck on start.
    public int difficulty = 0;
}

/**
 * @brief A way to create more nodes.
 */
[CreateAssetMenu(menuName = "Path/Node")]
[Serializable]
public class PathNodeData : ScriptableObject{

    [SerializeField]
    /// The scene this node will load when it is clicked.
    private string scene_name;

    /// Only valid for the opponent scene
    public NextOpponentData next_opponent = null;

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

    private class PathNodeSaveData
    {
        public string scene_name;

        /* Next opponent data */
        public CreatedOpponents opponent;
        public OpponentAttackStyle opponent_attack_style;
        public int difficulty = 0;

        public string image_resource_path;

        public PathNodeSaveData(PathNodeData data)
        {
            this.scene_name = data.scene_name;
            if (data.next_opponent != null)
            {
                this.opponent = data.next_opponent.opponent;
                this.opponent_attack_style = data.next_opponent.opponent_attack_style;
                this.difficulty = data.next_opponent.difficulty;
            }
            this.image_resource_path = data.image.name;
        }

        public static PathNodeData FromJson(string json)
        {
            PathNodeData ret = ScriptableObject.CreateInstance<PathNodeData>();

            PathNodeSaveData data = JsonUtility.FromJson<PathNodeSaveData>(json);

            ret.scene_name = data.scene_name;
            ret.next_opponent = new NextOpponentData();
            if (data.difficulty <= 0)
            {
                ret.next_opponent = null;
            }
            else
            {
                ret.next_opponent.opponent = data.opponent;
                ret.next_opponent.opponent_attack_style = data.opponent_attack_style;
                ret.next_opponent.difficulty = data.difficulty;
            }
            ret.image = Resources.Load<Texture2D>("Images/" + data.image_resource_path);

            return ret;
        }
    }
    
    public string ToJson()
    {
        return JsonUtility.ToJson(new PathNodeSaveData(this), true);
    }

    public static PathNodeData FromJson(string json)
    {
        return PathNodeSaveData.FromJson(json);
    }
}
