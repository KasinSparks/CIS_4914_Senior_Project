using UnityEngine;

public class ArmoredCardModifier : CardModifier
{

    public int damage_reduction;

    void Awake()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.damage_reduction.ToString()));
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
}
