using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.IO;
public class DevInstakill : MonoBehaviour
{
    void Update() //for the demo, if I hold die the combat ends
    { //hidden so that it can be in game without players noticing, cheat code
        var keyboard = Keyboard.current; //need to use this since our projects input settings were changed
        if (keyboard != null && keyboard.sKey.isPressed && keyboard.kKey.isPressed && keyboard.pKey.isPressed) //hold down die to skip to reward
        {
            SceneManager.LoadScene("reward");
        }
        if (keyboard != null && keyboard.wKey.isPressed && keyboard.iKey.isPressed && keyboard.nKey.isPressed) //hold down die to skip to reward
        {
            PlayerStats.Save();
            DirectoryInfo dir = new DirectoryInfo("SAVES");
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
            if (subDir.Name != "WORDS" && subDir.Name != "PLAYER")
                {
                    Directory.Delete(Path.Combine(dir.Name, subDir.Name), true);
                }
                else if (subDir.Name == "PLAYER")
                {
                    File.Delete(Path.Combine(dir.Name, subDir.Name) + "/PLAYER_SCENE.json");
                    File.Delete(Path.Combine(dir.Name, subDir.Name) + "/PLAYER_HP.json");
                }
            }
            SceneManager.LoadScene("gamewin");
        }
    }
}
