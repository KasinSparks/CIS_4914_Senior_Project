using UnityEngine;

public class DodgeCardModifier : CardModifier
{
    [Range(0.0f, 100.0f)]
    public float dodge_chance;

    void Awake()
    {
        SetDisplayDescription(this.description.Replace("XXX", this.dodge_chance.ToString()));
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
}
