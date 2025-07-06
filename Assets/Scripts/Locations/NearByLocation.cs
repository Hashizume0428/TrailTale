using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using LocationLibrary;
using Cysharp.Threading.Tasks;

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
    public async UniTask<string> SearchNearByLocation(LatLng location)
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
        request.SetRequestHeader("X-Goog-FieldMask", "places.displayName");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response:\n" + request.downloadHandler.text);
            responseData = ResponseParser.ParseResponse(request.downloadHandler.text);
            for (int i = 0; i < responseData.places.Length; i++)
            {
                Debug.Log($"Place {i + 1}: {responseData.places[i].displayName.text}");
            }

            // 見つかったランドマーク名をランダムで返す
            int randomIndex = Random.Range(0, responseData.places.Length);
            return responseData.places[randomIndex].displayName.text;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            return null;
        }
    }
}
