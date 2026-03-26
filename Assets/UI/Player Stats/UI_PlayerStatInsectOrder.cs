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
    private float width;

    private RectTransform rect_transform;

    void Awake()
    {
        this.rect_transform = GetComponent<RectTransform>();
        this.height = rect_transform.sizeDelta.y;    
        this.width  = rect_transform.sizeDelta.x;    
    }

    public float GetHeight()
    {
        return this.height;
    }

    public float GetWidth()
    {
        return this.width;
    }

    public void AddToYPosition(float y)
    {
        this.rect_transform.localPosition += new Vector3(0, y, 0);
    }

    public void AddToXPosition(float x)
    {
        this.rect_transform.localPosition += new Vector3(x, 0, 0);
    }

    public void SetCards(CardData[] cards)
    {
        this.cards = cards;
    }

    public void DisplayCards()
    {
        float offset = -((width) / 2.0f);
        foreach (CardData card in cards)
        {
            UI_PlayerStatInsectDisplayPrefab prefab =
                Instantiate<UI_PlayerStatInsectDisplayPrefab>(display_prefab,
                    this.rect_transform);

            offset += prefab.GetWidth();


            prefab.SetImage(card.image);
            prefab.SetName(card.name);

            prefab.SetXPosition(offset);
            prefab.AddToYPosition(-64.0f);

            offset += 32.0f;
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
