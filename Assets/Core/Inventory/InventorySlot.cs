using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public GameObject background;
    public Image itemIcon;
    public TextMeshProUGUI itemAmount;

    public SOItem currentItem;
    public int currentAmount = 0;

    public Color selectedColor, notSelectedColor;

    private void Start()
    {
        if (currentItem == null)
        {
            itemIcon.gameObject.SetActive(false);
            itemAmount.gameObject.SetActive(false);
        }
    }

    public void AddItem(SOItem item, int amount)
    {
        if(currentItem == null) currentItem = item;

        itemIcon.sprite = item.itemUIIcon;
        itemIcon.gameObject.SetActive(true);

        currentAmount += amount;
        itemAmount.text = currentAmount.ToString();
        itemAmount.gameObject.SetActive(true);
    }

    public void RemoveItemComplete(SOItem item)
    {
        currentItem = null;
        currentAmount = 0;
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);
        itemAmount.text = "0";
        itemAmount.gameObject.SetActive(false);
    }

    public void Select()
    {
        background.GetComponent<Image>().color = selectedColor;
    }

    public void Deselect()
    {
        background.GetComponent<Image>().color = notSelectedColor;
    }

    public SOItem GetInvItem() => currentItem;
    public int GetCurrentAmount() => currentAmount;
}
