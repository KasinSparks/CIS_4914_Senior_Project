using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Card/Modifier/Flutter")]
public class Flutter : CardModifier
{
    private Playfield playfield;

    [SerializeField]
    private CardData leave_behind_card;

    public override void Initialize()
    {
        if (this.leave_behind_card == null)
        {
            SetDisplayDescription(this.description);
        }
        else
        {
            SetDisplayDescription(this.description.Replace("XXX", this.leave_behind_card.card_name));
        }
        
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            this.playfield = GameObject.Find("Playfield").GetComponent<Playfield>();
        }
    }

    override public void ApplyModifier(Card card, Card other)
    {
        CardSlot[] lanes_open = playfield.GetOpenLanes(card.GetOwnership());
        if (lanes_open.Length <= 0)
        {
            this.modifier_state = ModifierState.SetToReadyNextTurn;
            return;
        }

        CardSlot original_slot = card.GetSlot();

        if (this.leave_behind_card != null)
        {
            Card new_card = Instantiate<Card>(Resources.Load<Card>("Card"), playfield.transform);
            new_card.SetCardData(leave_behind_card);
            new_card.Initialize(leave_behind_card);
            original_slot.SetCard(new_card);
            original_slot.SetIsCardPlaced(true);
            new_card.SetSlot(original_slot);
            new_card.SetState(CardState.OnPlayfield);
            new_card.SetOwnership(card.GetOwnership());

            new_card.transform.SetPositionAndRotation(
                new Vector3(original_slot.transform.position.x,
                original_slot.transform.position.y + 0.0001f * new_card.transform.localScale.x,
                original_slot.transform.position.z),
                Quaternion.Euler(0, 0, 0)
            );
            float card_scale = playfield.GetCardScaleAmount();
            new_card.transform.localScale = new Vector3(card_scale, card_scale, card_scale);
        }
        else
        {
            original_slot.ResetCardSlot();
        }

        // Move to one of the slot, if empty
        int random_lane_index = UnityEngine.Random.Range(0, lanes_open.Length);
        CardSlot random_slot = lanes_open[random_lane_index];

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
