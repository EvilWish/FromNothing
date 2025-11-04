using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [Header("Inputs")]
    public List<InventorySlot> toolbar;
    public PlayerData playerData;


    private Dictionary<SOItem, int> playerItems;
    private int currentSelected = -1;
    private int scrollCounter = 1;

    #region Instance
    public static InventoryManager Instance;
    private bool hasSpace = true;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    #endregion

    #region Unity
    private void Start()
    {
        RescueInit();
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame) DebugItemInv();
        ToolbarSelection();
    }
    #endregion

    #region Toolbar Interaction
    void ToolbarSelection()
    {
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
                ChangeSelectedSlot(scrollCounter);
            }
        }
    }

    void ChangeSelectedSlot(int slotID)
    {
        if(currentSelected >= 0)
        {
            toolbar[slotID].Deselect();
        }
        toolbar[slotID].Select();
        currentSelected = slotID;
    }

    #endregion

    #region ItemHandling
    public void AddItemToInventory(SOItem item, int amount)
    {
        if (playerItems == null) RescueInit();

        if (CanCarry(item))
        {
            if (hasSpace)
            {
                if (playerItems.ContainsKey(item))
                {
                    playerItems[item] += amount;
                    playerData.AddCurrentCarryWeight(item.itemWeight);
                    //UI
                    AddItemToSlot(item, amount);
                }
                else
                {
                    playerItems.Add(item, amount);
                    playerData.AddCurrentCarryWeight(item.itemWeight);
                    //UI
                    AddItemToSlot(item, amount);
                }
            }
            else
            {
                // Debug only first
                Debug.LogError("[Item-ERR] Not enough space left!");
                return;
            }
        }
        else
        {
            // Debug only first
            Debug.LogError($"[Item-ERR] Not enough carryWeight left! -> [{item.itemName} x {amount} || left: {playerData.GetCurrentCarryWeight()}]");
            return;
        }
    }

    private void AddItemToSlot(SOItem item, int amount)
    {
        InventorySlot slot = GetSameItem(item, amount);
        if (hasSpace)
        {
            if (slot.currentItem != null)
            {
                if (slot.currentAmount < item.maxItemStackSize - amount)
                {
                    slot.AddItem(item, amount);

                }
                else
                {
                    Debug.Log("[Item-Info] New Stack while old was full");
                    slot = FindFirstEmpty();
                    slot.AddItem(item, amount);
                }
            }
            else
            {
                slot.AddItem(item, amount);
            }
        }
    }

    void DebugItemInv()
    {
        foreach (SOItem item in playerItems.Keys)
        {
            Debug.Log($"Inventory Content -> {item.itemName} x{playerItems[item]}");
        }
    }
    #endregion

    #region Helper
    InventorySlot GetSameItem(SOItem item, int amount)
    {
        for (int i = 0; i < toolbar.Count; i++)
        {
            if (toolbar[i].currentItem == item && toolbar[i].currentAmount < item.maxItemStackSize - amount) return toolbar[i];
        }
        Debug.LogWarning("[ITEM-WA] No/Full Item with that Type Found -> Find first Free Slot");
        return FindFirstEmpty();
    }

    InventorySlot FindFirstEmpty()
    {
        for (int i = 0; i < toolbar.Count; i++)
        {
            if (toolbar[i].currentItem == null)
            {
                hasSpace = true;
                return toolbar[i];
            }
        }

        Debug.LogError("[ITEM-ERR] No Free inventory Space going to Fallback -> TODO");
        hasSpace = false;
        return null;
    }

    public bool CanCarry(SOItem item)
    {
        float weightLeft = playerData.GetMaxCarryWeight() - playerData.GetCurrentCarryWeight();
        if (weightLeft < item.itemWeight)
            return false;
        return true;
    }

    public bool HasSpace() => hasSpace;

    void RescueInit()
    {
        playerItems = new Dictionary<SOItem, int>();
        Debug.LogError($"[Item-ERR] Rescue Init for Inventory Dictionary. What happend?");
    }

    void LoadInventory()
    {
        //From PlayerSave Later
    }
    #endregion
}
