using TMPro;
using UnityEngine;

public class UI_PlayerStatInsectOrder : MonoBehaviour
{
    [SerializeField]
    private UI_PlayerStatInsectDisplayPrefab display_prefab;
    [SerializeField]
    private TextMeshProUGUI order_name;

    private CardData[] cards;

    private float height;

    private RectTransform transform;

    void Awake()
    {
        this.transform = GetComponent<RectTransform>();
        this.height = transform.sizeDelta.y;    
    }

    public float GetHeight()
    {
        return this.height;
    }

    public void SetYPosition(float y)
    {
        this.transform.localPosition = new Vector3(
            this.transform.localPosition.x,
            this.transform.localPosition.y + y,
            this.transform.localPosition.z
        ); 
    }

    public void SetCards(CardData[] cards)
    {
        this.cards = cards;
    }

    public void DisplayCards()
    {
        float offset = 0.0f;
        foreach (CardData card in cards)
        {
            UI_PlayerStatInsectDisplayPrefab prefab =
                Instantiate<UI_PlayerStatInsectDisplayPrefab>(display_prefab,
                    this.transform);

            prefab.SetImage(card.image);
            prefab.SetName(card.name);

            prefab.SetXPosition(offset);

            offset += prefab.GetWidth() + 32.0f;
        }
    }

    public void SetOrderName(string name)
    {
        this.order_name.text = name;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
