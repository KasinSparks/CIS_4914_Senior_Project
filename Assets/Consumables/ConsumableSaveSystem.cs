using UnityEngine;
using System.Collections.Generic;

public class ConsumableSaveSystem : MonoBehaviour
{
    [SerializeField]
    ConsumableButton[] buttons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // If the save file does not exists, load the default data
        ScriptableObject[] consumables = SaveSystem.LoadConsumablesFromSaveFile(SaveSystemFile.PlayerConsumables);
        if (consumables == null)
        {
            return;
        }
        
        // Load the consumables
        Queue<ScriptableObject> consumable_queue = new Queue<ScriptableObject>();
        foreach (ScriptableObject consumable in consumables)
        {
            consumable_queue.Enqueue(consumable);
        }

        // Load the consumable data from the save file
        foreach (ConsumableButton button in buttons)
        {
            
            // TODO(KASIN): I don't think this is the best way of doing this,
            //    but it will work for now.
            if (consumable_queue.Count > 0)
            {
                ScriptableObject curr_consumable = consumable_queue.Dequeue();
                switch (curr_consumable.GetType().Name)
                {
                    case "DamageAllOpponentsConsumable":
                        button.SetDamageConsumable((DamageAllOpponentsConsumable) curr_consumable);
                        break;
                    case "HealAllPlayerCardsConsumable":
                        button.SetHealConsumable((HealAllPlayerCardsConsumable) curr_consumable);
                        break;
                    case "AddCardToHandConsumable":
                        button.SetCardConsumable((AddCardToHandConsumable) curr_consumable);
                        break;
                    case "HealSingleCardConsumable":
                        button.SetSingleHealConsumable((HealSingleCardConsumable) curr_consumable);
                        break;
                }
            }
            else
            {
                button.SetConsumableToEmpty();
            }
        }
    }

    private void OnDestroy()
    {
        // Get the current consumables
        // This will allow the count for the newlines in the file to be accurate
        List<ScriptableObject> curr_consumables = new List<ScriptableObject>();
        foreach (ConsumableButton button in buttons)
        {
            if (button.GetConsumableAssigned() != null)
            {
                curr_consumables.Add(button.GetConsumableAssigned());
            }
        }

        SaveSystem.SaveConsumablesToFile(curr_consumables.ToArray(), SaveSystemFile.PlayerConsumables);
    }


}
