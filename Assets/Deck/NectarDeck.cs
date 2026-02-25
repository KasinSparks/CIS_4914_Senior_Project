using UnityEngine;
using UnityEngine.EventSystems;

public class NectarDeck : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardOwnership owner;
    [SerializeField] private CardData card;
    [SerializeField] private GameState gameState;
    [SerializeField] private Hand hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {}

    // Update is called once per frame
    void Update() {}

    /**
     * @brief When a user clicks on their deck, add a card from the deck to the
     * hand.
     * @param eventData
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.owner != CardOwnership.Player)
        {
            return;
        }

        // Check to see if the player is eligible to draw a card
        if (gameState.current_turn_state != TurnStates.PlayerDrawCard)
        {
            Debug.Log("Player can not draw a card if the game is not in the PlayerDrawCardState.");
            return;
        }

        // Send a card from the deck to the Hand Handler
        hand.AddCard(this.card, this.owner);

        // Player has drawn a card, unset the DrawCard state so the player can not draw another
        // card this turn.
        gameState.current_turn_state = TurnStates.PlayerTurn;
    }

}
