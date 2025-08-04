using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Cysharp.Threading.Tasks;
using LocationLibrary;

public class NearByLocation : MonoBehaviour
{
    [SerializeField]
    private string GoogleApiKey;

    public ResponseData responseData { get; private set; }

    private string CreateRequestJson(LatLng location)
    {
        var request = new RequestData
        {
            maxResultCount = 10,
            rankPreference = "DISTANCE",
            locationRestriction = new LocationRestriction
            {
                circle = new Circle
                {
                    center = location,
                    radius = 100.0f
                }
            }
        };

        return JsonUtility.ToJson(request);
    }

    /// <summary>
    /// 指定した位置の近くの場所を検索します。
    /// </summary>
    /// <param name="location"></param>
    public async UniTask<PlaceData> SearchNearByLocation(LatLng location)
    {
        string requestJson = CreateRequestJson(location);

        string url = "https://places.googleapis.com/v1/places:searchNearby";
        Debug.Log(requestJson);

        byte[] postData = Encoding.UTF8.GetBytes(requestJson);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Goog-Api-Key", GoogleApiKey);
        request.SetRequestHeader("X-Goog-FieldMask", "places.displayName,places.location");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response:\n" + request.downloadHandler.text);
            responseData = ResponseParser.ParseResponse(request.downloadHandler.text);

            if (responseData.places == null || responseData.places.Length == 0)
            {
                Debug.LogWarning("付近のランドマークを発見できませんでした。");
                return null;
            }

            for (int i = 0; i < responseData.places.Length; i++)
            {
                Debug.Log($"Place {i + 1}: {responseData.places[i].displayName.text}");
            }

            // 見つかったランドマーク名をランダムで返す
            int randomIndex = Random.Range(0, Mathf.Min(responseData.places.Length, 3)); // 最初の3つのランドマークからランダムに選択
            return responseData.places[randomIndex];    // ランダムに選ばれたランドマークを返す
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            return null;
        }
    }
}
