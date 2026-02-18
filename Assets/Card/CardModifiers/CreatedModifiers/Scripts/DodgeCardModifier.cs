using UnityEngine;


[CreateAssetMenu(menuName = "Card/Modifier/Dodge")]
public class DodgeCardModifier : CardModifier
{
    [Range(0.0f, 100.0f)]
    public float dodge_chance;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.dodge_chance.ToString()));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card._AddDodgeChance(this.dodge_chance / 100.0f);
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        card._RemoveDodgeChance(this.dodge_chance / 100.0f);
        this.modifier_state = ModifierState.ReadyToApply;
     
    }

    override public void SetData(CardModifier other)
    {
        base.SetData(other);
    }
}
