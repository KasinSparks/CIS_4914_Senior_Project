using System;
using UnityEngine;
using System.Collections.Generic;


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


    public void PlayerAttack() {
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            CardSlot opponent_card_slot_ref = playfield.GetCardSlots(CardOwnership.Opponent)[i];
            Card opponent_card_ref = opponent_card_slot_ref.GetCard();
            Card player_card_ref = playfield.GetCardSlots(CardOwnership.Player)[i].GetCard();

            if (player_card_ref != null)
            {
                player_card_ref.Attack(opponent_card_ref);

                // After player attacks, check for enemy card death and resets cardslot
                // This is called before card is deleted, so check with hp
                if (opponent_card_ref != null && opponent_card_ref.hp <= 0)
                {
                    Debug.Log("Opponent card died");
                    opponent_card_slot_ref.ResetCardSlot();
                }
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
                opponent_card_ref.Attack(player_card_ref);

                // After enemy attacks, check for player card death and resets cardslot
                // This is called before card is deleted, so check with hp
                if (player_card_ref != null && player_card_ref.hp <= 0)
                {
                    Debug.Log("Player card died");
                    player_card_slot_ref.ResetCardSlot();
                }
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
