using UnityEngine;

public class ConsumableHelper
{
    public string consumable_type;
    public ScriptableObject consumable;
}

public interface IConsumableSavable
{
    public string ToJson();
    public ConsumableHelper FromJson(string json);
}