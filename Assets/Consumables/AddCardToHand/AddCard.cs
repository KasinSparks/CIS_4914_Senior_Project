using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/AddCardToHand")]
public class AddCardToHandConsumable : ScriptableObject //in order to add any card, just instatiate a new scriptable object and drag in the desired card
{
    public string itemName;
    public Sprite icon;
    public CardData cardToAdd; //assign card, can be used for basic card like ant or flower card once added

    public void Use(Hand playerHand)
    {
        playerHand.AddCard(cardToAdd, CardOwnership.Player);
        Debug.Log("Added to hand: " + cardToAdd.card_name);
    }
}