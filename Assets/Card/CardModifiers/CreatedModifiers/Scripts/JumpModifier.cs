using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Card/Modifier/Jump")]
public class JumpModifier : CardModifier
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
        CardSlot original_slot = card.GetSlot();
        CardSlot[] lanes = playfield.GetLeftAndRightOpenSlots(card.GetOwnership(), original_slot);
        int random_lane_index = UnityEngine.Random.Range(0, lanes.Length);

        if (lanes[0] == null && lanes[1] == null)
        {
            this.modifier_state = ModifierState.SetToReadyNextTurn;
            return;
        }
        else if (lanes[0] != null && lanes[1] == null)
        {
            random_lane_index = 0;
        }
        else if (lanes[0] == null && lanes[1] != null)
        {
            random_lane_index = 1;
        }

        original_slot.ResetCardSlot();

        // Move to one of the slot, if empty
        CardSlot random_slot = lanes[random_lane_index];

        card.transform.SetPositionAndRotation(
            new Vector3(random_slot.transform.position.x,
            random_slot.transform.position.y + 0.0001f * card.transform.localScale.x,
            random_slot.transform.position.z),
            Quaternion.Euler(0, 0, 0)
        );

        random_slot.SetCard(card);
        random_slot.SetIsCardPlaced(true);
        card.SetSlot(random_slot);

        this.modifier_state = ModifierState.SetToReadyNextTurn;
    }

    override public void UpdateModifier(Card card) {}

    override public void UnapplyModifier(Card card, Card other)
    {
        this.modifier_state = ModifierState.ReadyToApply;
    }
}
