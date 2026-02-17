using UnityEngine;

public class HPSystem : MonoBehaviour
{

    public int player_hp;
    public int opponent_hp;
    public bool player_dead = false;
    public bool opponent_dead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_hp = 10;
        opponent_hp = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Positive value for damage, negative for healing
    public int PlayerHPChange(int value)
    {
        player_hp -= value;

        if (player_hp < 0)
        {
            player_hp = 0;
            player_dead = true;
        }

        return player_hp;
    }

    // Positive value for damage, negative for healing
    public int OpponentHPChange(int value)
    {
        opponent_hp -= value;

        if (opponent_hp < 0) 
        {
            opponent_hp = 0;
            opponent_dead = true; 
        }

        return opponent_hp;
    }
}
