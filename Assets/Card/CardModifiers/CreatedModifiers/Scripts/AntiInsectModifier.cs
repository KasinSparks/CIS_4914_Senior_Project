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

    public class ModifierSaveData : CardModifierSaveData
    {
        public int additional_damage;

        public ModifierSaveData(AntiInsectModifier modifier) : base(modifier)
        {
            this.additional_damage = modifier.addtional_damage;
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
        this.addtional_damage = raw_save_data.additional_damage;

        return ScriptableObject.Instantiate(this);
    }
}
