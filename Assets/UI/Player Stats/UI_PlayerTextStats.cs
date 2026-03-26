using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerTextStats : MonoBehaviour
{
    // Use this dictionary to get the stats in the UI
    private Dictionary<string, UI_PlayerTextStat> text_stats =
        new Dictionary<string, UI_PlayerTextStat>();

    void Awake()
    {
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            UI_PlayerTextStat text_stat =
                this.transform.GetChild(i).GetComponent<UI_PlayerTextStat>();
            text_stat.UpdateFields();
            this.text_stats.Add(text_stat.GetName(), text_stat);
        }
    }

    void OnEnable()
    {
        this.text_stats["Damage Dealt"].SetValue(
            PlayerStats.player_data.GetDamageDealt().ToString());    
        this.text_stats["Insects Defeated"].SetValue(
            PlayerStats.player_data.GetInsectsDefeated().ToString());    
        this.text_stats["Opponents Defeated"].SetValue(
            PlayerStats.player_data.GetOpponentsDefeated().ToString());    
        this.text_stats["Nodes Traveled"].SetValue(
            PlayerStats.player_data.GetNodesTraversed().ToString());
    }

    // Update is called once per frame
    void Update()
    {
    }
}
