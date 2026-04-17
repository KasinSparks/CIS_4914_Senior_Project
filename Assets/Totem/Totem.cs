using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class Totem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TotemData totem_data;
    [SerializeField] private CardModifier modifier;
    [SerializeField] private CardOrder order;
    [SerializeField] private GameObject totem_prefab;

    private Reward reward;

    [SerializeField] private string display_description;
    private string scene_name = "Path";
    private GameObject modifierInfoCanvas;
    private GameObject modifierInfoUIWidget;

    void Awake()
    {
        reward = null;
        modifierInfoCanvas = GameObject.Find("UI_ModifierDisplay");
        modifierInfoUIWidget = GameObject.Find("UI_ModifierDisplay/UI_TotemModifierInfoRef");


        if (this.totem_data == null)
        {
            Debug.Log("Totem data not set");
            return;
        }

        this.order = this.totem_data.GetOrder();

        this.totem_prefab = Instantiate(this.totem_data.GetModel(), this.transform);

        if (this.totem_prefab == null)
        {
            Debug.Log("Totem model not set in totem data.");
            return;
        }

    }

    public CardModifier GetModifier()
    {
        return this.modifier;
    }

    /**
     * @brief Sets this.modifier and updates the totem's display description based on the modifier
     * 
     * @param modifier used to set this.modifier
     */
    public void SetModifier(CardModifier modifier)
    {
        this.modifier = modifier;
        this.totem_data.SetModifier(this.modifier);
        this.totem_data.UpdateDisplayDescription();
        
        if (this.reward != null)
        {
            this.display_description = "Cards of totem's order will have the " + this.modifier.GetName() + " modifier applied: \n " + this.modifier.GetDisplayDescription();
        } else
        {
            this.display_description = this.totem_data.GetDisplayDescription();
        }
        
    }

    /**
     * @brief Sets this.order, sets the modifier for the new totem, updates the totem's display description based on the order
     * 
     * @param order used to set this.order
     */
    public void SetOrder(CardOrder order)
    {
        this.order = order;

        if (this.totem_prefab != null)
        {
            Destroy(this.totem_prefab);
        }
        this.totem_prefab = Instantiate(this.totem_data.GetModel(), this.transform);

        if (this.totem_prefab == null)
        {
            Debug.LogError("Totem model not set in totem data.");
            return;
        }
        this.SetModifier(this.modifier);
    }

    public Reward GetReward()
    {
        return this.reward;
    }

    public void SetReward(Reward reward)
    {
        this.reward = reward;
    }

    /**
     * @brief Displays totem modifier/order when hover over totem
     * 
     * @param eventData
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject modifier_info_widget = Instantiate(this.modifierInfoUIWidget, modifierInfoCanvas.transform);
        UIModifierInfo modifier_info_widget_data = modifier_info_widget.GetComponent<UIModifierInfo>();

        // if totem isn't reward and doesn't have a modifier, return
        if (this.modifier == null && this.reward == null)
        {
            return;
        // if totem is order reward, set description and name for order
        } else if (this.modifier == null && this.reward != null)
        {
            this.display_description = "Cards of order " + this.order.ToString() + " will have the totem's modifier applied";
            modifier_info_widget_data.SetName(this.order.ToString());
        // if totem isn't an order reward, set name and attach image
        } else
        {
            modifier_info_widget_data.SetImage(this.modifier.GetImage());
            modifier_info_widget_data.SetName(this.modifier.GetName());
        }

        // Have a UI Pop-up to show the totem details
        modifier_info_widget_data.SetDescription(this.display_description);

        float pauseButtonSpacing = 200;

        Vector2 widget_size = modifier_info_widget_data.GetRectSize();
        modifier_info_widget.transform.SetPositionAndRotation(
            new Vector3(
                modifierInfoUIWidget.transform.position.x,
                modifierInfoUIWidget.transform.position.y
                    - (widget_size.y + 16 + pauseButtonSpacing),
                modifierInfoUIWidget.transform.position.z
            ),
            modifierInfoUIWidget.transform.rotation
        );        
    }

    /**
     * @brief Removes display of totem modifier/order when leave totem
     * 
     * @param eventData
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.modifierInfoCanvas == null)
        {
            return;
        }

        Transform obj = this.modifierInfoCanvas.transform.GetChild(2);

        if (obj != null && !obj.name.Equals("UI_TotemModifierInfoRef"))
        {
            Destroy(obj.gameObject);
        }        
    }

    /**
     * @brief If reward totem, saves order or modifier and loads path scene when clicked
     * 
     * @param eventData
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        // if not reward totem, return
        if (this.reward == null)
        {
            return;
        // if totem is order reward, add to totem order file
        } else if (this.modifier == null)
        {
            SaveSystem.AddTotemOrderToSaveFile(this.reward.GetSelectedOrder(), SaveSystemFile.TotemOrders);
        // if totem is modifier reward, add to totem modifier file
        } else
        {
            SaveSystem.AddTotemModifierToSaveFile(this.reward.GetSelectedModifier(), SaveSystemFile.TotemModifiers);
        }

        // once reward totem clicked, load path
        if (!string.IsNullOrEmpty(this.scene_name))
        {
            SaveSystem.SavePlayerPathNodeState(this.scene_name);
            BackgroundSound.Play(BackgroundSound.Sounds.Path);
            SceneManager.LoadScene(this.scene_name);
        }
    }
}
