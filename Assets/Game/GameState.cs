using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public TurnStates current_turn_state;
    public AttackSystem attack_sys;
    public Opponent opponent;

    [SerializeField]
    private Playfield playfield;

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
    
    // TODO(KASIN): Make sure the recurrsion in the function is not going to
    //    cause a runtime error or crash.
    public void UpdateTurnState(TurnStates state)
    {
        switch (state)
        {
            case TurnStates.PlayerTurn:
                {
                    this.current_turn_state = TurnStates.PlayerTurn;
                    // Perform attacks of player's cards
                    // GameState gets updated in AttackSystem
                }
                break;
            case TurnStates.PlayerEndTurn:
                {
                    this.current_turn_state = TurnStates.PlayerEndTurn;
                    // Perform attacks of player's cards
                    // GameState gets updated in AttackSystem
                    attack_sys.PlayerAttack();
                }
                break;

            case TurnStates.OpponentEndTurn:
                {
                    this.current_turn_state = TurnStates.OpponentEndTurn;
                    // Perform attacks of opponent's cards
                    // GameState gets updated in AttackSystem
                    attack_sys.OpponentAttack();
                }
                break;

            case TurnStates.PlayerDrawCard:
                {
                    if (this.current_turn_state == TurnStates.PlayerSacrifice)
                    {
                        // Don't reload the on turn start modifiers
                        this.current_turn_state = TurnStates.PlayerDrawCard;
                        break;
                    }

                    this.current_turn_state = TurnStates.PlayerDrawCard;
                    List<Card> player_cards = attack_sys.GetCards(CardOwnership.Player);
                    for (int i = 0; i < player_cards.Count; ++i)
                    {
                        Debug.Log("Owner: " + player_cards[i].GetOwnership());
                        player_cards[i].OnTurnStart();
                    }
                    playfield.ResetLanesAttacked(CardOwnership.Player);
                }
                break;

            case TurnStates.OpponentDrawCard:
                {
                    opponent.DrawCards();
                    this.current_turn_state = TurnStates.OpponentDrawCard;
                    List<Card> opponent_cards = attack_sys.GetCards(CardOwnership.Opponent);
                    for (int i = 0; i < opponent_cards.Count; ++i)
                    {
                        opponent_cards[i].OnTurnStart();
                    }
                    this.UpdateTurnState(TurnStates.OpponentTurn);
                    playfield.ResetLanesAttacked(CardOwnership.Opponent);
                }
                break;
            case TurnStates.OpponentTurn:
                {
                    opponent.Turn();
                    this.current_turn_state = TurnStates.OpponentTurn;
                    this.UpdateTurnState(TurnStates.OpponentEndTurn);
                }
                break;

            default:
                this.current_turn_state = state;
                break;
        }
    }

    public TurnStates GetCurrentState()
    {
        return this.current_turn_state;
    }
}
