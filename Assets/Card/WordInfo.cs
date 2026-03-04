using UnityEngine;

[CreateAssetMenu(menuName = "Card/WordInfo")]
public class WordInfo : ScriptableObject
{
    public string word;
    [TextArea(3,10)]
    public string info;
}