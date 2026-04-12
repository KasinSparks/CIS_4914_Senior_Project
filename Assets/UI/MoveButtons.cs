using UnityEngine;

//This is for the campfire scene, where I will move buttons instead of toggling setActive because I already manipulate that elsewhere
public class MoveButtons : MonoBehaviour
{
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;

    public void MoveForward()
    {
        Move(button1, 10000f);
        Move(button2, 10000f);
        Move(button3, 10000f);
    }

    public void MoveBack()
    {
        Move(button1, -10000f);
        Move(button2, -10000f);
        Move(button3, -10000f);
    }

    private void Move(GameObject obj, float direction)
    {
        if (obj.activeInHierarchy) //only moves if currently active
        {
            obj.transform.position += new Vector3(direction, 0f, 0f);
        }
    }
}
