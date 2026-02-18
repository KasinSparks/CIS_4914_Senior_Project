using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Heal on Attack")]
public class HealOnAttackModifier : CardModifier
{

    public override void Initialize()
    {
        SetDisplayDescription(this.description);
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card.SetCurrentHP(card.GetCurrentHP() + card.GetBaseAttack());
        this.modifier_state = ModifierState.ReadyToApply;
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
