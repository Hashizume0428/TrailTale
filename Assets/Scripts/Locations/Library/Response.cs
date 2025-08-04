namespace LocationLibrary
{
    using System;
    using UnityEngine;
    using LocationLibrary;

    /// <summary>
    /// AIからのレスポンスをJSON形式で解析してResponseContentに変換します。
    /// </summary>
    public class ResponseParser : MonoBehaviour
    {
        public static ResponseData ParseResponse(string json)
        {
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(json);
            return responseData;
        }
    }
    
    /// <summary>
    /// NearBySearchの結果を格納するクラス
    /// </summary>
    [Serializable]
    public class ResponseData
    {
        public PlaceData[] places;
    }

    [Serializable]
    public class PlaceData
    {
        public DisplayName displayName;
        public LatLng location;
    }

    [Serializable]
    public class DisplayName
    {
        public string text;
        public string languageCode;
    }
}

