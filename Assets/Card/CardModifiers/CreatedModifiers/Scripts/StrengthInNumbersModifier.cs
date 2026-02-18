using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Strength in Numbers")]
public class StrengthInNumberModifier : CardModifier
{
    private int num_of_hymenopteras = 0;
    private int old_num = 0;

    public override void Initialize()
    {
        SetDisplayDescription(this.description);
        //this.SetImage();
    }

    public void SetNumberOfHymenopteras(int num)
    {
        this.num_of_hymenopteras = num;
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card._RemoveAttackBonusDamage(old_num);
        old_num = num_of_hymenopteras;
        card._AddAttackBonusDamage(num_of_hymenopteras);
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
