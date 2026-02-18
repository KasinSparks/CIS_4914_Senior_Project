using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class AttackSystem : MonoBehaviour
{
    public Playfield playfield;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AttackAnimation(Card card, Card opponent)
    {
        Debug.Log("Get additional attack count: " + card._GetNumAdditionalAttacks());
        for (int a = 0; a < card._GetNumAdditionalAttacks(); ++a)
        {
            if (opponent == null)
            {
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
            Transform original = card.transform;
            for (int i = 0; i < 255; ++i)
            {
                card.transform.position = Vector3.Lerp(
                    original.position,
                    target,
                    (float)1 / 64
                );
                yield return null;
            }

            card.Attack(opponent);


            for (int i = 0; i < 255; ++i)
            {
                if (card == null) { yield break; }
                card.transform.position = Vector3.Lerp(
                    original.position,
                    original_pos,
                    (float)1 / 64
                );
                yield return null;
            }
        }
    }

    public void PlayerAttack() {
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            CardSlot opponent_card_slot_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i];
            Card opponent_card_ref = opponent_card_slot_ref.GetCard();
            Card player_card_ref = playfield.GetCardSlots(CardOwnership.Player)[i].GetCard();

            if (player_card_ref != null)
            {
                //player_card_ref.Attack(opponent_card_ref);
                StartCoroutine(this.AttackAnimation(player_card_ref, opponent_card_ref));
            }

        }
    }

    public void OpponentAttack() {
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card opponent_card_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i].GetCard();
            CardSlot player_card_slot_ref = playfield.GetCardSlots(CardOwnership.Player)[i];
            Card player_card_ref = player_card_slot_ref.GetCard();

            if (opponent_card_ref != null)
            {
                //enemy_card_ref.Attack(player_card_ref);
                StartCoroutine(this.AttackAnimation(opponent_card_ref, player_card_ref));
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
