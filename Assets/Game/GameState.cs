using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
public class GameState : MonoBehaviour
{
    public TurnStates current_turn_state;
    public AttackSystem attack_sys;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.UpdateTurnState(TurnStates.PlayerDrawCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        if (this.current_turn_state == TurnStates.PlayerDrawCard ||
            this.current_turn_state == TurnStates.PlayerTurn)
        {
            this.UpdateTurnState(TurnStates.PlayerEndTurn);
            UnityEngine.Debug.Log("Opponents Turn");
        }
        else
        {
            UnityEngine.Debug.Log("Wait until it is your turn again.");
        }
    }

    public void UpdateTurnState(TurnStates state)
    {
        switch (state)
        {
            case TurnStates.PlayerEndTurn:
                // Perform attacks of player's cards
                attack_sys.PlayerAttack();
                // Update the state
                this.current_turn_state = TurnStates.OpponentDrawCard;
                break;

            case TurnStates.OpponentEndTurn:
                // Perform attacks of opponent's cards
                attack_sys.OpponentAttack();
                // Update the state
                this.current_turn_state = TurnStates.PlayerDrawCard;
                break;

            default:
                this.current_turn_state = state;
                break;
        }
    }
}
