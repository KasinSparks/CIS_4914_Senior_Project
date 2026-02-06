using UnityEngine;

public class GameState : MonoBehaviour
{
    public TurnStates current_turn_state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.current_turn_state = TurnStates.PlayerDrawCard;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
