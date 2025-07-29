using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LocationLibrary;

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
#elif UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "Data/Logs/LocationLog.txt");
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
    
    public List<LatLng> ParseLogToLatLng(string log)
    {
        List<LatLng> points = new List<LatLng>();

        var lines = log.Split('\n');

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 2) continue;

            double lat = double.Parse(parts[0]);
            double lon = double.Parse(parts[1]);

            points.Add(new LatLng(lat, lon));
        }

        return points;
    }
}
