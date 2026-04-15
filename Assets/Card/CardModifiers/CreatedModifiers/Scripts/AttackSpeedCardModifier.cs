using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Attack Speed")]
public class AttackSpeedCardModifier : CardModifier
{

    public int num_of_additional_attacks;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", (this.num_of_additional_attacks + 1).ToString()));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card._SetNumAdditionalAttacks(card._GetNumAdditionalAttacks()
            + this.num_of_additional_attacks);
        
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        card._SetNumAdditionalAttacks(card._GetNumAdditionalAttacks()
            - this.num_of_additional_attacks);

        this.modifier_state = ModifierState.ReadyToApply;
    }

    override public void SetData(CardModifier other)
    {
        base.SetData(other);
    }

    public void SetData(AttackSpeedCardModifier other)
    {
        base.SetData(other);
        this.num_of_additional_attacks = other.num_of_additional_attacks;
    }

    public class ModifierSaveData : CardModifierSaveData
    {
        public int num_of_additional_attacks;

        public ModifierSaveData(AttackSpeedCardModifier modifier) : base(modifier)
        {
            this.num_of_additional_attacks = modifier.num_of_additional_attacks;
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
        this.num_of_additional_attacks = raw_save_data.num_of_additional_attacks;

        return ScriptableObject.Instantiate(this);
    }
}
