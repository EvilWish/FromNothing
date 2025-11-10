using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public SOItem[] itemToPickup;


    public void PickupItem(int id)
    {
       bool result = inventoryManager.AddItem(itemToPickup[id], 1);


        if (result == true)

            Debug.Log("Item wurde erfolgreich hinzugefügt!");

        else
            Debug.Log("Item konnte nicht hinzugefügt werden!!!");
    }
}
