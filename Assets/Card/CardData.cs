using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/Card")]
public class CardData : ScriptableObject
{
    public string card_name;
    public string description;

    public WordInfo[] highlighted_words;

    public CardOrder order;

    public int hp;
    public int attack;
    public int nektar_cost;

    public int nektar_given_when_scarificed;

    public Texture image;

    public CardRarity card_rarity;

    // NOTE: This list is only used to add modifiers in the editor. If you need to get
    //       modifiers on this card during game runtime, use the GetModifiers function.
    public List<CardModifier> starting_modifiers;
    
    /**
     * @brief A very simple comparison. Compares name and order to determine
     * equality.
     * @param The other card data.
     * @return If the card is the same as the other.
     */
    public bool Compare(CardData other)
    {
        if (other == null) return false;

        if ((this.card_name.Equals(other.card_name)) && (this.order == other.order)) return true;

        return false;
    }
}