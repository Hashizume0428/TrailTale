using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayItem : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData; // プレイヤーデータ

    [SerializeField]
    private ItemDataBase itemDataBase; // アイテムデータベース

    public Image[] icons;
    public TextMeshProUGUI[] texts;



    private void Start()
    {
        var items = itemDataBase.GetItems();
        for (int i = 0; i < icons.Length; i++)
        {
            // ステータスのアイコンを設定
            icons[i].sprite = items[i].icon;
            // アイテムの値を設定
            texts[i].text = items[i].itemName + ": " + playerData.GetItemAmount(items[i].itemType).ToString();
        }
    }
}