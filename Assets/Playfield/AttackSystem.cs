using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct CardHolderColumn
{
    public CardSlot player_cardholder;
    public GameObject enemy_cardholder;
}

public class AttackSystem : MonoBehaviour
{
    // TODO(KASIN): get the cards currently on the playfield.
    // NOTE(KASIN): This may need to be changed once the code for the placement
    //     of cards gets pushed.

    public Playfield playfield;

    // (Player cardholder, Enemy cardholder)
    //public (GameObject, GameObject)[] columns =
    //    new (GameObject, GameObject)[NUM_OF_CARDS_IN_ROW];
    public CardHolderColumn[] columns;

    // BEGIN: FOR TESTING DELETE THIS
    public Card[] opponent_cards = new Card[Playfield.NUM_OF_CARDS_IN_ROW];
    public Card[] current_opponent_cards = new Card[Playfield.NUM_OF_CARDS_IN_ROW];
    // END: FOR TESTING DELETE THIS


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // BEGIN: FOR TESTING DELETE THIS
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            if (opponent_cards[i] != null)
            {
                Transform opponent_card_holder_transform = columns[i].enemy_cardholder.transform;
                Card card_ref = Instantiate(opponent_cards[i]);
                card_ref.transform.localScale = new Vector3(
                    0.1f * card_ref.transform.localScale.x,
                    0.1f * card_ref.transform.localScale.y,
                    0.1f * card_ref.transform.localScale.z
                );
                card_ref.transform.SetPositionAndRotation(
                    opponent_card_holder_transform.position,
                    Quaternion.Euler(0, 180, 90)
                );
                card_ref.SetOwnership(CardOwnership.Opponet);
                current_opponent_cards[i] = card_ref;
            }
        }
        // END: FOR TESTING DELETE THIS
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayerAttack() {
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card enemy_card_ref  = this.current_opponent_cards[i];
            Card player_card_ref = this.columns[i].player_cardholder.GetCard();
            if (player_card_ref != null)
            {
                player_card_ref.Attack(enemy_card_ref);
            }
        }
    }

    public void OpponentAttack() {
        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card enemy_card_ref  = this.current_opponent_cards[i];
            Card player_card_ref = this.columns[i].player_cardholder.GetCard();
            if (enemy_card_ref != null)
            {
                enemy_card_ref.Attack(player_card_ref);
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
        return this.GetCards(CardOwnership.Player | CardOwnership.Opponet);
    }

    public List<Card> GetCards(CardOwnership owner)
    {
        List<Card> ret = new List<Card>();

        for (int i = 0; i < Playfield.NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card player_card_ref = this.columns[i].player_cardholder.GetCard();
            if (player_card_ref != null && player_card_ref.GetOwnership() == CardOwnership.Player)
            {
                ret.Add(player_card_ref);
            }

            Card enemy_card_ref  = this.current_opponent_cards[i];
            if (enemy_card_ref != null && enemy_card_ref.GetOwnership() == CardOwnership.Opponet)
            {
                ret.Add(enemy_card_ref);
            }
        }

        return ret;
    }
}
