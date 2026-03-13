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
    // NOTE: If you use the delay, make sure it is longer than the animation or
    //    drifting will occur.
    IEnumerator AttackAnimation(Card card, CardSlot opponent_slot, int index, float delay = 0.0f)
    {
        Debug.Log("Get additional attack count: " + card._GetNumAdditionalAttacks());
        for (int a = 0; a < card._GetNumAdditionalAttacks(); ++a)
        {
            if (delay > 0.5f)
            {
                // Artificial delay. This is useful for the side strike.
                yield return new WaitForSeconds(delay);
            }

            if (opponent_slot == null)
            {
                // Update state
                this.attack_animation_status |= (1 << index);
                yield break;
            }

            Vector3 original_pos = card.transform.position;
            //Transform target = opponent.transform;
            Vector3 target = opponent_slot.transform.position;
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

            card.Attack(opponent_slot.GetCard());


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

    private void _Attack(CardOwnership owner)
    {
        // Set the animation status to start
        CardOwnership curr_target = CardOwnership.None;
        switch (owner)
        {
            case CardOwnership.Player:
                curr_target = CardOwnership.Opponent;
                this.attack_animation_status |= (1 << 31);
                break;
            case CardOwnership.Opponent:
                curr_target = CardOwnership.Player;
                this.attack_animation_status |= (1 << 30);
                break;
        }


        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card card_ref = playfield.GetCardSlots(owner)[i].GetCard();

            if (card_ref != null)
            {
                if (card_ref.GetHasSideStrike())
                {
                    int offset = i;

                    // Check LHS
                    CardSlot target_card_slot_ref = null;
                    try
                    {
                        target_card_slot_ref = playfield.GetCardSlots(curr_target)[i - 1];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Ignore
                    }

                    bool attcked_lhs = false;
                    Card target_card_ref = null;
                    if (target_card_slot_ref != null)
                    {
                        // Attack LHS
                        // Will update the bit flag outside the range for the player.
                        // Thus, the RHS animation will determine when the gamestate
                        // may continue.
                        target_card_ref = target_card_slot_ref.GetCard();
                        if (target_card_ref != null)
                        {
                            // This is so it does not trigger the gamestate to
                            // change before the RHS animation has a chance to
                            // run
                            offset += 4;
                            if (owner == CardOwnership.Opponent)
                            {
                                offset += aas_opponent_offset;
                            }

                            StartCoroutine(this.AttackAnimation(card_ref, target_card_slot_ref, offset));
                            playfield.AddLaneToAttackedList(target_card_slot_ref, curr_target);
                            attcked_lhs = true;
                        }
                    }


                    // Check RHS
                    target_card_slot_ref = null;
                    try
                    {
                        target_card_slot_ref = playfield.GetCardSlots(curr_target)[i + 1];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Ignore
                    }

                    // Attack RHS
                    target_card_ref = null;
                    if (target_card_slot_ref != null)
                    {
                        target_card_ref = target_card_slot_ref.GetCard();
                    }

                    float delay = 0.0f;
                    if (attcked_lhs)
                    {
                        // Delay for the LHS Attack animation to finish
                        delay = 2.0f;
                    }

                    offset = i;
                    if (owner == CardOwnership.Opponent)
                    {
                        offset += aas_opponent_offset;
                    }
                    StartCoroutine(this.AttackAnimation(card_ref, target_card_slot_ref, offset, delay));
                    if (target_card_slot_ref != null)
                    {
                        playfield.AddLaneToAttackedList(target_card_slot_ref, curr_target);
                    }
                }
                else
                {
                    // Normal attack
                    CardSlot target_card_slot_ref = playfield.GetCardSlots(curr_target)[i];
                    Card target_card_ref = target_card_slot_ref.GetCard();
                    int offset = i;
                    if (owner == CardOwnership.Opponent)
                    {
                        offset += aas_opponent_offset;
                    }
                    StartCoroutine(this.AttackAnimation(card_ref, target_card_slot_ref, offset));
                    playfield.AddLaneToAttackedList(target_card_slot_ref, curr_target);
                }
            }
            else
            {
                // Update state
                switch (owner)
                {
                    case CardOwnership.Player:
                        this.attack_animation_status |= (1 << i);
                        break;
                    case CardOwnership.Opponent:
                        this.attack_animation_status |= (1 << (i + aas_opponent_offset));
                        break;
                }
            }
        }
    }


    public void PlayerAttack() {
        this._Attack(CardOwnership.Player);
    }

    public void OpponentAttack() {
        this._Attack(CardOwnership.Opponent);
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
