using UnityEngine;

public class PlaceCard : MonoBehaviour
{
    private Card selected_card;
    private int numRow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.numRow = 3;
        this.selected_card = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelectedCard(Card selected_card)
    {
        this.selected_card = selected_card;
        Debug.Log("Set selected card: " + selected_card.card_name);
    }

    public void PlaceSelectedCard()
    {
        for (int i = 0; i < numRow - 1; i++)
        {
            foreach (Transform child in this.transform)
            {
                //if child.name ==
                //Whatever w = childTransform.GetComponent<Whatever>();
                //w.DoThing();
            }
        }
        
    }
}
