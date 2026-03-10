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

    public GameObject upgradeButton;

    public GameObject exitButton;

    public GameObject healButton;


    private bool upgradePerformed = false; //ensures only one upgrade
    private CardData upgradedCardData; //stores the upgraded card
    public Card selectedUpgradeCard; 

    void Start()
    {
        CardData[] playerCards = SaveSystem.LoadDeck(SaveSystemFile.PlayerDeck); //for save data
        foreach (CardData cardData in playerCards) //changed to save data cards
        {
            playerHand.AddCardForUpgrade(cardData);
        }
        slot1.SetPlayfieldUpgrade(this);
        slot2.SetPlayfieldUpgrade(this);
        if (SceneManager.GetActiveScene().name == "Campfire")
        {
            exitButton.SetActive(false); //you cant straight up exit, either upgrade or heal
        }
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
        card.transform.SetPositionAndRotation(slot.transform.position, Quaternion.Euler(0, 0, 0));
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
        if (SceneManager.GetActiveScene().name == "Sacrafice") 
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
            CardData data1 = c1.GetCardData();
            CardData data2 = c2.GetCardData();
            if (data1.card_name.Contains("Blessed") || data1.card_name.Contains("Evolved") || data2.card_name.Contains("Blessed") || data2.card_name.Contains("Evolved"))
            {
                Debug.Log("Upgraded cards cannot be upgraded again");
                return;
            }
            CardData newData = Instantiate(data1); //will write to this data and then apply to first card
            if (data1.card_name == data2.card_name) //if they are same card double the stats
            {
                newData.attack *= 2;
                newData.hp *= 2;
                newData.card_name = "Evolved " + newData.card_name;
            }
            else //if they dont have same name they arent same card so upgrade normally
            {
                newData.card_name = "Blessed " + newData.card_name;
                newData.starting_modifiers = MergeModifiers(data1, data2); //combine modifiers
            }
            playerHand.RemoveCard(c2);
            c2.transform.position += new Vector3(0, -1000, 0);
            slot2.ResetCardSlot();
            c1.SetCardData(newData); //apply new stats
            c1.Initialize(newData);
            c1.SetSlot(slot1);
            SaveUpgradedCard(c1, c2);
            upgradePerformed = true;
            Debug.Log("Upgraded");
        }
        if (SceneManager.GetActiveScene().name == "Campfire")
        {
            if (upgradePerformed)
            {
                Debug.Log("Already Upgraded");
                return;
            }
            if (slot1.GetCard() == null)
            {
                Debug.Log("Fill the slot with a card");
                return;
            }
            Card c1 = slot1.GetCard();
            CardData data1 = c1.GetCardData();
            if (data1.card_name.Contains("+"))
            {
                Debug.Log("Upgraded cards cannot be upgraded again");
                return;
            }
            CardData newData = Instantiate(data1); //will write to this data and then apply to first card
            Card oldData = c1;
            newData.attack += 2;
            newData.hp += 2;
            newData.card_name = newData.card_name + "+"; //ex: Ant+
            c1.SetCardData(newData); //apply new stats
            c1.Initialize(newData);
            c1.SetSlot(slot1);
            SaveUpgradedCardSingle(oldData, c1);
            upgradePerformed = true;
            Debug.Log("Upgraded");
            healButton.SetActive(false);
            exitButton.SetActive(true);
        }
        upgradeButton.SetActive(false); //deactivate option
    }

    public void HealPlayer()
    {
        upgradeButton.SetActive(false);
        healButton.SetActive(false);
        exitButton.SetActive(true);
        //TODO once Gabriel finishes health system
    }

    public void ExitScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    //merge modifiers from two cards avoiding duplicates
    private List<CardModifier> MergeModifiers(CardData d1, CardData d2)
    {
        List<CardModifier> merged = new List<CardModifier>();
        foreach (var mod in d1.starting_modifiers)
        { //add all modifiers to merged list
            merged.Add(mod);
        }
        foreach (var mod2 in d2.starting_modifiers) //no duplicate modifiers
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
                merged.Add(mod2);
        }
        return merged;
    }

    private void SaveUpgradedCard(Card keptCard, Card sacrificedCard)
    {
        SaveSystem.RemoveCardFromDeckSave(keptCard.GetCardData(), SaveSystemFile.PlayerDeck);
        SaveSystem.RemoveCardFromDeckSave(sacrificedCard.GetCardData(), SaveSystemFile.PlayerDeck);

        SaveSystem.AddCardToDeckSave(keptCard.GetCardData(), SaveSystemFile.PlayerDeck);
    }

    private void SaveUpgradedCardSingle(Card oldCard, Card newCard)
    {
        SaveSystem.RemoveCardFromDeckSave(oldCard.GetCardData(), SaveSystemFile.PlayerDeck);
        SaveSystem.AddCardToDeckSave(newCard.GetCardData(), SaveSystemFile.PlayerDeck);
    }
}