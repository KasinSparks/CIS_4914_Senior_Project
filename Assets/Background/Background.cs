using UnityEngine;
using UnityEngine.SceneManagement;

public class Background : MonoBehaviour
{
    private BackgroundSound.Sounds current_bg_type;
    private string current_bg_name;
    private GameObject current_bg_model;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        this.current_bg_type = SaveSystem.LoadPlayerPathNodeState().curr_background_sound_type;

        switch (current_bg_type)
        {
            case BackgroundSound.Sounds.Forest:
                current_bg_name = "forest";
                break;
            case BackgroundSound.Sounds.Desert:
                current_bg_name = "desert";
                break;
            case BackgroundSound.Sounds.Plains:
                current_bg_name = "plain";
                break;
        }

        if (scene.name == "Campfire" || scene.name == "Gameplay" || scene.name == "Reward"
            || scene.name == "Sacrafice" || scene.name == "Shop")
        {
            current_bg_model = Instantiate(Resources.Load(current_bg_name, typeof(GameObject))) as GameObject;
        }
    }

}
