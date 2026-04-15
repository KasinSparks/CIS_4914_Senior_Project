using UnityEngine;


[CreateAssetMenu(menuName = "Card/Modifier/Explode on Death")]
public class ExplodeOnDeathModifier : CardModifier
{
    public int damage_on_death;

    public override void Initialize()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.damage_on_death.ToString()));
        //this.SetImage();
    }

    override public void ApplyModifier(Card card, Card other)
    {
        if (other != null)
        {
            other.DefendDirect(damage_on_death);
        }
        this.modifier_state = ModifierState.Applied;
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

    public class ModifierSaveData : CardModifierSaveData
    {
        public int damage_on_death;

        public ModifierSaveData(ExplodeOnDeathModifier modifier) : base(modifier)
        {
            this.damage_on_death = modifier.damage_on_death;
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
        this.damage_on_death = raw_save_data.damage_on_death;

        return ScriptableObject.Instantiate(this);
    }
}
