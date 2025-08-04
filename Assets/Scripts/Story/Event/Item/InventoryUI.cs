using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventLibrary;

public class InventoryUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemAmount;
    public Button useButton;

    public void Init(InventoryManager inventoryManager, Item item, int amount)
    {
        itemIcon.sprite = item.icon;
        UpdateUI(amount);

        useButton.onClick.AddListener(() => inventoryManager.OnClickUseItemButton(item.itemType));
    }

    public void UpdateUI(int amount)
    {
        itemAmount.text = amount.ToString();

        if (amount > 0)
        {
            useButton.interactable = true;
        }
        else
        {
            useButton.interactable = false;
        }
    }
}