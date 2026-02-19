using System;
using UnityEngine;
using System.Collections.Generic;

//[Serializable]
//public struct CardHolderColumn
//{
//    public CardSlot player_cardholder;
//    public GameObject opponent_cardholder;
//}

public class AttackSystem : MonoBehaviour
{
    // TODO(KASIN): get the cards currently on the playfield.
    // NOTE(KASIN): This may need to be changed once the code for the placement
    //     of cards gets pushed.

    public Playfield playfield;

    // (Player cardholder, Enemy cardholder)
    //public (GameObject, GameObject)[] columns =
    //    new (GameObject, GameObject)[NUM_OF_CARDS_IN_ROW];
    //public CardHolderColumn[] columns;

    // BEGIN: FOR TESTING DELETE THIS
    //public Card[] opponent_cards = new Card[Playfield.NUM_OF_CARDS_IN_ROW];
    //public Card[] current_opponent_cards = new Card[Playfield.NUM_OF_CARDS_IN_ROW];
    // END: FOR TESTING DELETE THIS


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // BEGIN: FOR TESTING DELETE THIS
        //for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        //{
        //    if (opponent_cards[i] != null)
        //    {
        //        Transform opponent_card_holder_transform = columns[i].enemy_cardholder.transform;
        //        Card card_ref = Instantiate(opponent_cards[i]);
        //        card_ref.transform.localScale = new Vector3(
        //            0.1f * card_ref.transform.localScale.x,
        //            0.1f * card_ref.transform.localScale.y,
        //            0.1f * card_ref.transform.localScale.z
        //        );
        //        card_ref.transform.SetPositionAndRotation(
        //            opponent_card_holder_transform.position,
        //            Quaternion.Euler(0, 180, 90)
        //        );
        //        card_ref.SetOwnership(CardOwnership.Opponent);
        //        current_opponent_cards[i] = card_ref;
        //    }
        //}
        // END: FOR TESTING DELETE THIS
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
                if (opponent_card_ref != null && opponent_card_ref.hp <= 0)
                {
                    opponent_card_slot_ref.ResetCardSlot();
                }
            }

            //Card enemy_card_ref  = this.current_opponent_cards[i];
            //Card player_card_ref = this.columns[i].player_cardholder.GetCard();
            //if (player_card_ref != null)
            //{
            //    player_card_ref.Attack(enemy_card_ref);
            //}
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
                if (player_card_ref != null && player_card_ref.hp <= 0)
                {
                    player_card_slot_ref.ResetCardSlot();
                }
            }

            //Card enemy_card_ref  = this.current_opponent_cards[i];
            //CardSlot player_card_slot_ref = this.columns[i].player_cardholder;
            //Card player_card_ref = player_card_slot_ref.GetCard();
            //if (enemy_card_ref != null)
            //{
            //    enemy_card_ref.Attack(player_card_ref);

            //    // After enemy attacks, check for player card death and resets cardslot
            //    if (player_card_ref != null && player_card_ref.hp <= 0)
            //    {
            //        player_card_slot_ref.ResetCardSlot();
            //    }
            //}
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

            //Card player_card_ref = this.columns[i].player_cardholder.GetCard();
            //if (player_card_ref != null && player_card_ref.GetOwnership() == CardOwnership.Player)
            //{
            //    ret.Add(player_card_ref);
            //}

            //Card enemy_card_ref  = this.current_opponent_cards[i];
            //if (enemy_card_ref != null && enemy_card_ref.GetOwnership() == CardOwnership.Opponent)
            //{
            //    ret.Add(enemy_card_ref);
            //}
        }

        return ret;
    }
}
