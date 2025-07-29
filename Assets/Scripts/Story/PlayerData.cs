using UnityEngine;
using EventLibrary;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    public int atk;
    public int def;
    public int hp;
    public int spd;
    public int luck;

    // ステータスを更新する
    public void UpdateStatus(StatusType statusType, int value)
    {
        Debug.Log($"Updating status: {statusType} by {value}");
        switch (statusType)
        {
            case StatusType.ATK:
                atk += value;
                break;
            case StatusType.DEF:
                def += value;
                break;
            case StatusType.HP:
                hp += value;
                break;
            case StatusType.SPD:
                spd += value;
                break;
            case StatusType.LUCK:
                luck += value;
                break;
            default:
                Debug.LogWarning("Unknown status type: " + statusType);
                break;
        }
    }

    public void Load()
    {
        SaveManager.LoadPlayerData(this);
    }

    public void Save()
    {
        SaveManager.SavePlayerData(this);
    }

    public void Delete()
    {
        SaveManager.DeletePlayerData();
    }
}
