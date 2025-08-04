using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventLibrary;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private HeroController hero;

    [SerializeField]
    private InventoryUI[] inventoryUI;

    [SerializeField]
    private Item[] items;

    private void Start()
    {
        for (int i = 0; i < inventoryUI.Length; i++)
        {
            // UIの初期化
            int amount = playerData.GetItemAmount(items[i].itemType);
            inventoryUI[i].Init(this, items[i], amount);
        }
    }

    // アイテムの使用ボタンをクリックしたときの処理
    public void OnClickUseItemButton(ItemType itemType)
    {
        Item item = GetItemByType(itemType);
        if (item != null)
        {
            if (hero == null)
            {
                Debug.LogWarning("HeroController is null. Cannot apply item effect.");
                return;
            }
            // アイテムの使用処理
            item.Use(hero);
            playerData.UpdateInventory(itemType, -1);
        }
        else
        {
            Debug.LogWarning($"Item of type {itemType} not found in inventory.");
            return;
        }

        UpdateUI();
    }

    // アイテムタイプからアイテムを取得するメソッド
    public Item GetItemByType(ItemType itemType)
    {
        foreach (var item in items)
        {
            if (item.itemType == itemType)
            {
                return item;
            }
        }
        Debug.LogWarning($"Item of type {itemType} not found in inventory.");
        return null;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < inventoryUI.Length; i++)
        {
            int amount = playerData.GetItemAmount(items[i].itemType);
            inventoryUI[i].UpdateUI(amount);
        }
    }
}