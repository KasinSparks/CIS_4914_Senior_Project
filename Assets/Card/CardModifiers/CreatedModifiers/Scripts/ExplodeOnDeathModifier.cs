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
        other.DefendDirect(damage_on_death);
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
}
