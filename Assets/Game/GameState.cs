using UnityEngine;
using System.Collections.Generic;

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

    public void EndOpponetTurn()
    {
        if (this.current_turn_state == TurnStates.OpponentDrawCard ||
            this.current_turn_state == TurnStates.OpponentTurn)
        {
            this.UpdateTurnState(TurnStates.OpponentEndTurn);
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
                {
                    this.current_turn_state = TurnStates.PlayerEndTurn;
                    // Perform attacks of player's cards
                    attack_sys.PlayerAttack();
                    // Update the state
                    this.UpdateTurnState(TurnStates.OpponentDrawCard);
                }
                break;

            case TurnStates.OpponentEndTurn:
                {
                    this.current_turn_state = TurnStates.OpponentEndTurn;
                    // Perform attacks of opponent's cards
                    attack_sys.OpponentAttack();
                    // Update the state
                    this.UpdateTurnState(TurnStates.PlayerDrawCard);
                }
                break;

            case TurnStates.PlayerDrawCard:
                {
                    this.current_turn_state = TurnStates.PlayerDrawCard;
                    List<Card> player_cards = attack_sys.GetCards(CardOwnership.Player);
                    for (int i = 0; i < player_cards.Count; ++i)
                    {
                        player_cards[i].OnTurnStart();
                    }
                }
                break;

            case TurnStates.OpponentDrawCard:
                {
                    this.current_turn_state = TurnStates.OpponentDrawCard;
                    List<Card> opponent_cards = attack_sys.GetCards(CardOwnership.Opponet);
                    for (int i = 0; i < opponent_cards.Count; ++i)
                    {
                        opponent_cards[i].OnTurnStart();
                    }
                }
                break;

            default:
                this.current_turn_state = state;
                break;
        }
    }
}
