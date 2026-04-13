using UnityEngine;
using UnityEngine.Rendering;

public abstract class CardModifier : ScriptableObject, ICardModifierEvents
{
    public string modifier_name;
    public string description;

    public Texture2D image;

    public ModifierType modifier_type;

    public ModifierState modifier_state;

    public bool defered_for_spawn_card = false;

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

    public class CardModifierSaveData
    {
        public string modifier;
        public string modifier_name;
        public string description;
        public string image_resouce_path;

        public bool defered_for_spawn_card;

        public ModifierType modifier_type;

        public ModifierState modifier_state;
        
        protected CardModifierSaveData() { }
        public CardModifierSaveData(CardModifier card_modifier)
        {
            this.modifier           = card_modifier.GetType().Name;
            this.modifier_name      = card_modifier.modifier_name;
            this.description        = card_modifier.description;
            this.image_resouce_path = card_modifier.image.name;

            this.defered_for_spawn_card = card_modifier.defered_for_spawn_card;

            this.modifier_type  = card_modifier.modifier_type;
            this.modifier_state = card_modifier.modifier_state;
        }

        public virtual CardModifierSaveData FromJson(string json)
        {
            return JsonUtility.FromJson<CardModifierSaveData>(json);
        }

    }

    protected void LoadBaseValuesFromSaveData(CardModifierSaveData data)
    {
        this.modifier_name = data.modifier_name;
        this.description   = data.description;
        this.defered_for_spawn_card = data.defered_for_spawn_card;

        this.modifier_type  = data.modifier_type;
        this.modifier_state = data.modifier_state;

        this.image = Resources.Load<Texture2D>("Images/" + data.image_resouce_path);
    }

    public virtual string ToJson()
    {
        return JsonUtility.ToJson(new CardModifierSaveData(this), true);
    }

    public static CardModifier FromJson(string json)
    {
        CardModifierSaveData raw_save_data =
            JsonUtility.FromJson<CardModifierSaveData>(json);
        CardModifier mod = (CardModifier)ScriptableObject.CreateInstance(raw_save_data.modifier);
        
        switch (raw_save_data.modifier)
        {
            case "AfricanDerivedQueenModifier":
                return ((AfricanDerivedQueenModifier)mod)._FromJson(json);
            case "AntiInsectModifier":
                return ((AntiInsectModifier)mod)._FromJson(json);
            case "ArmoredCardModifier":
                return ((ArmoredCardModifier)mod)._FromJson(json);
            case "AttackSpeedCardModifier":
                return ((AttackSpeedCardModifier)mod)._FromJson(json);
            case "ChemicalSprayCardModifier":
                return ((ChemicalSprayCardModifier)mod)._FromJson(json);
            case "ChemicalSprayEffect":
                return ((ChemicalSprayEffect)mod)._FromJson(json);
            case "DodgeCardModifier":
                return ((DodgeCardModifier)mod)._FromJson(json);
            case "ExplodeOnDeathModifier":
                return ((ExplodeOnDeathModifier)mod)._FromJson(json);
            case "Flutter":
                return ((FlutterModifier)mod)._FromJson(json);
            case "HealOnAttackModifier":
                return ((HealOnAttackModifier)mod)._FromJson(json);
            case "JumpModifier":
                return ((JumpModifier)mod)._FromJson(json);
            case "MoveToLaneAttackedModifier":
                return ((MoveToLaneAttackedModifier)mod)._FromJson(json);
            case "NektarReductionModifier":
                return ((NektarReductionModifier)mod)._FromJson(json);
            case "QueenModifier":
                return ((QueenModifier)mod)._FromJson(json);
            case "SideStrikeModifier":
                return ((SideStrikeModifier)mod)._FromJson(json);
            case "SpawnChildModifier":
                return ((SpawnChildModifier)mod)._FromJson(json);
            case "StingerDetachModifier":
                return ((StingerDetachModifier)mod)._FromJson(json);
            case "StrengthInNumberModifier":
                return ((StrengthInNumberModifier)mod)._FromJson(json);
            default:
                throw new System.NotImplementedException();
        }

        throw new System.Exception("Unable to find correct Card Modifier type to cast to. Failed to load Card Modifier data");
    }
}
