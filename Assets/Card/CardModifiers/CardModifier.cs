using UnityEngine;

public abstract class CardModifier : MonoBehaviour
{
    public string modifier_name;
    public string description;
    private string display_description;

    public Texture2D image;

    public ModifierType  modifier_type;
    public ModifierState modifier_state;


    protected void SetImage()
    {
        this.gameObject.transform.Find("ModifierImage").gameObject.GetComponent<Renderer>().material.mainTexture = this.image;
    }

    protected void SetDisplayDescription(string description)
    {
        this.display_description = description;
    }

    public string GetDisplayDescription()
    {
        return this.display_description;
    }
    
    // Compare the name, description, and modifier_type
    public bool Compare(CardModifier other)
    {
        if (this.modifier_name != other.modifier_name)
        {
            return false;
        }

        if (this.description != other.description)
        {
            return false;
        }

        if (this.modifier_type != other.modifier_type)
        {
            return false;
        }

        return true;
    }

    public abstract void ApplyModifier(Card card, Card other);

    public abstract void UpdateModifier(Card card);

    public abstract void UnapplyModifier(Card card, Card other);
}
