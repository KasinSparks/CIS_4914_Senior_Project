using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SelectTotem : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown order_dropdown;
    [SerializeField] private TMPro.TMP_Dropdown modifier_dropdown;

    [SerializeField] private Totem totem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.totem.GetModifiers().Count == 0 || this.totem.GetOrders().Count == 0)
        {
            Debug.Log("Either no orders or modifiers");
            this.gameObject.SetActive(false);
            return;
        }
        this.gameObject.SetActive(true);

        this.order_dropdown.options.Clear();
        TMPro.TMP_Dropdown.OptionData options;
        foreach (CardOrder o in this.totem.GetOrders())
        {
            options = new TMPro.TMP_Dropdown.OptionData();
            options.text = o.ToString();
            this.order_dropdown.options.Add(options);
        }
        this.order_dropdown.RefreshShownValue();

        this.modifier_dropdown.options.Clear();
        foreach (CardModifier m in this.totem.GetModifiers())
        {
            options = new TMPro.TMP_Dropdown.OptionData();
            options.text = m.GetName();
            this.modifier_dropdown.options.Add(options);
        }
        this.modifier_dropdown.RefreshShownValue();

        this.totem.SetSelectedOrder(this.totem.GetOrders()[0]);
        this.totem.SetSelectedModifier(this.totem.GetModifiers()[0]);


    }

    public void SetSelectedOrder()
    {
        this.totem.SetSelectedOrder(this.totem.GetOrders()[order_dropdown.value]);
    }

    public void SetSelectedModifier()
    {
        this.totem.SetSelectedModifier(this.totem.GetModifiers()[modifier_dropdown.value]);
    }
}
