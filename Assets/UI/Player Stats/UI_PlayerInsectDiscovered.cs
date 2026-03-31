using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInsectDiscovered : MonoBehaviour
{
    [SerializeField]
    private UI_PlayerStatInsectOrder insect_order;

    [SerializeField]
    private RectTransform scroll_content_transform;

    private float height;
    private float text_height;

    void Awake()
    {
        this.height = GetComponent<RectTransform>().sizeDelta.y;
        this.text_height =
            this.transform.Find("Title").GetComponent<RectTransform>().sizeDelta.y;
    }

    void OnEnable()
    {
        float offset = float.PositiveInfinity;

        Dictionary<string, CardData> cards =
            PlayerStats.player_data.GetCardsDiscovered();

        Dictionary<CardOrder, List<CardData>> card_orders =
            new Dictionary<CardOrder, List<CardData>>();

        foreach (CardData card in cards.Values)
        {
            if (!card_orders.ContainsKey(card.order))
            {
                card_orders[card.order] = new List<CardData>();
            }
            card_orders[card.order].Add(card);
        }

        foreach (CardOrder order in card_orders.Keys)
        {
            UI_PlayerStatInsectOrder order_display =
                Instantiate<UI_PlayerStatInsectOrder>(insect_order, this.transform);

            if (offset.Equals(float.PositiveInfinity))
            {
                offset = ((height / 2.0f) - this.text_height) - (order_display.GetHeight() / 2.0f);
            }

            order_display.SetOrderName(Enum.GetName(typeof(CardOrder), order));
            order_display.SetCards(card_orders[order].ToArray());
            order_display.DisplayCards();
            order_display.AddToYPosition(offset);
            offset -= order_display.GetHeight() + 64.0f;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
