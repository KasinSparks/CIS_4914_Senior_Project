using UnityEngine;

[CreateAssetMenu(menuName = "Totem/Totem")]
public class TotemData : ScriptableObject
{
    [SerializeField] private CardOrder order;
    [SerializeField] private GameObject totem_model;

    private CardModifier modifier;
    private string display_description;

    public void SetOrder(CardOrder order)
    {
        this.order = order;
        this.UpdateDisplayDescription();
    }

    public void SetModifier(CardModifier modifier)
    {
        this.modifier = modifier;
        this.UpdateDisplayDescription();
    }

    public CardModifier GetModifier()
    {
        return this.modifier;
    }

    public CardOrder GetOrder()
    {
        return this.order;
    }

    public GameObject GetModel()
    {
        return this.totem_model;
    }

    public string GetDisplayDescription()
    {
        return this.display_description;
    }

    /**
     * @brief Updates totem's display description based on current order and modifier.
     */
    public void UpdateDisplayDescription()
    {
        if (this.modifier == null)
        {
            Debug.Log("Modifier null for totem data");
        }
        this.display_description = "Cards of order " + this.order.ToString() + " have the " + this.modifier.GetName() + " modifier applied: " + this.modifier.GetDisplayDescription();
    }
}
