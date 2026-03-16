using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Chemical Spray")]
public class ChemicalSprayCardModifier : CardModifier
{
    public int damage;
    public int num_of_turns;

    public Texture2D spray_effect_image;

    public override void Initialize()
    {
        string display_description = this.description.Replace("XXX", this.damage.ToString());
        display_description = display_description.Replace("ZZZ", this.num_of_turns.ToString());
        SetDisplayDescription(display_description);
    }

    override public void ApplyModifier(Card card, Card other)
    {
        ChemicalSprayEffect sprayEffect =
            ScriptableObject.CreateInstance<ChemicalSprayEffect>();
        sprayEffect.modifier_name = "Chemical Spray Effect";
        sprayEffect.description = "Deals XXX damage over ZZZ turns.";
        sprayEffect.image = this.spray_effect_image;
        sprayEffect.modifier_type = ModifierType.OnTurnStart;
        sprayEffect.modifier_state = ModifierState.ReadyToApply;
        sprayEffect.damage = this.damage;
        sprayEffect.num_of_turns = this.num_of_turns;

        other.AttachModifier(sprayEffect);
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

        ((JsonObject)base_obj["Data"])["image"] =
            SaveSystemTable.GetJsonForTexture2D(this.image);

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

        this.image = SaveSystemTable.GetTexture2DFromJsonImage(
            ((JsonObject)base_data["image"])
        );
    }
}
