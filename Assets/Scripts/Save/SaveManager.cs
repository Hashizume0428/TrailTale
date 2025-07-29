// SaveManager.cs
using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string saveDirectory;
    private const string playerFileName = "playerData.json";

    static SaveManager()
    {
        // データの保存場所を設定 (プラットフォームによってパスが異なります)
#if UNITY_ANDROID && !UNITY_EDITOR
        saveDirectory = Application.persistentDataPath;
        Debug.Log("Save Directory: " + saveDirectory);
#elif UNITY_EDITOR
        saveDirectory = Path.Combine(Application.dataPath, "Data/Player");
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
        Debug.Log("Save Directory: " + saveDirectory);
#endif
    }

    /// <summary>
    /// PlayerDataをJSON形式でファイルに保存します。
    /// </summary>
    /// <param name="playerData">保存するPlayerData ScriptableObject</param>
    public static void SavePlayerData(PlayerData playerData)
    {
        // ScriptableObjectの内容をJSON文字列に変換
        string json = JsonUtility.ToJson(playerData);
        string filePath = Path.Combine(saveDirectory, playerFileName);

        try
        {
            // ファイルに書き込む
            File.WriteAllText(filePath, json);
            Debug.Log($"Player data saved to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save player data: {e.Message}");
        }
    }

    /// <summary>
    /// ファイルからJSON形式のPlayerデータをロードし、既存のPlayerData ScriptableObjectに反映します。
    /// </summary>
    /// <param name="playerData">データをロードして反映するPlayerData ScriptableObject</param>
    /// <returns>ロードが成功した場合はtrue、失敗した場合はfalse</returns>
    public static bool LoadPlayerData(PlayerData playerData)
    {
        string filePath = Path.Combine(saveDirectory, playerFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No player data found at: {filePath}. Using default data.");
            return false;
        }

        try
        {
            // ファイルからJSON文字列を読み込む
            string json = File.ReadAllText(filePath);

            // JSON文字列を既存のScriptableObjectに反映
            JsonUtility.FromJsonOverwrite(json, playerData);
            Debug.Log($"Player data loaded from: {filePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load player data: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 保存されているPlayerデータのファイルを削除します。
    /// </summary>
    public static void DeletePlayerData()
    {
        string filePath = Path.Combine(saveDirectory, playerFileName);
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"Player data deleted from: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to delete player data: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("No player data file to delete.");
        }
    }
}