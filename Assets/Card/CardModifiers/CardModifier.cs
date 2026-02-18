using UnityEngine;

public abstract class CardModifier : ScriptableObject, ICardModifierEvents
{
    public string modifier_name;
    public string description;

    public Texture2D image;

    public ModifierType modifier_type;

    public ModifierState modifier_state;

    protected string display_description;

    public abstract void Initialize();

    protected void SetDisplayDescription(string description)
    {
        this.display_description = description;
    }

    public string GetDisplayDescription()
    {
        return this.display_description;
    }

    public Texture2D GetImage()
    {
        return this.image;
    }

    public ModifierType GetModifierType()
    {
        return this.modifier_type;
    }

    public ModifierState GetModifierState()
    {
        return this.modifier_state;
    }

    public string GetName() {
        return this.modifier_name;
    }

    public string GetDescription()
    {
        return this.description;
    }

    // Compare the name, description, and modifier_type
    public bool Compare(CardModifier other)
    {
        if (!(this.modifier_name.Equals(other.GetName())))
        {
            return false;
        }

        if (!(this.description.Equals(other.GetDescription())))
        {
            return false;
        }

        if (!(this.modifier_type.Equals(other.GetModifierType())))
        {
            return false;
        }

        return true;
    }

    public virtual void SetData(CardModifier other)
    {
        this.modifier_name = other.GetName();
        this.description = other.GetDescription();
        this.image = other.GetImage();
        this.modifier_state = other.modifier_state;
        this.modifier_type = other.modifier_type;
    }

    public abstract void ApplyModifier(Card card, Card other);

    public abstract void UpdateModifier(Card card);

    public abstract void UnapplyModifier(Card card, Card other);
}
