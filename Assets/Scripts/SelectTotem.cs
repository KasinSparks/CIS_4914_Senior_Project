using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SelectTotem : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown order_dropdown;
    [SerializeField] private TMPro.TMP_Dropdown modifier_dropdown;
    [SerializeField] private PlayerTotem totem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if no orders or modifiers, don't show totem menu
        if (this.totem.GetModifiers().Count == 0 || this.totem.GetOrders().Count == 0)
        {
            Debug.Log("Either no orders or modifiers");
            this.gameObject.SetActive(false);
            return;
        }

        // show totem menu
        this.gameObject.SetActive(true);

        // set up order dropdown with saved_order from PlayerTotem (saved orders)
        this.order_dropdown.options.Clear();
        TMPro.TMP_Dropdown.OptionData options;
        foreach (CardOrder o in this.totem.GetOrders())
        {
            options = new TMPro.TMP_Dropdown.OptionData();
            options.text = o.ToString();
            this.order_dropdown.options.Add(options);
        }
        this.order_dropdown.RefreshShownValue();

        // set up modifier dropdown with saved_modifiers from PlayerTotem (saved modifiers)
        this.modifier_dropdown.options.Clear();
        foreach (CardModifier m in this.totem.GetModifiers())
        {
            options = new TMPro.TMP_Dropdown.OptionData();
            options.text = m.GetName();
            this.modifier_dropdown.options.Add(options);
        }
        this.modifier_dropdown.RefreshShownValue();

        // sets selected order and modifer with first value from dropdown as default
        this.totem.SetSelectedOrder(this.totem.GetOrders()[0]);
        this.totem.SetSelectedModifier(this.totem.GetModifiers()[0]);
    }

    /**
     * @brief Calls SetSelectedOrder in Totem with current order dropdown value and calls this.SetSelectedModifier()
     */
    public void SetSelectedOrder()
    {
        this.totem.SetSelectedOrder(this.totem.GetOrders()[order_dropdown.value]);
        this.SetSelectedModifier();
    }

    /**
     * @brief Calls SetSelectedModifier in Totem with current modifier dropdown value
     */
    public void SetSelectedModifier()
    {
        this.totem.SetSelectedModifier(this.totem.GetModifiers()[modifier_dropdown.value]);
    }
}
