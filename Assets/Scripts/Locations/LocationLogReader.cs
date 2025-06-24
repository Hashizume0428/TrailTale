using UnityEngine;
using System.IO;

public class LocationLogReader
{
    public string Read()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
        string path = GetAndroidLogPath();
        if (File.Exists(path))
        {
            string line = File.ReadAllText(path);
            return line;
        }
        else
        {
            Debug.LogWarning("ログファイルが存在しません: " + path);
        }
    #endif
        return null;
    }

    public void Clear()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        string path = GetAndroidLogPath();
        if (File.Exists(path)) {
            File.WriteAllText(path, "");  // 空文字で上書き
            Debug.Log("ログファイルをクリアしました");
        }
        #endif
    }

    private string GetAndroidLogPath()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                AndroidJavaObject file = activity.Call<AndroidJavaObject>("getExternalFilesDir", (string)null);
                string dir = file.Call<string>("getAbsolutePath");
                return Path.Combine(dir, "location_log.txt");
            }
        }
    }
}
