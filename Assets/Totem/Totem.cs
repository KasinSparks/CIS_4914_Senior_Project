using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Totem : MonoBehaviour
{
    [SerializeField] private List<CardModifier> temp_modifiers;
    [SerializeField] private List<CardOrder> temp_orders;
    [SerializeField] private List<CardModifier> modifiers;
    [SerializeField] private List<CardOrder> orders;
    [SerializeField] private CardModifier selected_modifier;
    [SerializeField] private CardOrder selected_order;

    private GameObject totem_prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CardModifier[] saved_modifiers = SaveSystem.LoadTotemModifiers(SaveSystemFile.TotemModifiers);
        if (saved_modifiers == null || saved_modifiers.Length < 1)
        {
            foreach (CardModifier m in temp_modifiers)
            {
                this.modifiers.Add(m);
            }
            

        } else
        {
            Debug.Log("Loaded mod from file.");
            foreach (CardModifier m in saved_modifiers)
            {
                this.modifiers.Add(m);
            }
        }

        CardOrder[] saved_orders = SaveSystem.LoadTotemOrders(SaveSystemFile.TotemOrders);
        if (saved_orders == null || saved_orders.Length < 1)
        {
            foreach (CardOrder o in temp_orders)
            {
                this.orders.Add(o);
            }
            

        } else
        {
            Debug.Log("Loaded order from file.");
            foreach (CardOrder o in saved_orders)
            {
                this.orders.Add(o);
            }
        }

        this.totem_prefab = Resources.Load<GameObject>(selected_order.ToString() + "TotemPrefab");
        if (this.totem_prefab == null)
        {
            Debug.LogError("Failed to load Totem model from Resources.");
            return;
        }

        Instantiate(this.totem_prefab, this.gameObject.transform);
    }

    public void AttachModifier(Card card)
    {
        if (card.GetOrder() != this.selected_order) return;

        card.AttachModifier(this.selected_modifier);
    }

    void OnDestroy()
    {
        SaveSystem.SaveTotemOrders(this.orders.ToArray(), SaveSystemFile.TotemOrders);
        SaveSystem.SaveTotemModifiers(this.modifiers.ToArray(), SaveSystemFile.TotemModifiers);
    }

    // make a way for the player to select a totem (select)
}
