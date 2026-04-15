using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Nektar Reduction")]
public class NektarReductionModifier : CardModifier
{

    public int nektar_reduction;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.nektar_reduction.ToString()));

        // Starts as an OnPlace mod, but will change to an OnDeath mod once placed.
        this.modifier_type = ModifierType.OnPlace;
    }

    override public void ApplyModifier(Card card, Card other)
    {
        switch (card.GetOwnership())
        {
            case CardOwnership.Player:
                card.GetHandRef().CurrentNektarReductionAdjustment(nektar_reduction);
                card.GetHandRef().UpdateCardsNektarCost();
                this.modifier_state = ModifierState.Applied;

                // Change the type to Passive so it unapplies when card dies
                this.modifier_type = ModifierType.Passive;
                break;
        }
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        switch (card.GetOwnership())
        {
            case CardOwnership.Player:
                card.GetHandRef().CurrentNektarReductionAdjustment(-nektar_reduction);
                card.GetHandRef().UpdateCardsNektarCost();
                this.modifier_state = ModifierState.ReadyToApply;
                break;
        }
    }

    override public void SetData(CardModifier other)
    {
        this.SetData((NektarReductionModifier) other);
    }

    public void SetData(NektarReductionModifier other)
    {
        base.SetData(other);
        this.nektar_reduction = other.nektar_reduction;
    }

    public class ModifierSaveData : CardModifierSaveData
    {
        public int nektar_reduction;

        public ModifierSaveData(NektarReductionModifier modifier) : base(modifier)
        {
            this.nektar_reduction = modifier.nektar_reduction;
        }

        public override CardModifierSaveData FromJson(string json)
        {
            return JsonUtility.FromJson<ModifierSaveData>(json);
        }
    }

    public override string ToJson()
    {
        return JsonUtility.ToJson(new ModifierSaveData(this), true);
    }

    public CardModifier _FromJson(string json)
    {
        ModifierSaveData raw_save_data =
            JsonUtility.FromJson<ModifierSaveData>(json);
        this.LoadBaseValuesFromSaveData(raw_save_data);
        this.nektar_reduction = raw_save_data.nektar_reduction;

        return ScriptableObject.Instantiate(this);
    }
}
