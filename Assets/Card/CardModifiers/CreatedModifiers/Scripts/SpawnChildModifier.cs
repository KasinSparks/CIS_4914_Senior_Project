using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Card/Modifier/Spawn Child")]
public class SpawnChildModifier : CardModifier
{
    private Playfield playfield;

    [SerializeField]
    private CardData leave_behind_card;

    public override void Initialize()
    {
        if (this.leave_behind_card == null)
        {
            SetDisplayDescription(this.description);
        }
        else
        {
            SetDisplayDescription(this.description.Replace("XXX", this.leave_behind_card.card_name));
        }
        
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            this.playfield = GameObject.Find("Playfield").GetComponent<Playfield>();
        }
    }

    override public void ApplyModifier(Card card, Card other)
    {
        CardSlot original_slot = card.GetSlot();

        Card new_card = Instantiate<Card>(Resources.Load<Card>("Card"), playfield.transform);
        new_card.SetCardData(leave_behind_card);
        original_slot.SetCard(new_card);
        original_slot.SetIsCardPlaced(true);
        new_card.SetSlot(original_slot);
        new_card.SetState(CardState.OnPlayfield);
        new_card.SetOwnership(card.GetOwnership());

        new_card.transform.SetPositionAndRotation(
            new Vector3(original_slot.transform.position.x,
            original_slot.transform.position.y + 0.0001f * new_card.transform.localScale.x,
            original_slot.transform.position.z),
            Quaternion.Euler(0, 0, 0)
        );
        float card_scale = playfield.GetCardScaleAmount();
        new_card.transform.localScale = new Vector3(card_scale, card_scale, card_scale);

        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card) {}

    override public void UnapplyModifier(Card card, Card other)
    {
        this.modifier_state = ModifierState.ReadyToApply;
    }

    override public JsonValue ToJsonObject()
    {
        JsonObject base_obj = (JsonObject)base.ToJsonObject();

        System.Guid leave_behind_card_guid =
            SaveSystemTable.FindGuid(this.leave_behind_card.GetInstanceID());
        if (leave_behind_card_guid.Equals(System.Guid.Empty))
        {
            leave_behind_card_guid = SaveSystemTable.Add(this.leave_behind_card,
                this.leave_behind_card.GetInstanceID());
        }

        ((JsonObject)base_obj["Data"])["leave_behind_card"] =
            new JsonString() { value = leave_behind_card_guid.ToString() };

        return base_obj;
    }

    public override void OverrideValuesFromJson(JsonValue json)
    {
        JsonObject base_data = (JsonObject)json;
        System.Guid leave_behind_card_guid =
            System.Guid.Parse(((JsonString)base_data["leave_behind_card"]).value);
        this.leave_behind_card = SaveSystemTable.Get<CardData>(leave_behind_card_guid);
        
        base.OverrideValuesFromJson(json);
    }
}
