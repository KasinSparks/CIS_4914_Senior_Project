using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatInsectDisplayPrefab : MonoBehaviour
{
    [SerializeField]
    private Image card_image;
    [SerializeField]
    private TextMeshProUGUI card_name;

    private RectTransform rect_transform;

    void Awake()
    {
        this.rect_transform = GetComponent<RectTransform>(); 
    }

    public float GetWidth()
    {
        return this.rect_transform.sizeDelta.x;
    }

    public void SetXPosition(float x)
    {
        this.rect_transform.localPosition = new Vector3(
            x,
            this.rect_transform.localPosition.y,
            this.rect_transform.localPosition.z
        );
    }

    public void SetName(string name)
    {
        this.card_name.text = name;
    }

    public void SetImage(Texture image)
    {
        this.card_image.sprite = Sprite.Create(
            (Texture2D) image,
            new Rect(
                0.0f,
                0.0f,
                image.width,
                image.height
            ),
            new Vector2(0.5f, 0.5f),
            100.0f
        );
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
