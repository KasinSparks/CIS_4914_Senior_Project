using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{

    public void Load(string scene_name)
    {
        if (!string.IsNullOrEmpty(scene_name))
        {
            SceneManager.LoadScene(scene_name);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}