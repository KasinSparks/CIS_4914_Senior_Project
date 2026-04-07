using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class PlayerTotem : MonoBehaviour
{
    [SerializeField] private List<CardModifier> modifiers;
    [SerializeField] private List<CardOrder> orders;
    [SerializeField] private CardModifier selected_modifier;
    [SerializeField] private CardOrder selected_order;

    private GameObject totem_prefab;

    void Awake()
    {
        CardModifier[] saved_modifiers = SaveSystem.LoadTotemModifiers(SaveSystemFile.TotemModifiers);
        CardOrder[] saved_orders = SaveSystem.LoadTotemOrders(SaveSystemFile.TotemOrders);

        if (saved_modifiers != null && saved_modifiers.Length > 0)
        {
            Debug.Log("Loaded mod from file.");
            foreach (CardModifier m in saved_modifiers)
            {
                this.modifiers.Add(m);
            }
        }

        if (saved_orders != null && saved_orders.Length > 0)
        {
            Debug.Log("Loaded order from file.");
            foreach (CardOrder o in saved_orders)
            {
                this.orders.Add(o);
            }
        }
    }

    /**
     * @brief Attaches this.selected_modifer to card if card is of this.selected_order (used in Hand)
     * 
     * @param card card that the modifer attached to
     */
    public void AttachModifier(Card card)
    {
        if (card.GetOrder() != this.selected_order || card.GetOrder().Equals(CardOrder.Other))
        {
            return;
        }

        card.AttachModifier(this.selected_modifier);
    }

    public List<CardModifier> GetModifiers()
    {
        return this.modifiers;
    }

    public List<CardOrder> GetOrders()
    {
        return this.orders;
    }

    /**
     * @brief Sets this.selected_modifier, updates modifer image on totem
     * 
     * @param modifier used to set this.selected_modifer
     */
    public void SetSelectedModifier(CardModifier modifier)
    {
        this.selected_modifier = modifier;

        CardModifier modifier_img = Instantiate(this.selected_modifier);
        modifier_img.Initialize();

        this.totem_prefab.transform.GetComponent<Totem>().SetModifier(this.selected_modifier);

        this.totem_prefab.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = modifier_img.image;
    }

    /**
     * @brief Sets this.selected_order, destroys old order's totem, loads new order's totem
     * 
     * @param order used to set this.selected_order
     */
    public void SetSelectedOrder(CardOrder order)
    {
        this.selected_order = order;

        if (this.totem_prefab != null)
        {
            Destroy(this.totem_prefab);
        }
        
        this.totem_prefab = Resources.Load<GameObject>(selected_order.ToString() + "TotemPrefab");

        if (this.totem_prefab == null)
        {
            Debug.LogError("Failed to load Totem model from Resources.");
            return;
        }

        this.totem_prefab = Instantiate(this.totem_prefab, this.gameObject.transform);
    }
}
