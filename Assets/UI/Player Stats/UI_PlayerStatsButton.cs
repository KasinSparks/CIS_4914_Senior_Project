using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatsButton : MonoBehaviour
{
    [SerializeField]
    private Button button;

    void Awake()
    {
        this.button.interactable = true;

        GameObject obj = GameObject.Find("-----UI-----/Player Stats");
        if (obj == null)
        {
            this.button.interactable = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
