using UnityEngine;

[CreateAssetMenu(menuName = "Card/WordInfo")]
public class WordInfo : ScriptableObject
{
    [SerializeField]
    private string[] words;

    [TextArea(3,10)]
    [SerializeField]
    private string info;

    [SerializeField]
    private Texture image;
    
    public string[] GetWords()
    {
        return this.words;
    }

    public string GetInfo()
    {
        return this.info;
    }

    public Texture GetImage()
    {
        return this.image;
    }

    public Sprite GetSprite()
    {
        return Sprite.Create(
            (Texture2D) this.image,
            new Rect(
                0.0f,
                0.0f,
                this.image.width,
                this.image.height
            ),
            new Vector2(0.5f, 0.5f),
            100.0f
        );
    }
}