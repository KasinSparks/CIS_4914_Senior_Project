using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/ChemicalSprayEffect")]
public class ChemicalSprayEffect: CardModifier
{
    public int damage;
    public int num_of_turns;

    public override void Initialize()
    {
        string display_description = this.description.Replace("XXX", this.damage.ToString());
        display_description = display_description.Replace("ZZZ", this.num_of_turns.ToString());
        SetDisplayDescription(display_description);
    }

    override public void ApplyModifier(Card card, Card other)
    {
        // Deal the damge
        card.DefendDirect(this.damage);
        this.num_of_turns -= 1;
        string display_description = this.description.Replace("XXX", this.damage.ToString());
        display_description = display_description.Replace("ZZZ", this.num_of_turns.ToString());
        SetDisplayDescription(display_description);

        // Expired
        if (this.num_of_turns <= 0)
        {
            card.RemoveModifier(this);
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

        ((JsonObject)base_obj["Data"])["damage"] =
            new JsonInt() { value = this.damage};

        ((JsonObject)base_obj["Data"])["num_of_turns"] =
            new JsonInt() { value = this.num_of_turns};

        return base_obj;
    }

    public override void OverrideValuesFromJson(JsonValue json)
    {
        base.OverrideValuesFromJson(json);
        JsonObject base_data = (JsonObject)json;
        this.damage =
            ((JsonInt)base_data["damage"]).value;

        this.num_of_turns =
            ((JsonInt)base_data["num_of_turns"]).value;
    }
}
