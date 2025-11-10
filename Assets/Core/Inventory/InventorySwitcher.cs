using UnityEngine;

public class InventorySwitcher : MonoBehaviour
{
    public bool isMainInvOpen;

    public GameObject mainInvGroup;

    // On Click Events
    public void CloseInventory()
    {
        mainInvGroup.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        isMainInvOpen = false;
        InputManager.Instance.isUIOpen = false;
    }

    // Inventory Open per Key
    public void OpenInventory()
    {
        if (isMainInvOpen)
        {
            InputManager.Instance.isUIOpen = false;
            mainInvGroup.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMainInvOpen = false;
        }
        else
        {
            InputManager.Instance.isUIOpen = true;
            mainInvGroup.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMainInvOpen = true;
        }
    }

}
