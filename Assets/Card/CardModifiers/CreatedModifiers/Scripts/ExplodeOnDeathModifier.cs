using UnityEngine;


[CreateAssetMenu(menuName = "Card/Modifier/Explode on Death")]
public class ExplodeOnDeathModifier : CardModifier
{
    public int damage_on_death;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.damage_on_death.ToString()));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        if (other != null)
        {
            other.DefendDirect(damage_on_death);
        }
        this.modifier_state = ModifierState.Applied;
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

        ((JsonObject)base_obj["Data"])["damage_on_death"] =
            new JsonInt() { value = this.damage_on_death};

        return base_obj;
    }

    public override void OverrideValuesFromJson(JsonValue json)
    {
        base.OverrideValuesFromJson(json);
        JsonObject base_data = (JsonObject)json;
        this.damage_on_death =
            ((JsonInt)base_data["damage_on_death"]).value;
        
    }
}
