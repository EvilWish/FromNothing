using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the player's inventory, including adding items, selecting items, and accessing the currently held item.
/// Handles hotbar selection via number keys and mouse wheel.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    #region Singleton

    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region Inventory Setup

    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public int selectedSlot = -1;
    private int scrollCounter = 1;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        ChangeHotbar();
    }

    #endregion

    #region Hotbar Management

    /// <summary>
    /// Handles changing the selected hotbar slot using number keys (1-8) and mouse wheel scrolling.
    /// </summary>
    private void ChangeHotbar()
    {
        // Number key input for hotbar selection
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 9)
            {
                ChangeSelectedSlot(number - 1);
            }
        }

        // Mouse wheel scrolling for hotbar selection
        float scrollValue = Mouse.current.scroll.ReadValue().y;
        if (scrollValue > 0)
        {
            if (scrollCounter > 0 && scrollCounter < 9)
            {
                scrollCounter++;
                if (scrollCounter == 9)
                {
                    scrollCounter = 1;
                }
                ChangeSelectedSlot(scrollCounter - 1);
            }
        }
        else if (scrollValue < 0)
        {
            if (scrollCounter > 0 && scrollCounter < 9)
            {
                scrollCounter--;
                if (scrollCounter == 0)
                {
                    scrollCounter = 8;
                }
                ChangeSelectedSlot(scrollCounter - 1);
            }
        }
    }

    /// <summary>
    /// Changes the currently selected inventory slot, updating the UI selection.
    /// </summary>
    /// <param name="newValue">The index of the new slot to select.</param>
    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    #endregion

    #region Item Management

    /// <summary>
    /// Adds an item to the inventory. Tries to stack with existing items first, then adds to a new empty slot.
    /// </summary>
    /// <param name="item">The BaseItem to add.</param>
    /// <param name="amountToAdd">The quantity of the item to add.</param>
    /// <returns>True if the item was successfully added, false otherwise (inventory full).</returns>
    public bool AddItem(SOItem item, int amountToAdd)
    {
        int remainingAmountToAdd = amountToAdd;
        // Search for existing stacks of the same item with space
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null &&
                itemInSlot.item == item &&
                itemInSlot.item.canItemStack)
            {
                int spaceInStack = itemInSlot.item.maxItemStackSize - itemInSlot.count;
                if (spaceInStack > 0)
                {
                    int canAdd = Mathf.Min(remainingAmountToAdd, spaceInStack);
                    itemInSlot.count += canAdd;
                    itemInSlot.RefreshCount();
                    remainingAmountToAdd -= canAdd;
                    if (remainingAmountToAdd <= 0)
                    {
                        return true;
                    }
                }
            }
        }

        // Spawn new items in empty slots for the remaining amount
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null && remainingAmountToAdd > 0)
            {
                int amountToSpawn = Mathf.Min(remainingAmountToAdd, item.maxItemStackSize);
                SpawnNewItem(item, slot, amountToSpawn);
                remainingAmountToAdd -= amountToSpawn;
                if (remainingAmountToAdd <= 0)
                {
                    return true;
                }
            }
        }

        return remainingAmountToAdd <= 0; // Returns true if all amount was added
    }

    /// <summary>
    /// Instantiates a new inventory item prefab into the specified inventory slot.
    /// </summary>
    /// <param name="item">The BaseItem to represent.</param>
    /// <param name="slot">The InventorySlot to place the item in.</param>
    /// <param name="amount">The initial quantity of the item.</param>
    private void SpawnNewItem(SOItem item, InventorySlot slot, int amount)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item, amount);
    }

    /// <summary>
    /// Gets the item in the currently selected hotbar slot and optionally consumes one unit of it.
    /// </summary>
    /// <param name="use">If true, decreases the item count by one and destroys the item if the count reaches zero.</param>
    /// <returns>The BaseItem in the selected slot, or null if the slot is empty.</returns>
    public SOItem GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null)
        {
            SOItem item = itemInSlot.item;
            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }

    /// <summary>
    /// Gets the item in the currently selected hotbar slot and consumes a specified amount.
    /// </summary>
    /// <param name="amount">The amount of the item to use.</param>
    /// <returns>The BaseItem if enough quantity is available, otherwise null.</returns>
    public SOItem GetSelectedItemByAmount(int amount)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null)
        {
            SOItem item = itemInSlot.item;
            if (itemInSlot.count >= amount)
            {
                itemInSlot.count -= amount;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
                return item;
            }
            else
            {
                Debug.LogWarning("Not enough materials.");
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if the currently selected hotbar slot contains at least a certain amount of an item.
    /// </summary>
    /// <param name="amount">The required amount.</param>
    /// <returns>True if there are at least the specified amount, false otherwise.</returns>
    public bool HasEnoughItemCount(int amount)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        return itemInSlot != null && itemInSlot.count >= amount;
    }

    /// <summary>
    /// Gets the BaseItem in the currently selected hotbar slot without consuming it.
    /// </summary>
    /// <returns>The BaseItem in the selected slot, or null if empty.</returns>
    public SOItem GetHandItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        return itemInSlot?.item;
    }

    /// <summary>
    /// Gets the quantity of the item in the currently selected hotbar slot.
    /// </summary>
    /// <returns>The quantity of the item, or 0 if the slot is empty.</returns>
    public int GetHandItemQuantity()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        return slot.GetComponentInChildren<InventoryItem>()?.count ?? 0;
    }

    /// <summary>
    /// Removes the item from the currently selected hotbar slot entirely.
    /// </summary>
    public void PurgeHandItem(int amountToRemove)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot.count >= amountToRemove)
        {
            GetSelectedItemByAmount(amountToRemove);
        }
        else
        {
            if (itemInSlot != null)
            {
                Destroy(itemInSlot.gameObject);
            }
        }
    }

    #endregion
}