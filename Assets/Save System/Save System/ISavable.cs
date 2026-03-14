using UnityEngine;

public interface ISavable
{
    public string ToJsonString();
    public JsonValue ToJsonObject();
    public void OverrideValuesFromJson(string json);

    //public void FromJsonOverride(string json, object obj_ref);
}
