using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{

    public void Load(string scene_name)
    {
        // Load the player stats from file
        PlayerStats.Load();

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