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

    public Texture image;

    public CardRarity card_rarity;

    // NOTE: This list is only used to add modifiers in the editor. If you need to get
    //       modifiers on this card during game runtime, use the GetModifiers function.
    public List<CardModifier> starting_modifiers;
}