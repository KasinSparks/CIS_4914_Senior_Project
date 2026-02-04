using UnityEngine;
using System.Collections.Generic;
using System;

public class Deck : MonoBehaviour
{
    public List<Card> cards;
    private Queue<Card> card_queue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.card_queue = new Queue<Card>();
        // Copy the starting cards to the queue
        this.Shuffle();

        foreach (Card c in this.card_queue)
        {
            Debug.Log(c);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    void OnMouseEnter()
    {
        
    }

    void OnMouseExit()
    {
        
    }

    void OnMouseDown()
    {
        
    }


    // Suffles all the cards in the deck then puts the result into the card_queue
    // TODO: Unit tests
    public void Shuffle()
    {
        // Uses the Durstenfeld's shuffle algorithm based on Fisher-Yates shuffle
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

        Card[] shuffled_array = cards.ToArray();
        for (int i = shuffled_array.Length - 1; i > 0; --i)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Card temp = shuffled_array[j];
            shuffled_array[j] = shuffled_array[i];
            shuffled_array[i] = temp;
        }

        // Add the newly shuffled cards into the queue
        this.card_queue.Clear();
        for (int i = 0; i < shuffled_array.Length; ++i)
        {
            this.card_queue.Enqueue(shuffled_array[i]);
        }
    }
    
    // Returns the next card in the card_queue, and removes it from the queue
    public Card GetNextCard()
    {
        throw new System.NotImplementedException();
    }

    // Returns the next card in the card_queue, but does not remove it from the queue
    public Card PeekNextCard()
    {
        throw new System.NotImplementedException();
    }

    public void AddCard(Card card)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveCard(Card card)
    {
        throw new System.NotImplementedException();
    }


}
