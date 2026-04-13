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

    public class WordInfoSaveData
    {
        public string[] words;
        public string info;
        public string image_resource_path;
        
        public WordInfoSaveData(WordInfo data)
        {
            this.words = new string[data.words.Length];
            for (int i = 0; i <  data.words.Length; ++i)
            {
                this.words[i] = data.words[i];
            }

            this.info = data.info;
            this.image_resource_path = data.image.name;
        }

        public static WordInfoSaveData FromJson(string json)
        {
            return JsonUtility.FromJson<WordInfoSaveData>(json);
        }
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(new WordInfoSaveData(this), true);
    }

    public static WordInfo FromJson(string json)
    {
        WordInfo ret = ScriptableObject.CreateInstance<WordInfo>();

        WordInfoSaveData raw_save_data = WordInfoSaveData.FromJson(json);

        ret.words = raw_save_data.words;
        ret.info  = raw_save_data.info;
        ret.image = Resources.Load<Texture>("Images/" + raw_save_data.image_resource_path);

        return ret;
    }
}