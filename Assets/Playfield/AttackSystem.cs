using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct CardHolderColumn
{
    public GameObject player_cardholder;
    public GameObject enemy_cardholder;
}

public class AttackSystem : MonoBehaviour
{
    // TODO(KASIN): get the cards currently on the playfield.
    // NOTE(KASIN): This may need to be changed once the code for the placement
    //     of cards gets pushed.
    
    const int NUM_OF_CARDS_IN_ROW = 4;
    // (Player cardholder, Enemy cardholder)
    //public (GameObject, GameObject)[] columns =
    //    new (GameObject, GameObject)[NUM_OF_CARDS_IN_ROW];
    public CardHolderColumn[] columns =
        new CardHolderColumn[NUM_OF_CARDS_IN_ROW];

    // BEGIN: FOR TESTING DELETE THIS
    public Card[] player_cards = new Card[NUM_OF_CARDS_IN_ROW];
    public Card[] opponent_cards = new Card[NUM_OF_CARDS_IN_ROW];
    // END: FOR TESTING DELETE THIS

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // BEGIN: FOR TESTING DELETE THIS
        for (int i = 0; i < NUM_OF_CARDS_IN_ROW; ++i)
        {
            if (player_cards[i] != null)
            {
                Transform player_card_holder_transform = columns[i].player_cardholder.transform;
                Card card_ref = Instantiate(player_cards[i], player_card_holder_transform);
                // TODO(KASIN): Hack, fix this
                card_ref.transform.localScale = new Vector3(
                    card_ref.transform.localScale.x * (1 / player_card_holder_transform.localScale.x),
                    card_ref.transform.localScale.y * (1 / player_card_holder_transform.localScale.z),
                    card_ref.transform.localScale.z * (1 / player_card_holder_transform.localScale.y)
                );
                card_ref.transform.position = new Vector3(
                    card_ref.transform.position.x,
                    card_ref.transform.position.y + 0.2f,
                    card_ref.transform.position.z
                );
                card_ref.SetOwnership(CardOwnership.Player);
            }

            if (opponent_cards[i] != null)
            {
                Transform opponent_card_holder_transform = columns[i].enemy_cardholder.transform;
                Card card_ref = Instantiate(opponent_cards[i], opponent_card_holder_transform);
                // TODO(KASIN): Hack, fix this
                card_ref.transform.localScale = new Vector3(
                    card_ref.transform.localScale.x * (1 / opponent_card_holder_transform.localScale.x),
                    card_ref.transform.localScale.y * (1 / opponent_card_holder_transform.localScale.z),
                    card_ref.transform.localScale.z * (1 / opponent_card_holder_transform.localScale.y)
                );
                card_ref.transform.position = new Vector3(
                    card_ref.transform.position.x,
                    card_ref.transform.position.y + 0.2f,
                    card_ref.transform.position.z
                );
                card_ref.SetOwnership(CardOwnership.Opponet);
            }
        }
        // END: FOR TESTING DELETE THIS
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Card GetCardFromHolder(GameObject holder)
    {
        Card card_ref = holder.transform.GetComponentInChildren<Card>();
        return card_ref;
    }

    public void PlayerAttack() {
        for (int i = 0; i < NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card player_card_ref = this.GetCardFromHolder(this.columns[i].player_cardholder);
            Card enemy_card_ref  = this.GetCardFromHolder(this.columns[i].enemy_cardholder);
            if (player_card_ref != null)
            {
                player_card_ref.Attack(enemy_card_ref);
            }
        }
    }

    public void OpponentAttack() {
        for (int i = 0; i < NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card player_card_ref = this.GetCardFromHolder(this.columns[i].player_cardholder);
            Card enemy_card_ref  = this.GetCardFromHolder(this.columns[i].enemy_cardholder);
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

        for (int i = 0; i < NUM_OF_CARDS_IN_ROW; ++i)
        {
            Card player_card_ref = this.GetCardFromHolder(this.columns[i].player_cardholder);
            Card enemy_card_ref  = this.GetCardFromHolder(this.columns[i].enemy_cardholder);
            if (player_card_ref != null && player_card_ref.GetOwnership() == CardOwnership.Player)
            {
                ret.Add(player_card_ref);
            }

            if (enemy_card_ref != null && enemy_card_ref.GetOwnership() == CardOwnership.Opponet)
            {
                ret.Add(enemy_card_ref);
            }
        }

        return ret;
    }
}
