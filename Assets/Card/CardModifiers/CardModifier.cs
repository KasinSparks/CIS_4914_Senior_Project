using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CardModifier : ScriptableObject, ICardModifierEvents, ISavable
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

    public virtual JsonValue ToJsonObject()
    {
        JsonObject json_object = new JsonObject();
        JsonString type_info = new JsonString() { value = this.GetType().ToString() };
        JsonObject data = new JsonObject();
        data["modifier_name"] = new JsonString() { value = this.modifier_name };
        data["description"]   = new JsonString() { value = this.description };
        data["ModifierType"]   = new JsonInt() { value = (int)this.modifier_type };
        data["ModifierState"]  = new JsonInt() { value = (int)this.modifier_state};
        data["image"] = SaveSystemTable.GetJsonForTexture2D(this.image);

        json_object["Type"] = type_info;
        json_object["Data"] = data;

        return json_object;
    }

    public virtual void OverrideValuesFromJson(JsonValue json)
    {
        JsonObject json_data = (JsonObject)json;

        this.modifier_name  = ((JsonString)json_data["modifier_name"]).value;
        this.description    = ((JsonString)json_data["description"]).value;
        this.modifier_type  = (ModifierType) ((JsonInt)json_data["ModifierType"]).value;
        this.modifier_state = (ModifierState) ((JsonInt)json_data["ModifierState"]).value;

        JsonObject image_data = (JsonObject)json_data["image"];
        this.image = SaveSystemTable.GetTexture2DFromJsonImage(image_data);
    }
}
