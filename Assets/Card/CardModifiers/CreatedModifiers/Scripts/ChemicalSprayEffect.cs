using UnityEngine;

public class ChemicalSprayEffect: CardModifier
{
    public int damage;
    public int num_of_turns;

    void Awake()
    {
        string display_description = this.description.Replace("XXX", this.damage.ToString());
        display_description = display_description.Replace("ZZZ", this.num_of_turns.ToString());
        SetDisplayDescription(display_description);
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
}
