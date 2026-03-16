using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Attack Speed")]
public class AttackSpeedCardModifier : CardModifier
{

    public int num_of_additional_attacks;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", (this.num_of_additional_attacks + 1).ToString()));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card._SetNumAdditionalAttacks(card._GetNumAdditionalAttacks()
            + this.num_of_additional_attacks);
        
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        card._SetNumAdditionalAttacks(card._GetNumAdditionalAttacks()
            - this.num_of_additional_attacks);

        this.modifier_state = ModifierState.ReadyToApply;
    }

    override public void SetData(CardModifier other)
    {
        base.SetData(other);
    }

    public void SetData(AttackSpeedCardModifier other)
    {

    }

    override public JsonValue ToJsonObject()
    {
        JsonObject base_obj = (JsonObject)base.ToJsonObject();

        ((JsonObject)base_obj["Data"])["num_of_additional_attacks"] =
            new JsonInt() { value = this.num_of_additional_attacks};

        return base_obj;
    }

    public override void OverrideValuesFromJson(JsonValue json)
    {
        base.OverrideValuesFromJson(json);
        JsonObject base_data = (JsonObject)json;
        this.num_of_additional_attacks =
            ((JsonInt)base_data["num_of_additional_attacks"]).value;
        
    }
}
