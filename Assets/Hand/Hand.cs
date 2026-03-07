using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{

    public List<Card> cards;

    public GameObject card_starting_mark;
    public GameObject card_ending_mark;

    private const float MAX_CARD_FLARE_ANGLE = 140;

    private Card front_card;
    private float front_card_original_z;

    

    private Card current_card_selected;
    private const float RAISE_AMT = 0.05f;
    
    private int current_nektar_reduction;

    [SerializeField] private Playfield playfield;
    [SerializeField] private Totem totem;

    private void Awake()
    {
        this.current_card_selected = null;
        this.current_nektar_reduction = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cards = new List<Card>();
        front_card = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCardsNektarCost()
    {
        foreach (Card card in cards)
        {
            card.SetNektarAmountAdjustment(current_nektar_reduction);
            card.UpdateCardTextStats();
        }
    }

    private void AddCardHelper(Card card, CardOwnership owner)
    {
        // Update the newly instantiated card's fields 
        card.gameObject.SetActive(true);
        card.SetState(CardState.InHand);
        card.SetOwnership(owner);

        // adds totem modifier temporarily (won't be saved to card)
        this.totem.AttachModifier(card);

        // Add the card to the hand
        this.cards.Add(card);

        // Move card to position
        this.FlareCards();

        this.UpdateCardsNektarCost();
    }

    public void AddCard(Card card, CardOwnership owner)
    {
        // Instantiate card and add it to the hand list
        Card new_card = Instantiate(card, this.transform);

        this.AddCardHelper(new_card, owner);
    }

    public void AddCard(CardData card_data, CardOwnership owner)
    {
        // Instantiate card and add it to the hand list
        Card card_prefab = Resources.Load<Card>("Card");
        Card new_card = Instantiate(card_prefab, this.transform);
        new_card.SetCardData(card_data);

        this.AddCardHelper(new_card, owner);
    }

    public void RemoveCard(Card card)
    {
        this.cards.Remove(card);
        if (card == this.front_card)
        {
            this.front_card = null;
        }
        //card.gameObject.SetActive(false);
        card.SetState(CardState.OnPlayfield);
        // Activate the OnPlace modifiers, if any are attached.
        card.Placed();
        //throw new System.NotImplementedException();

        this.UpdateCardsNektarCost();
    }

    public void MoveCardFront(Card card)
    {
        if (this.front_card != null)
        {
            // Move the current front card back
            this.front_card.transform.SetPositionAndRotation(
                new Vector3(
                    this.front_card.transform.position.x,
                    this.front_card.transform.position.y,
                    this.front_card_original_z),
                this.front_card.transform.rotation
            );
        }

        // Move the new front card forward
        this.front_card = card;
        this.front_card_original_z = this.front_card.transform.position.z;
        
        this.front_card.transform.SetPositionAndRotation(
            new Vector3(
                this.front_card.transform.position.x,
                this.front_card.transform.position.y,
                this.card_starting_mark.transform.position.z - 0.01f * this.front_card.transform.parent.transform.localScale.x),
            this.front_card.transform.rotation
        );
    }

    private void FlareCards()
    {
        Vector3 heading = this.card_ending_mark.transform.position
            - this.card_starting_mark.transform.position;

        float distance = heading.magnitude;

        // Divide spacing equally between the cards in the hand
        float spacing = distance / this.cards.Count;

        // Unit vector of the distance vector between start and end marks
        Vector3 unit_vector = heading / distance;

        // Go through each card an update its' position.
        for (int i = 0; i < this.cards.Count; ++i)
        {
            Vector3 pos = this.card_starting_mark.transform.position + (unit_vector * spacing * i);

            pos.y += i * 0.002f; //so cards dont clip
            this.cards[i].transform.position = pos;
        }

        this.front_card = this.cards[0];
        this.front_card_original_z = this.front_card.transform.position.z;
        this.current_card_selected = null;
    }

    public void SetSelectedCard(CardOwnership owner, Card selected_card)
    {
        if (owner != CardOwnership.Player) return;
        if (SceneManager.GetActiveScene().name == "Sacrafice" || SceneManager.GetActiveScene().name == "Campfire" || SceneManager.GetActiveScene().name == "Shop") return; //to stop errors from being thrown
        
        if (this.current_card_selected != null &&
            this.current_card_selected.card_state == CardState.InHand)
        {
            // Reset the old selected card's position
            this.current_card_selected.transform.position = new Vector3(
                this.current_card_selected.transform.position.x,
                this.current_card_selected.transform.position.y - RAISE_AMT,
                this.current_card_selected.transform.position.z
            );
        }
        
        playfield.SetHand(owner, this);
        playfield.SetSelectedCard(owner, selected_card);

        this.current_card_selected = selected_card;
        if (selected_card == null)
        {
            return;
        }

        // Raise the selected card
        this.current_card_selected.transform.position = new Vector3(
            this.current_card_selected.transform.position.x,
            this.current_card_selected.transform.position.y + RAISE_AMT,
            this.current_card_selected.transform.position.z
        );

        playfield.SetCurrentSacrificeCost(selected_card.GetNektarCost());
    }

    public void AddCardForUpgrade(CardData card_data) //this allows gamestate to not exist when upgrading card
    {
        Card card_prefab = Resources.Load<Card>("Card");
        Card new_card = Instantiate(card_prefab, this.transform);
        new_card.SetContext(Card.CardContext.Upgrade);
        new_card.SetCardData(card_data);
        AddCardHelper(new_card, CardOwnership.Player);
    }

    public void CurrentNektarReductionAdjustment(int amt)
    {
        this.current_nektar_reduction += amt;
        Debug.Log(this.current_nektar_reduction);
    }
}
