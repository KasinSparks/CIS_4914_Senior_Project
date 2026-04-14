using UnityEngine;

[CreateAssetMenu(menuName = "Card/Modifier/ChemicalSprayEffect")]
public class ChemicalSprayEffect: CardModifier
{
    public int damage;
    public int num_of_turns;

    public override void Initialize()
    {
        string display_description = this.description.Replace("XXX", this.damage.ToString());
        display_description = display_description.Replace("ZZZ", this.num_of_turns.ToString());
        SetDisplayDescription(display_description);
    }

    override public void ApplyModifier(Card card, Card other)
    {
        // Deal the damge
        card.DefendDirect(this.damage);
        this.num_of_turns -= 1;
        string display_description = this.description.Replace("XXX", this.damage.ToString());
        display_description = display_description.Replace("ZZZ", this.num_of_turns.ToString());
        SetDisplayDescription(display_description);

        // Expired
        if (this.num_of_turns <= 0)
        {
            card.RemoveModifier(this);
        }

        this.modifier_state = ModifierState.SetToReadyNextTurn;
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
        public int damage;
        public int num_of_turns;

        public ModifierSaveData(ChemicalSprayEffect modifier) : base(modifier)
        {
            this.damage       = modifier.damage;
            this.num_of_turns = modifier.num_of_turns;
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
        this.damage       = raw_save_data.damage;
        this.num_of_turns = raw_save_data.num_of_turns;

        return ScriptableObject.Instantiate(this);
    }
}
