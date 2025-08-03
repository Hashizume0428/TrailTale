using UnityEngine;
using EventLibrary;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Items/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField]
    private List<Item> items;

    public List<Item> GetItems()
    {
        return items;
    }

    public Item GetItemByType(ItemType itemType)
    {
        return items.Find(item => item.itemType == itemType);
    }
}