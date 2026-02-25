using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayfieldUpgrade : MonoBehaviour
{
    [SerializeField] private CardSlot slot1;
    [SerializeField] private CardSlot slot2;
    [SerializeField] private Hand playerHand;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private string nextSceneName; //scene to load after exit
    [SerializeField] private float cardScale = 1f; //scale for playfield display
    [SerializeField] private List<CardData> availableCards;

    private bool upgradePerformed = false; //ensures only one upgrade
    private CardData upgradedCardData; //stores the upgraded card
    public Card selectedUpgradeCard; 

    void Start()
    {
        foreach (CardData cardData in availableCards)
        {
            playerHand.AddCardForUpgrade(cardData);
        }
        slot1.SetPlayfieldUpgrade(this);
        slot2.SetPlayfieldUpgrade(this);
    }

    public void PlaceCard(Card card, CardSlot slot)
    {
        if (upgradePerformed || slot.GetIsCardPlaced() || card == null) 
        { //can only upgrade once
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
        if (upgradePerformed)
        {
            Debug.Log("Already Upgraded");
            return;
        }
        if (slot1.GetCard() == null || slot2.GetCard() == null)
        {
            Debug.Log("Both slots must be filled");
            return;
        }
        Card c1 = slot1.GetCard();
        Card c2 = slot2.GetCard();
        if (c1.card_name == c2.card_name) //if they are same card double the stats
        {
            upgradedCardData = CreateDoubledCard(c1);
        }
        else //if they dont have same name they arent same card so upgrade normally
        {
            upgradedCardData = CreateUpgradedCard(c1, c2);
        }
        playerHand.RemoveCard(c1); //remove orignal cards
        playerHand.RemoveCard(c2);
        if (playerHand != null)
        {
            playerHand.RemoveCard(c1);
            playerHand.RemoveCard(c2);
        }
        Destroy(c1.gameObject); //destroy cards
        Destroy(c2.gameObject);
        slot1.ResetCardSlot(); //reset slots
        slot2.ResetCardSlot();
        Card newCard = Instantiate(cardPrefab, transform).GetComponent<Card>(); //display card
        newCard.SetContext(Card.CardContext.Upgrade);
        newCard.Initialize(upgradedCardData);
        newCard.transform.SetParent(this.transform);
        card.transform.SetPositionAndRotation(slot.transform.position, Quaternion.Euler(0, 0, 90));
        newCard.transform.localScale = Vector3.one * cardScale;
        newCard.SetSlot(slot1);
        newCard.SetState(CardState.OnPlayfield);
        slot1.SetCard(newCard);
        slot1.SetIsCardPlaced(true);
        SaveUpgradedCard(upgradedCardData); //save card
        upgradePerformed = true;
        Debug.Log("Upgraded");
    }

    public void ExitScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private CardData CreateUpgradedCard(Card c1, Card c2)
    {
        CardData newData = Instantiate(c1.GetCardData());
        newData.card_name += "+"; //ex. ant -> ant+
        newData.starting_modifiers = MergeModifiers(c1, c2); //combine modifiers
        return newData;
    }

    private CardData CreateDoubledCard(Card c1)
    {
        CardData newData = Instantiate(c1.GetCardData());
        newData.card_name = "Evolved " + newData.card_name; //ex. ant -> Evolved ant
        newData.attack = c1.attack * 2; //double attack and health
        newData.hp = c1.hp * 2;
        return newData;
    }

    //merge modifiers from two cards avoiding duplicates
    private List<CardModifier> MergeModifiers(Card c1, Card c2)
    {
        List<CardModifier> merged = new List<CardModifier>();
        List<CardModifier> mods1 = c1.GetAllModifierData();
        List<CardModifier> mods2 = c2.GetAllModifierData();

        foreach (var mod in mods1) //add all modifiers to merged list
        {
            merged.Add(mod);
        }

        foreach (var mod2 in mods2) //no duplicate modifiers
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
            if (duplicate == false)
            {
                merged.Add(mod2);
            }
        }
        return merged;
    }

    private void SaveUpgradedCard(CardData card)
    {
        //todo
    }
}