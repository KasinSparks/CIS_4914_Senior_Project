using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

public class Reward : MonoBehaviour
{
    [Tooltip("Percentage chance of getting a totem reward (order or modifier)")]
    [Range(0, 100)]
    [SerializeField] private int totem_chance = 10;

    [Header("Possible Rewards")]
    [Tooltip("All possible order the reward totem can be")]
    [SerializeField] private List<CardOrder> all_orders;
    [Tooltip("All possible modifiers the reward totem can be")]
    [SerializeField] private List<CardModifier> all_modifiers;
    [Tooltip("All possible cards the reward card can be")]
    [SerializeField] private List<CardData> available_reward_cards;

    [Header("Card Settings")]
    [SerializeField] private float card_scale = 1.0f;
    [SerializeField] private GameObject card_prefab;

    private List<CardOrder> available_reward_orders; // Orders that the player doesn't have
    private List<CardModifier> available_reward_modifiers; // Modifiers that the player doesn't have

    private Card selected_card; // Card that is selected by the player
    private CardOrder selected_order = CardOrder.Coleoptera; // Order of totem
    private CardModifier selected_modifier; // Modifier of totem

    private string scene_name = "Path";
    private GameObject totem_prefab;
    private static int NUM_REWARDS = 3;

    void Awake()
    {
        bool has_totem = false; // only one totem reward allowed per reward scene
        bool is_order_valid = true;
        bool is_modifier_valid = true;
        this.available_reward_orders = new List<CardOrder>();
        this.available_reward_modifiers = new List<CardModifier>();

        for (int i = 0; i < NUM_REWARDS; i++)
        {
            Transform slot = this.transform.GetChild(i);
            CardSlot card_slot = slot.GetComponent<CardSlot>();

            if (!has_totem && (Random.Range(0, 100) < this.totem_chance))
            {
                has_totem = true;

                // loads saved totem modifers, if no file then save empty array
                CardModifier[] saved_modifiers = SaveSystem.LoadTotemModifiers(SaveSystemFile.TotemModifiers);
                if (saved_modifiers == null)
                {
                    SaveSystem.SaveTotemModifiers(System.Array.Empty<CardModifier>(), SaveSystemFile.TotemModifiers);
                    saved_modifiers = System.Array.Empty<CardModifier>();
                }

                // if there are saved totems, get all modifers that aren't saved and set as available rewards
                // (won't shown totem modifers that the player already has)
                if (saved_modifiers.Length > 0)
                {
                    Debug.Log("Loaded mod from file.");
                    foreach (CardModifier m in this.all_modifiers.Except(saved_modifiers.ToList()))
                    {
                        this.available_reward_modifiers.Add(m);
                    }
                // if no saved totems, all modifers are set as available rewards
                } else
                {
                    this.available_reward_modifiers = this.all_modifiers;
                }

                // loads saved totem order, if no file then save empty array
                CardOrder[] saved_orders = SaveSystem.LoadTotemOrders(SaveSystemFile.TotemOrders);
                if (saved_orders == null)
                {
                    SaveSystem.SaveTotemOrders(System.Array.Empty<CardOrder>(), SaveSystemFile.TotemOrders);
                    saved_orders = System.Array.Empty<CardOrder>();
                }

                // if there are saved totems, get all orders that aren't saved and set as available rewards
                // (won't shown totem orders that the player already has)
                if (saved_orders.Length > 0)
                {
                    Debug.Log("Loaded order from file.");
                    foreach (CardOrder o in this.all_orders.Except(saved_orders.ToList()))
                    {
                        this.available_reward_orders.Add(o);
                    }
                // if no saved totems, all orders are set as available rewards
                } else
                {
                    this.available_reward_orders = this.all_orders;
                }

                // if there aren't any available orders as rewards, then is_order_valid is false
                if (this.available_reward_orders.Count < 1)
                {
                    Debug.Log("No reward orders in list to select from");
                    is_order_valid = false;
                }

                // if there aren't any available modifiers as rewards, then is_modifier_valid is false
                if (this.available_reward_modifiers.Count < 1)
                {
                    Debug.Log("No reward modifers in list to select from");
                    is_modifier_valid = false;
                }

                // hides/disables card slot
                card_slot.enabled = false;
                slot.GetComponent<MeshRenderer>().enabled = false;

                // if there are both available totem orders and modifiers, 50/50 chance of getting either
                if (is_order_valid && is_modifier_valid)
                {
                    this.InstantiateTotemPrefab(slot);
                    card_slot.SetReward(this);

                    if (Random.Range(0, 2) == 0)
                    {
                        // Picked totem order as reward
                        this.InstantiateTotemOrder(slot);
                    } else
                    {
                        // Picked totem modifier as reward
                        this.InstantiateTotemModifier();
                    }
                    continue;
                // if there are only available totem orders, pick an order as reward
                }
                else if (is_order_valid)
                {
                    this.InstantiateTotemOrder(slot);
                    continue;

                // if there are only available totem modifiers, pick a modifier as reward
                }
                else if (is_modifier_valid)
                {
                    this.InstantiateTotemPrefab(slot);
                    this.InstantiateTotemModifier();

                    continue;

                // if no available totem orders and modifiers, pick a card as reward
                }
                else
                {
                    card_slot.enabled = true;
                    slot.GetComponent<MeshRenderer>().enabled = true;
                }

            }

            InstantiateCard(slot, card_slot, i);
        }
    }

    public CardOrder GetSelectedOrder()
    {
        return this.selected_order;
    }

    public CardModifier GetSelectedModifier()
    {
        return this.selected_modifier;
    }

    public Card GetSelectedCard()
    {
        return this.selected_card;
    }

    /**
     * @brief When selected card is set, adds this card to deck and loads path scene
     * 
     * @param card card to be set to this.selected_card
     */
    public void SetSelectedCard(Card card)
    {
        this.selected_card = card;

        // add selected card to deck file
        SaveSystem.AddCardToDeckSave(this.selected_card.GetCardData(), SaveSystemFile.PlayerDeck);
        PlayerStats.player_data.AddCardToDiscoveredList(this.selected_card.GetCardData());

        PlayerStats.Save();

        // load path scene
        if (!string.IsNullOrEmpty(this.scene_name))
        {
            SaveSystem.SavePlayerPathNodeState(this.scene_name);
            SceneManager.LoadScene(this.scene_name);
        }
    }

    /**
     * @brief Instantiates reward card
     * 
     * @param slot transform of slot game object
     * @param card_slot CardSlot script attached to the slot game object
     * @param i used to set current slot index for card_slot
     */
    private void InstantiateCard(Transform slot, CardSlot card_slot, int i)
    {
        if (this.available_reward_cards == null || this.available_reward_cards.Count < 1)
        {
            Debug.Log("No reward cards in list to select from");
            return;
        }

        int index = Random.Range(0, this.available_reward_cards.Count - 1);

        card_slot.SetCardSlot(slot.gameObject);
        card_slot.SetReward(this);
        card_slot.SetSlotIndex(i);
        card_slot.SetCardOwnership(CardOwnership.Reward);

        GameObject card_obj = Instantiate(this.card_prefab, slot); //create card, set data
        card_obj.transform.SetPositionAndRotation(
            new Vector3(slot.position.x,
            slot.position.y + 0.0001f * card_obj.transform.localScale.x,
            slot.position.z),
            Quaternion.Euler(0, 0, 0)
        );
        card_obj.transform.localScale = new Vector3(this.card_scale, this.card_scale, this.card_scale);
        Card card = card_obj.GetComponent<Card>();
        card.SetContext(Card.CardContext.Reward); //so it doesnt look for gamestate in card
        CardData data = Instantiate(this.available_reward_cards[index]); //changed to be random
        card.SetCardData(data);
        card.Initialize(data);
        card.SetCardData(this.available_reward_cards[index]);
        card.gameObject.SetActive(true);
        card.SetState(CardState.OnPlayfield);
        card.SetOwnership(CardOwnership.Reward);

        card.SetSlot(card_slot);
        card_slot.SetCard(card);
    }

    /**
     * @brief Instantiates reward totem for order
     * 
     * @param slot transform of slot game object
     */
    private void InstantiateTotemOrder(Transform slot)
    {
        this.selected_order = this.available_reward_orders[Random.Range(0, this.available_reward_orders.Count)];
        
        Destroy(this.totem_prefab);
        this.InstantiateTotemPrefab(slot);
        this.totem_prefab.transform.GetChild(0).gameObject.SetActive(false);
    }

    /**
     * @brief Instantiates reward totem for modifier
     */
    private void InstantiateTotemModifier()
    {
        this.selected_modifier = this.available_reward_modifiers[Random.Range(0, this.available_reward_modifiers.Count)];

        CardModifier modifier_img = Instantiate(this.selected_modifier);
        modifier_img.Initialize();
        this.totem_prefab.transform.GetComponent<Totem>().SetModifier(this.selected_modifier);
        this.totem_prefab.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = modifier_img.image;
        this.totem_prefab.transform.GetChild(0).rotation = Quaternion.Euler(
             0,
             180,
             this.totem_prefab.transform.rotation.x
        );

        this.totem_prefab.transform.GetChild(1).gameObject.SetActive(false);
    }

    /**
     * @brief Instantiates reward totem prefab (contains totem order model and modifer image as children)
     * 
     * @param slot transform of slot game object
     */
    private void InstantiateTotemPrefab(Transform slot)
    {
        this.totem_prefab = Resources.Load<GameObject>(this.selected_order.ToString() + "TotemPrefab");

        if (this.totem_prefab == null)
        {
            Debug.LogError("Failed to load Totem model from Resources.");
            return;
        }

        this.totem_prefab = Instantiate(this.totem_prefab, slot.transform);
        this.totem_prefab.transform.GetComponent<Totem>().SetReward(this);
    }
}