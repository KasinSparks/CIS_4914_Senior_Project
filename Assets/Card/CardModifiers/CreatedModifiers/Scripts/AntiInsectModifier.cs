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
}
