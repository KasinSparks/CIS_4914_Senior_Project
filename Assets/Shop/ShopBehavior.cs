using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class ShopBehavior : MonoBehaviour
{
    [SerializeField] private List<CardSlot> slots; //now 12 slots (pairs)
    private List<CardSlot> purchasedSlots = new List<CardSlot>(); //to track which slots have been used for purchase
    private List<CardOrder> shopOrder = new List<CardOrder>(); //to track the order of each card in each slot
    [SerializeField] private Hand playerHand;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private string nextSceneName; //scene to load after exit
    [SerializeField] private float cardScale = 1f; //scale for playfield display
    [SerializeField] private List<CardData> availableCards;
    [SerializeField] private List<CardData> shopCards; //cards to be placed in shop
    [SerializeField] private Material highlightMaterial; //to highlight wanted slots green to show where card can be placed
    [SerializeField] private Material defaultMaterial;

    public GameObject exitButton;
    private CardData upgradedCardData; //stores the upgraded card TODO for later? Maybe save will work instantly
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
        InitializeShopCards();
    }

    private void InitializeShopCards() //if we want to weigh the odds, duplicate cards in inspector
    {
        int shopIndex = 0;
        for (int i = 0; i < slots.Count; i += 2) // first slot in each pair
        {
            if (shopIndex >= shopCards.Count)
                break;

            CardSlot slot = slots[i];
            GameObject cardObj = Instantiate(cardPrefab); //create card, set data
            Card card = cardObj.GetComponent<Card>();
            card.SetContext(Card.CardContext.Upgrade); //so it doesnt look for gamestate in card
            CardData data = Instantiate(shopCards[Random.Range(0, shopCards.Count)]); //changed to be random
            card.SetCardData(data);
            card.Initialize(data);
            slot.SetCard(card); //place in 0, 2, ... 10 slot so that there will be a slot next to it
            slot.SetIsCardPlaced(true);
            card.transform.SetParent(this.transform);
            card.transform.SetPositionAndRotation(slot.transform.position, Quaternion.Euler(0, 0, 90));
            card.transform.localScale = Vector3.one * cardScale;
            card.SetSlot(slot);
            card.SetState(CardState.OnPlayfield);
            shopOrder.Add(card.GetOrder()); //save order of all cards in shop
            shopIndex++;
        }
    }

    public void PlaceCard(Card card, CardSlot slot)
    {
        /*
        foreach (var order in shopOrder)
        {
            Debug.Log(order);
        }
        */
        if (purchasedSlots.Contains(slot) || slot.GetIsCardPlaced() || card == null || card.card_state == CardState.OnPlayfield) //cannot purchase from same slot twice
        {
            return;
        }
        CardSlot pairedSlot = GetPairedSlot(slot); //get card in purchase pair, do not let player place card next to bug of wrong order
        Card pairedCard = pairedSlot.GetCard();
        if (pairedCard != null && pairedCard.GetOrder() != card.GetOrder())
        {
            Debug.Log("Card must be placed next to same order.");
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
        ConfirmUpgrade(); //the highlighted slots make it obvious so instant confirm
    }

    private CardSlot GetPairedSlot(CardSlot slot)
    {
        int index = slots.IndexOf(slot);
        return slots[index - 1]; //-1 since can only place on odd index
    }

    public void ConfirmUpgrade() //really confirm purchase
    {
        //find which pair is filled
        CardSlot pairSlot1 = null;
        CardSlot pairSlot2 = null;
        for (int i = 0; i < slots.Count; i += 2)
        {
            if (slots[i].GetCard() != null && slots[i + 1].GetCard() != null) //this will buy last slot if multiple slots are filled, may change to just insta buy
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
        CardData newData = Instantiate(data1);
        pairSlot2.ResetCardSlot();

        c2.SetCardData(newData); //copying purhcased card data to players card, so i dont have to create a new card, and this card should already have save data configured for it
        c2.Initialize(newData);
        c2.SetSlot(pairSlot1);
        c2.transform.position = c1.transform.position; //after copying the card, switch the places to make it seem like purchased card is flying into your hand
        Destroy(c1.gameObject);
        Debug.Log("Purchased " + newData.card_name);
        StartCoroutine(FloatToHand(c2.transform));
        pairSlot1.transform.position += new Vector3(0, -1000, 0); //move off screen
        pairSlot2.transform.position += new Vector3(0, -1000, 0);
        UnhighlightSlots();
    }

    private IEnumerator FloatToHand(Transform obj, float distance = 1.5f, float duration = 1.5f)
    {
        Vector3 startPos = obj.position;
        Vector3 endPos = startPos + Vector3.back * distance + Vector3.up * .04f; //slightly up so it doesnt go inside other cards if on top row
        float time = 0f;
        while (time < duration)
        {
            obj.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        obj.position = endPos;
    }

    public void HighlightSlots()
    {
        if (selectedUpgradeCard == null)
            return;
        CardOrder order = selectedUpgradeCard.GetOrder();
        for (int i = 0; i < 6; i++)
        {
            Renderer r = slots[i * 2 + 1].GetComponent<Renderer>();
            if (order == shopOrder[i])
            {
                r.material = highlightMaterial;
            }
            else
            {
                r.material = defaultMaterial;
            }
        }
    }

    public void UnhighlightSlots()
    {
        foreach (CardSlot slot in slots)
        {
            Renderer r = slot.GetComponent<Renderer>();
            r.material = defaultMaterial;
        }
    }

    public void ExitScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void SaveUpgradedCard(Card card)
    {
        //todo
    }
}