using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
public class DevInstakill : MonoBehaviour
{
    void Update() //for the demo, if I hold die the combat ends
    { //hidden so that it can be in game without players noticing, cheat code
        var keyboard = Keyboard.current; //need to use this since our projects input settings were changed
        if (keyboard != null && keyboard.sKey.isPressed && keyboard.kKey.isPressed && keyboard.pKey.isPressed) //hold down die to skip to reward
        {
            SceneManager.LoadScene("reward");
        }
    }
}
