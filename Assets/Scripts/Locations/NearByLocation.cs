using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using LocationLibrary;

public class NearByLocation : MonoBehaviour
{
    [SerializeField]
    private string GoogleApiKey;

    void Start()
    {
        StartCoroutine("SearchNearByLocation");
    }

    private string CreateRequestJson()
    {
        var request = new NearBySearchRequest
        {
            maxResultCount = 10,
            rankPreference = "DISTANCE",
            locationRestriction = new LocationRestriction
            {
                circle = new Circle
                {
                    center = new LatLng
                    {
                        latitude = 34.7633,
                        longitude = 135.5011
                    },
                    radius = 100.0f
                }
            }
        };

        return JsonUtility.ToJson(request);
    }

    IEnumerator SearchNearByLocation()
    {
        string url = "https://places.googleapis.com/v1/places:searchNearby";
        string jsonBody = CreateRequestJson();
        Debug.Log(jsonBody);

        byte[] postData = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Goog-Api-Key", GoogleApiKey);
        request.SetRequestHeader("X-Goog-FieldMask", "places.displayName");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response:\n" + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }
}
