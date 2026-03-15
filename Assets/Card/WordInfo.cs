using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/WordInfo")]
public class WordInfo : ScriptableObject, ISavable
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

    public JsonValue ToJsonObject()
    {
        JsonObject json_obj = new JsonObject();

        json_obj.value.Add("Type", new JsonString() { value = this.GetType().ToString() });

        JsonObject json_data = new JsonObject();

        JsonArray word_array = new JsonArray();
        foreach (string word in this.words)
        {
            word_array.value.Add(new JsonString() { value = word });
        }


        json_data.value.Add("words", word_array);
        json_data.value.Add("info", new JsonString() { value = this.info });

        JsonObject image_data =
            SaveSystemTable.GetJsonForTexture2D((Texture2D)this.image);
        json_data.value.Add("image", image_data);
        
        json_obj.value.Add("Data", json_data);

        return json_obj;
    }

    public void OverrideValuesFromJson(JsonValue json)
    {
        JsonObject data = (JsonObject) json;
        JsonArray words = (JsonArray)data["words"];
        this.words = new string[words.value.Count];
        for (int i = 0; i <  words.value.Count; ++i)
        {
            this.words[i] = ((JsonString)words[i]).value;
        }

        this.info = ((JsonString)data["info"]).value;

        JsonObject image_data = (JsonObject)data["image"];

        this.image = SaveSystemTable.GetTexture2DFromJsonImage(image_data);
    }
}