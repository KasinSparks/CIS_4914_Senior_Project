using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    public void ShowStats()
    {
        GameObject panel = GameObject.Find("-----UI-----/Player Stats").transform.Find("Panel").gameObject;
        panel.SetActive(true);
    }

    public void HideStats()
    {
        GameObject panel = GameObject.Find("-----UI-----/Player Stats").transform.Find("Panel").gameObject;
        panel.SetActive(false);
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
