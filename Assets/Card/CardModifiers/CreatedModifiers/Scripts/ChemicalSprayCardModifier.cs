using Unity.VisualScripting;
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
        // TODO(KASIN): This is untested, make sure it works as intended
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
}
