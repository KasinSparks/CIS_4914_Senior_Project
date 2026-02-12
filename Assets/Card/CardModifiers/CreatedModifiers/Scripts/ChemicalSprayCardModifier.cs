using UnityEngine;

public class ChemicalSprayCardModifier : CardModifier
{
    public int damage;
    public int num_of_turns;

    void Awake()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.damage.ToString()));
        SetDisplayDescription(this.description.Replace("ZZZ", this.num_of_turns.ToString()));
        this.SetImage();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public void ApplyModifier(Card card, Card other)
    {
        // TODO(KASIN): This is untested, make sure it works as intended
        ChemicalSprayEffect sprayEffect = new ChemicalSprayEffect();
        sprayEffect.damage = this.damage;
        sprayEffect.num_of_turns = this.num_of_turns;
        sprayEffect.modifier_state = ModifierState.Applied;
        other.AttachModifier(sprayEffect);
        this.modifier_state = ModifierState.SetToReadyNextTurn;
    }

    override public void UpdateModifier(Card card)
    {

    }

    override public void UnapplyModifier(Card card, Card other)
    {
        this.modifier_state = ModifierState.ReadyToApply;
    }
}
