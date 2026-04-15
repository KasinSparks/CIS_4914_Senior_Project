using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/Armored")]
public class ArmoredCardModifier : CardModifier
{

    public int damage_reduction;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.damage_reduction.ToString()));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card._AddDefenseBonus(this.damage_reduction);
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        card._RemoveDefenseBonus(this.damage_reduction);
        this.modifier_state = ModifierState.ReadyToApply;
     
    }

    override public void SetData(CardModifier other)
    {
        this.SetData((ArmoredCardModifier) other);
    }

    public void SetData(ArmoredCardModifier other)
    {
        base.SetData(other);
        this.damage_reduction = other.damage_reduction;
    }

    public class ModifierSaveData : CardModifierSaveData
    {
        public int damage_reduction;

        public ModifierSaveData(ArmoredCardModifier modifier) : base(modifier)
        {
            this.damage_reduction = modifier.damage_reduction;
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
        this.damage_reduction = raw_save_data.damage_reduction;

        return ScriptableObject.Instantiate(this);
    }
}
