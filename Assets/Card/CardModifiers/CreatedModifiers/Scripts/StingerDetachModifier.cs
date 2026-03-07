using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Stinger Detach")]
public class StingerDetachModifier : CardModifier
{
    public override void Initialize()
    {
        SetDisplayDescription(this.description);
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card.Death(null);
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card) {}

    override public void UnapplyModifier(Card card, Card other) {}

    override public void SetData(CardModifier other)
    {
        base.SetData(other);
    }
}
