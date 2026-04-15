using UnityEngine;


[CreateAssetMenu(menuName = "Card/Modifier/Dodge")]
public class DodgeCardModifier : CardModifier
{
    [Range(0.0f, 100.0f)]
    public float dodge_chance;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.dodge_chance.ToString()));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        card._AddDodgeChance(this.dodge_chance / 100.0f);
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        card._RemoveDodgeChance(this.dodge_chance / 100.0f);
        this.modifier_state = ModifierState.ReadyToApply;
     
    }

    override public void SetData(CardModifier other)
    {
        base.SetData(other);
    }

    public class ModifierSaveData : CardModifierSaveData
    {
        public float dodge_chance;

        public ModifierSaveData(DodgeCardModifier modifier) : base(modifier)
        {
            this.dodge_chance = modifier.dodge_chance;
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
        this.dodge_chance = raw_save_data.dodge_chance;

        return ScriptableObject.Instantiate(this);
    }
}
