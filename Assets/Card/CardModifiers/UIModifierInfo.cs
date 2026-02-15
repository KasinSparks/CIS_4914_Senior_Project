using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIModifierInfo : MonoBehaviour
{
    private TextMeshProUGUI description_text;
    private Image image;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Get the UI description gameobject
        this.description_text = this.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        if (this.description_text == null )
        {
            throw new MissingReferenceException("Unable to find the Description UI element for the UI_ModifierInfoRef.");
        }

        // Get the UI Image gameobject
        this.image = this.transform.Find("Image").GetComponent<Image>();
        if (this.image == null )
        {
            throw new MissingReferenceException("Unable to find the Image UI element for the UI_ModifierInfoRef.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetName(string name)
    {
        this.name = "UI_Info_" + name;
    }

    public void SetDescription(string description)
    {
        this.description_text.text = description;
    }

    public void SetImage(Sprite sprite)
    {
        this.image.sprite = sprite;
    }

    public void SetImage(Texture2D texture)
    {
        this.image.sprite = Sprite.Create(
            texture,
            new Rect(
                0.0f,
                0.0f,
                texture.width,
                texture.height
            ),
            new Vector2(0.5f, 0.5f),
            100.0f
        );
    }

    public Vector2 GetRectSize()
    {
        return this.GetComponent<RectTransform>().sizeDelta;
    }
}
