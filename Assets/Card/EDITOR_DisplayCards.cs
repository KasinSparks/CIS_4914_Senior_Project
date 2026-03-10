using UnityEngine;
using UnityEngine.EventSystems;

public class EDITOR_DisplayCards : MonoBehaviour, IDragHandler, IScrollHandler
{
    [SerializeField]
    private CardData[] cards;

    private const int MAX_CARDS_PER_ROW = 9;
    private const float X_DELTA = 0.175f;
    private const float Z_DELTA = -0.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Card card_prefab = Resources.Load<Card>("Card");

        for (int i = 0; i < cards.Length; ++i)
        {
            int row = i / MAX_CARDS_PER_ROW;
            int col = i % MAX_CARDS_PER_ROW;

            Card card = Instantiate<Card>(card_prefab, this.transform);
            card.SetContext(Card.CardContext.Creator);
            card.SetCardData(cards[i]);

            card.transform.SetLocalPositionAndRotation(
                new Vector3(
                    card.transform.localPosition.x + (X_DELTA * col),
                    card.transform.localPosition.y,
                    card.transform.localPosition.z + (Z_DELTA * row)
                ),
                card.transform.localRotation
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.SetPositionAndRotation(
            new Vector3(
                this.transform.position.x + (eventData.delta.x * 0.001f),
                this.transform.position.y,
                this.transform.position.z + (eventData.delta.y * 0.001f)
            ),
            this.transform.rotation
        );
        Debug.Log("Drag delta: " + eventData.delta);
    }

    public void OnScroll(PointerEventData eventData)
    {
        this.transform.SetPositionAndRotation(
            new Vector3(
                this.transform.position.x,
                this.transform.position.y + (eventData.scrollDelta.y * 0.005f),
                this.transform.position.z
            ),
            this.transform.rotation
        );
        Debug.Log("Scroll delta: " + eventData.scrollDelta);
    }
}
