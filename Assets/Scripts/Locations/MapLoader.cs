using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LocationLibrary
{
    public class MapLoader
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private string dirPath => Path.Combine(Application.persistentDataPath, "MapCache");
#elif UNITY_EDITOR
        private string dirPath => Path.Combine(Application.dataPath, "Data/MapCache");
#endif

        /// <summary>
        /// マップ画像の保存キーを取得する(URLのハッシュ値を使用)
        /// </summary>
        public string GetCachePath(string url)
        {
            string hash = url.GetHashCode().ToString("x");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return Path.Combine(dirPath, $"mapcache_{hash}.png");
        }

        /// <summary>
        /// 指定したURLからマップ画像を非同期で読み込む
        /// </summary>
        public async UniTask<Texture2D> LoadMapAsync(string url)
        {
            string path = GetCachePath(url);

            if (File.Exists(path))
            {
                Debug.Log("Loading map from cache: " + path);
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);
                return tex;
            }
            else
            {
                UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
                await req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(req);

                    // キャッシュに保存
                    Debug.Log("Saving map to cache: " + path);
                    byte[] png = tex.EncodeToPNG();
                    File.WriteAllBytes(path, png);
                    return tex;
                }
                else
                {
                    Debug.LogError("Map Download Failed: " + req.error);
                    return null;
                }
            }
        }
    }
}
