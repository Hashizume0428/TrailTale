using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayStatus : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData; // プレイヤーデータ

    [SerializeField]
    private StatusDataBase statusDataBase; // ステータスデータベース 

    public Image[] icons;
    public TextMeshProUGUI[] texts;



    private void Start()
    {
        var statuses = statusDataBase.GetStatuses();
        for (int i = 0; i < icons.Length; i++)
        {
            // ステータスのアイコンを設定
            icons[i].sprite = statuses[i].icon;
            // ステータスの値を設定
            texts[i].text = statuses[i].name + ": " + playerData.GetStatus(statuses[i].statusType).ToString();
        }
    }
}
