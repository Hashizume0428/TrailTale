using UnityEngine;
using System;
using System.Collections.Generic;
using EventLibrary;
using LocationLibrary;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    // プレイヤーのステータスデータ
    public int atk;
    public int def;
    public int hp;
    public int spd;
    public int mp;

    // 移動経路とイベントが発生する位置のリストを保持するクラス
    public LatLngList latLngList;

    // プレイヤーのインベントリデータ
    public int healPotion_S;
    public int healPotion_M;
    public int healPotion_L;

    public int currentStoryIndex = 0; // 現在のマップインデックス
    public string currentLog = ""; // 現在のログ

    public int battleType = 0;

    // ステータスを更新する
    public void UpdateStatus(StatusType statusType, int value)
    {
        Load();

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
            case StatusType.MP:
                mp += value;
                break;
            default:
                Debug.LogWarning("Unknown status type: " + statusType);
                break;
        }

        Save();
    }

    public int GetStatus(StatusType statusType)
    {
        Load();

        switch (statusType)
        {
            case StatusType.ATK:
                return atk;
            case StatusType.DEF:
                return def;
            case StatusType.HP:
                return hp;
            case StatusType.SPD:
                return spd;
            case StatusType.MP:
                return mp;
            default:
                Debug.LogWarning("Unknown status type: " + statusType);
                return 0;
        }
    }

    // インベントリを更新する
    public void UpdateInventory(ItemType itemType, int amount)
    {
        Load();

        Debug.Log($"Updating inventory: {itemType} by {amount}");
        switch (itemType)
        {
            case ItemType.HealPotion_S:
                healPotion_S += amount;
                break;
            case ItemType.HealPotion_M:
                healPotion_M += amount;
                break;
            case ItemType.HealPotion_L:
                healPotion_L += amount;
                break;
            default:
                Debug.LogWarning("Unknown item type: " + itemType);
                break;
        }

        Save();
    }

    public int GetItemAmount(ItemType itemType)
    {
        Load();

        switch (itemType)
        {
            case ItemType.HealPotion_S:
                return healPotion_S;
            case ItemType.HealPotion_M:
                return healPotion_M;
            case ItemType.HealPotion_L:
                return healPotion_L;
            default:
                Debug.LogWarning("Unknown item type: " + itemType);
                return 0;
        }
    }

    public int GetCurrentStoryIndex()
    {
        Load();
        return currentStoryIndex;
    }

    public void SetCurrentStoryIndex(int index)
    {
        currentStoryIndex = index;
        Save();
    }

    public string GetCurrentLog()
    {
        Load();
        return currentLog;
    }

    public void SetCurrentLog(string log)
    {
        currentLog = log;
        Save();
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

/// <summary>
/// LatLngのリストをJSONに変換するためのクラス
/// イベントが発生するかどうかのフラグも含めてJSONに変換
/// </summary>
[Serializable]
public class LatLngList
{
    public List<LatLng> latLngs;
    public List<int> eventOccurred;

    public LatLngList(List<LatLng> latLngs, List<int> eventOccurred)
    {
        this.latLngs = latLngs;
        this.eventOccurred = eventOccurred;
    }
}

