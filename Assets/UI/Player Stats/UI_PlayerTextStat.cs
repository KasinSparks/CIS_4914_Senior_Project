using UnityEngine;

public class UI_PlayerTextStat : MonoBehaviour
{
    public TMPro.TextMeshProUGUI label;
    public TMPro.TextMeshProUGUI value;
    private string field_name;

    private static readonly string _dots =
"...............................................................................................................................";

    void Awake()
    {
        this.UpdateFields();
    }

    public void UpdateFields()
    {
        this.field_name = this.gameObject.name;

        // Use the gameobject name as the label
        this.label.text = this.field_name + _dots;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {}

    // Update is called once per frame
    void Update() {}

    public string GetName()
    {
        return this.field_name;
    }

    public void SetValue(string value)
    {
        this.value.text = value;
    }
}
