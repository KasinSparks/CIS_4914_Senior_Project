using UnityEngine;

public class AttackSpeedCardModifier : CardModifier
{

    public int num_of_additional_attacks;

    void Awake()
    {
        SetDisplayDescription(this.description.Replace("XXX", (this.num_of_additional_attacks + 1).ToString()));
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
}
