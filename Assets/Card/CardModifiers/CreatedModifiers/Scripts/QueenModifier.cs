using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @brief The modifier for Queen cards. Allows them to spawn a card each turn.
 */
[CreateAssetMenu(menuName = "Card/Modifier/Queen")]
public class QueenModifier : CardModifier
{
    public CardData spwan_card;

    private Hand hand;
    private Opponent opponent_ref;

    /**
     * @brief Set up this modifier before using
     */
    public override void Initialize()
    {
        Scene current_scene = SceneManager.GetActiveScene();
        if (current_scene != null &&
            (current_scene.name.Equals("Gameplay")))
        {
            this.hand = GameObject.Find("Hand").GetComponent<Hand>();
            this.opponent_ref = GameObject.Find("Opponent").GetComponent<Opponent>();
        }

        SetDisplayDescription(this.description.Replace("XXX", spwan_card.card_name));
    }

    override public void ApplyModifier(Card card, Card other)
    {
        switch (card.GetOwnership())
        {
            case CardOwnership.Player:
                // NOTE: The hand AddCard function handles the instantiation of
                //    the GameObject.
                this.hand.AddCard(this.spwan_card, card.GetOwnership());
                break;

            case CardOwnership.Opponent:
                // Create the card from the prefab, and add it to the opponent
                // hand.
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

    override public JsonValue ToJsonObject()
    {
        JsonObject base_obj = (JsonObject)base.ToJsonObject();

        System.Guid spawn_card_guid =
            SaveSystemTable.FindGuid(this.spwan_card.GetInstanceID());
        if (spawn_card_guid.Equals(System.Guid.Empty))
        {
            spawn_card_guid = SaveSystemTable.Add(this.spwan_card, this.spwan_card.GetInstanceID());
        }

        ((JsonObject)base_obj["Data"])["spawn_card"] =
            new JsonString() { value = spawn_card_guid.ToString() };

        return base_obj;
    }

    public override void OverrideValuesFromJson(JsonValue json)
    {
        base.OverrideValuesFromJson(json);
        JsonObject base_data = (JsonObject)json;
        System.Guid spawn_card_guid =
            System.Guid.Parse(((JsonString)base_data["spawn_card"]).value);
        this.spwan_card = SaveSystemTable.Get<CardData>(spawn_card_guid);
        
    }
}
