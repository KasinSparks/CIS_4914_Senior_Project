using UnityEngine;

public class CloseInfoBook : MonoBehaviour
{
    public void CloseBook()
    {
        GameObject obj = GameObject.Find("-----UI-----/UI_Book/Panel");
        obj.SetActive(false);
    }
}