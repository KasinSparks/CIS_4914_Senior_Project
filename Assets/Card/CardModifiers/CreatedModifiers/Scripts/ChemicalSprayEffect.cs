using UnityEngine;

public class ChemicalSprayEffect: CardModifier
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
        this.modifier_state = ModifierState.Applied;
    }

    override public void UpdateModifier(Card card)
    {
        // Deal the damge
        card.DefendDirect(this.damage);
        this.num_of_turns -= 1;
        SetDisplayDescription(this.description.Replace("ZZZ", this.num_of_turns.ToString()));

        // Expired
        if (this.num_of_turns <= 0)
        {
            card.RemoveModifier(this);
        }
    }

    override public void UnapplyModifier(Card card, Card other)
    {
        this.modifier_state = ModifierState.ReadyToApply;
     
    }
}
