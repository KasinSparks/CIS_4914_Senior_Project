using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ShopBehavior : MonoBehaviour
{
    [SerializeField] private List<CardSlot> slots; //now 12 slots (pairs)
    private List<CardSlot> purchasedSlots = new List<CardSlot>(); //to track which slots have been used for purchase
    [SerializeField] private Hand playerHand;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private string nextSceneName; //scene to load after exit
    [SerializeField] private float cardScale = 1f; //scale for playfield display
    [SerializeField] private List<CardData> availableCards;

    public GameObject upgradeButton;
    public GameObject exitButton;
    private CardData upgradedCardData; //stores the upgraded card TODO for later
    public Card selectedUpgradeCard;

    void Start()
    {
        foreach (CardData cardData in availableCards)
        {
            playerHand.AddCardForUpgrade(cardData);
        }
        foreach (var slot in slots)
        {
            slot.SetShopBehavior(this);
        }
    }

    public void PlaceCard(Card card, CardSlot slot)
    {
        if (purchasedSlots.Contains(slot) || slot.GetIsCardPlaced() || card == null) //cannot purchase from same slot twice
        {
            return;
        }
        slot.SetIsCardPlaced(true);
        slot.SetCard(card);
        card.transform.SetParent(this.transform);
        card.transform.SetPositionAndRotation(slot.transform.position, Quaternion.Euler(0, 0, 90));
        card.transform.localScale = Vector3.one * cardScale;
        card.SetSlot(slot);
        card.SetState(CardState.OnPlayfield);
        if (playerHand != null)
        {
            playerHand.RemoveCard(card);
        }
        if (selectedUpgradeCard == card)
        {
            selectedUpgradeCard = null;
        }
    }

    public void ConfirmUpgrade()
    {
        //find which pair is filled
        CardSlot pairSlot1 = null;
        CardSlot pairSlot2 = null;
        for (int i = 0; i < slots.Count; i += 2)
        {
            if (slots[i].GetCard() != null && slots[i + 1].GetCard() != null)
            {
                pairSlot1 = slots[i];
                pairSlot2 = slots[i + 1];
                break;
            }
        }
        if (pairSlot1 == null || pairSlot2 == null)
        {
            Debug.Log("No valid pair filled. Upgrade canceled.");
            return;
        }
        purchasedSlots.Clear();
        purchasedSlots.Add(pairSlot1);
        purchasedSlots.Add(pairSlot2);
        Card c1 = pairSlot1.GetCard();
        Card c2 = pairSlot2.GetCard();
        CardData data1 = c1.GetCardData();
        CardData data2 = c2.GetCardData();
        if (data1.card_name.Contains("Blessed") || data1.card_name.Contains("Evolved") || data2.card_name.Contains("Blessed") || data2.card_name.Contains("Evolved"))
        {
            Debug.Log("Upgraded cards cannot be upgraded again");
            return;
        }

        CardData newData = Instantiate(data1);
        if (data1.card_name == data2.card_name)
        {
            newData.attack *= 2;
            newData.hp *= 2;
            newData.card_name = "Evolved " + newData.card_name;
        }
        else
        {
            newData.card_name = "Blessed " + newData.card_name;
            newData.starting_modifiers = MergeModifiers(data1, data2);
        }

        playerHand.RemoveCard(c2);
        Destroy(c2.gameObject);
        pairSlot2.ResetCardSlot();

        c1.SetCardData(newData);
        c1.Initialize(newData);
        c1.SetSlot(pairSlot1);
        Debug.Log("Upgraded");
    }

    public void ExitScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private List<CardModifier> MergeModifiers(CardData d1, CardData d2)
    {
        List<CardModifier> merged = new List<CardModifier>();
        merged.AddRange(d1.starting_modifiers);

        foreach (var mod2 in d2.starting_modifiers)
        {
            bool duplicate = false;
            foreach (var existing in merged)
            {
                if (mod2.Compare(existing))
                {
                    duplicate = true;
                    break;
                }
            }
            if (!duplicate)
                merged.Add(mod2);
        }
        return merged;
    }

    private void SaveUpgradedCard(Card card)
    {
        //todo
    }
}