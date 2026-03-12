using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Side Strike")]
public class SideStrikeModifier : CardModifier
{
    public override void Initialize()
    {
        SetDisplayDescription(this.description);
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card.SetHasSideStrike(true);
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card) {}

    override public void UnapplyModifier(Card card, Card other)
    {
        card.SetHasSideStrike(false);
        this.modifier_state = ModifierState.ReadyToApply;
     
    }

    override public void SetData(CardModifier other)
    {
        this.SetData(other);
    }
}
