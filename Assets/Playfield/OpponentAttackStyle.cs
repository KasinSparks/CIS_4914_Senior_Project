public enum OpponentAttackStyle
{
    Random = 0,
    // Will shield the opponent as much as possible (cover as many slots, prefers to play cards with highest hp)
    Defensive = 1,
    // Will attack the player as much as possible (cover as many slots, prefers to play cards with highest attack)
    Aggressive = 2,
    // Will play based on the playfield, their hp, and the opponents hp
    Balanced = 3,
}