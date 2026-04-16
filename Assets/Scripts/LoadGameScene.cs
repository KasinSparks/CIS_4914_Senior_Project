using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{

    public void Load(string scene_name)
    {
        LoadScene(scene_name);
    }

    public static void LoadScene(string scene_name)
    {
        // Load the player stats from file
        PlayerStats.Load();
        
        if (scene_name != null && !scene_name.Equals("StartScreen") && !scene_name.Equals("Credits"))
        {
            // Load the scene in the save file
            scene_name = SaveSystem.LoadPlayerPathNodeState();
        }

        if (!string.IsNullOrEmpty(scene_name))
        {
            SceneManager.LoadScene(scene_name);
        }
    }

    public void Quit()
    {
        // Save the player stats to the player stats file
        PlayerStats.Save();

        Application.Quit();
    }
}