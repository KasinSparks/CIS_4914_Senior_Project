using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Card/Modifier/Move to Lane Attacked")]
public class MoveToLaneAttackedModifier : CardModifier
{
    private Playfield playfield;

    public override void Initialize()
    {
        SetDisplayDescription(this.description);
        
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            this.playfield = GameObject.Find("Playfield").GetComponent<Playfield>();
        }
    }

    override public void ApplyModifier(Card card, Card other)
    {
        CardSlot[] lanes_attacked = playfield.GetLanesAttacked(card.GetOwnership());
        // Move to one of the slot, if empty
        foreach (CardSlot slot in lanes_attacked)
        {
            if (!slot.GetIsCardPlaced())
            {
                card.GetSlot().ResetCardSlot();

                card.transform.SetPositionAndRotation(
                    new Vector3(slot.transform.position.x,
                    slot.transform.position.y + 0.0001f * card.transform.localScale.x,
                    slot.transform.position.z),
                    Quaternion.Euler(0, 0, 0)
                );

                slot.SetCard(card);
                card.SetSlot(slot);

                break;
            }
        }
        this.modifier_state = ModifierState.SetToReadyNextTurn;
    }

    override public void UpdateModifier(Card card) {}

    override public void UnapplyModifier(Card card, Card other)
    {
        this.modifier_state = ModifierState.ReadyToApply;
    }
}
