using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class AttackSystem : MonoBehaviour
{
    public Playfield playfield;
    private GameState game_state;

    private int attack_animation_status = 0;
    private int aas_opponent_offset = 4;

    void Awake()
    {
        // TODO(KASIN): Error handling
        this.game_state = GameObject.Find("GameState").GetComponent<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // CLEANUP(KASIN): 
        if ((this.attack_animation_status & (1 << 31)) != 0)
        {
            // Attack animation is active
            // Hold in attacking state until all animations finished.
            this.game_state.UpdateTurnState(TurnStates.Attacking);

            // Reset the state once all animation are done.
            if ((this.attack_animation_status & (0xF)) == 0XF)
            {
                // Done with attack animations
                this.attack_animation_status = 0;
                this.game_state.UpdateTurnState(TurnStates.OpponentDrawCard);
            }
        }
        else if ((this.attack_animation_status & (1 << 30)) != 0)
        {
            // Opponent attack animation is active
            // Hold in attacking state until all animations finished.
            this.game_state.UpdateTurnState(TurnStates.Attacking);

            // Reset the state once all animation are done.
            if ((this.attack_animation_status & ((0xF) << this.aas_opponent_offset)) == 0XF0)
            {
                // Done with attack animations
                this.attack_animation_status = 0;
                this.game_state.UpdateTurnState(TurnStates.PlayerDrawCard);
            }
        }
    }

    // CLEANUP(KASIN):
    IEnumerator AttackAnimation(Card card, Card opponent, int index)
    {
        Debug.Log("Get additional attack count: " + card._GetNumAdditionalAttacks());
        for (int a = 0; a < card._GetNumAdditionalAttacks(); ++a)
        {
            if (opponent == null)
            {
                // Update state
                this.attack_animation_status |= (1 << index);
                yield break;
            }

            Vector3 original_pos = card.transform.position;
            //Transform target = opponent.transform;
            Vector3 target = opponent.transform.position;
            target = new Vector3(
                target.x,
                target.y + .01f,
                target.z
            );
            //Transform original = card.transform; this is a moving point
            for (int i = 0; i < 175; ++i) //can change i to any number, i though 175 was a good time, the orginal 255 felt too slow
            {
                card.transform.position = Vector3.Lerp(
                    original_pos, //this orignally used orignal.position, which was always moving
                    target,
                    (float)i / 175f
                );
                yield return null;
            }
            card.transform.position = target; //snap to target, eliminate drifting

            card.Attack(opponent);


            for (int i = 0; i < 175; ++i)
            {
                if (card == null) {
                    // Update state
                    this.attack_animation_status |= (1 << index);
                    yield break;
                }
                card.transform.position = Vector3.Lerp(
                    target,
                    original_pos,
                    (float)i / 175f
                );
                yield return null;
            }
            card.transform.position = original_pos; //ensures no drifting
        }

        // Update state
        this.attack_animation_status |= (1 << index);
    }


    public void PlayerAttack() {
        // Set the animation status to start
        this.attack_animation_status |= (1 << 31);
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            CardSlot opponent_card_slot_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i];
            Card opponent_card_ref = opponent_card_slot_ref.GetCard();
            Card player_card_ref = playfield.GetCardSlots(CardOwnership.Player)[i].GetCard();

            if (player_card_ref != null)
            {
                //player_card_ref.Attack(opponent_card_ref);
                StartCoroutine(this.AttackAnimation(player_card_ref, opponent_card_ref, i));
            }
            else
            {
                // Update state
                this.attack_animation_status |= (1 << i);
            }
        }
    }

    public void OpponentAttack() {
        // Set the animation status to start
        this.attack_animation_status |= (1 << 30);
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card opponent_card_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i].GetCard();
            CardSlot player_card_slot_ref = playfield.GetCardSlots(CardOwnership.Player)[i];
            Card player_card_ref = player_card_slot_ref.GetCard();

            if (opponent_card_ref != null)
            {
                //enemy_card_ref.Attack(player_card_ref);
                StartCoroutine(this.AttackAnimation(opponent_card_ref, player_card_ref, i + aas_opponent_offset));
            }
            else
            {
                // Update state
                this.attack_animation_status |= (1 << (i + aas_opponent_offset));
            }
        }
    }

    public Card GetOppositeLeft() {
        throw new System.NotImplementedException();
    }

    public Card GetOppositeRight() {
        throw new System.NotImplementedException();
    }

    public List<Card> GetCards()
    {
        return this.GetCards(CardOwnership.Player | CardOwnership.Opponent);
    }

    public List<Card> GetCards(CardOwnership owner)
    {
        List<Card> ret = new List<Card>();

        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            switch (owner)
            {
                case (CardOwnership.Player):
                    Card player_card_ref = playfield.GetCardSlots(CardOwnership.Player)[i].GetCard();
                    if (player_card_ref != null && player_card_ref.GetOwnership() == CardOwnership.Player)
                    {
                        ret.Add(player_card_ref);
                    }
                    break;

                case (CardOwnership.Opponent):
                    Card opponent_card_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i].GetCard();
                    if (opponent_card_ref != null && opponent_card_ref.GetOwnership() == CardOwnership.Opponent)
                    {
                        ret.Add(opponent_card_ref);
                    }
                    break;
            }
        }

        return ret;
    }
}
