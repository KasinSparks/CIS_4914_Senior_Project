using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[CreateAssetMenu(menuName = "Card/Modifier/Queen")]
public class QueenModifier : CardModifier
{
    public CardData spwan_card;

    private Hand hand;
    private Opponent opponent_ref;

    public override void Initialize()
    {
        // TODO(KASIN): Fix when we get enemy working
        Scene current_scene = SceneManager.GetActiveScene();
        if (current_scene != null &&
            (current_scene.name.Equals("Gameplay")))
        {
            this.hand = GameObject.Find("Hand").GetComponent<Hand>();
            this.opponent_ref = GameObject.Find("Opponent").GetComponent<Opponent>();
        }

        SetDisplayDescription(this.description.Replace("XXX", spwan_card.card_name));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        switch (card.GetOwnership())
        {
            case CardOwnership.Player:
                this.hand.AddCard(this.spwan_card, card.GetOwnership());
                break;

            case CardOwnership.Opponent:
                Card card_prefab = Resources.Load<Card>("Card");
                Card new_card = Instantiate(card_prefab, opponent_ref.gameObject.transform);
                new_card.SetCardData(this.spwan_card);
                new_card.gameObject.SetActive(false);
                new_card.SetState(CardState.InHand);
                new_card.SetOwnership(CardOwnership.Opponent);
                opponent_ref.GetHand().Add(new_card);
                break;

            default:
                throw new System.Exception("Tried to apply the Queen modifier to a card with neither player or opponent ownership.");
        }
        this.modifier_state = ModifierState.SetToReadyNextTurn;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        this.modifier_state = ModifierState.ReadyToApply;
     
    }

    override public void SetData(CardModifier other)
    {
        base.SetData(other);
    }
}
