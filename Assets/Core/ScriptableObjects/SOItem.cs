using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "FromNothing/Item/Create new Item", order = 0)]
public class SOItem : ScriptableObject
{
    [Header("Item Data")]
    public string itemName;
    public string itemDescription;
    public ItemType itemType;

    [Header("Item Stacking")]
    public bool canItemStack;
    [Range(1, 64)] public int maxItemStackSize;

    [Header("Item Weight")]
    [Range(0f, 32f)] public float itemWeight;

    [Header("Item Visuals")]
    public Sprite itemUIIcon;
    public bool useGlobalItemDropPrefab;
    public GameObject itemPrefab;

    [Header("Building")]
    public GameObject buildingPrefab;
    public Vector2Int buildingSize;

}
