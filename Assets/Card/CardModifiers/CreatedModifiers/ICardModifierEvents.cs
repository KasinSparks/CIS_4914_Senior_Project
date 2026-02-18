public interface ICardModifierEvents
{
    public void ApplyModifier(Card card, Card other);

    public void UpdateModifier(Card card);

    public void UnapplyModifier(Card card, Card other);
}