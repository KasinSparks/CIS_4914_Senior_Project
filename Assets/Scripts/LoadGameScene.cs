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

        if (scene_name == null)
        {
            throw new System.Exception("Failed to load scene due to scene_name being null.");
        }
        
        if (scene_name.Equals("StartScreen") || scene_name.Equals("Credits") ||
            scene_name.Equals("GameOver") || scene_name.Equals("GameWin"))
        {
            BackgroundSound.should_be_playing = false;
            if (BackgroundSound.is_playing)
            {
                BackgroundSound.Pause();
            }

            BackgroundMusic.should_be_playing = true;
            if (!BackgroundMusic.is_playing)
            {
                BackgroundMusic.Play();
            }

        }

        if (!scene_name.Equals("StartScreen") && !scene_name.Equals("Credits"))
        {
            BackgroundMusic.should_be_playing = false;
            if (BackgroundMusic.is_playing)
            {
                BackgroundMusic.Pause();
            }
            
            BackgroundSound.should_be_playing = true;

            // Load the scene in the save file
            SaveSystem.PlayerPathNodeState node_state = 
                SaveSystem.LoadPlayerPathNodeState();

            if (node_state == null)
            {
                scene_name = "Path";
                BackgroundSound.Play(BackgroundSound.Sounds.Path);
            }
            else
            {
                scene_name = node_state.curr_scene;
                BackgroundSound.Play(node_state.curr_background_sound_type);
            }

            
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