using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Anti Insect")]
public class AntiInsectModifier : CardModifier
{

    public int addtional_damage;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.addtional_damage.ToString()));
    }

    override public void ApplyModifier(Card card, Card other)
    {
        if (other != null)
        {
            card._AddAttackBonusDamage(addtional_damage);
        }
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        
        if (other != null)
        {
            card._AddAttackBonusDamage(-addtional_damage);
        }

        this.modifier_state = ModifierState.ReadyToApply;
    }

    override public void SetData(CardModifier other)
    {
        base.SetData(other);
    }

    public void SetData(AntiInsectModifier other)
    {
        base.SetData(other);
        this.addtional_damage = other.addtional_damage;
    }

    override public JsonValue ToJsonObject()
    {
        JsonObject base_obj = (JsonObject)base.ToJsonObject();

        ((JsonObject)base_obj["Data"])["additional_damage"] =
            new JsonInt() { value = this.addtional_damage};

        return base_obj;
    }

    public override void OverrideValuesFromJson(JsonValue json)
    {
        JsonObject base_data = (JsonObject)json;
        this.addtional_damage =
            ((JsonInt)base_data["additional_damage"]).value;
        
        base.OverrideValuesFromJson(json);
    }
}
