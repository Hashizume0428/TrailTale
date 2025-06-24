using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using LocationLibrary;

public class LocationManager : MonoBehaviour
{
    /// <summary>
    /// 緯度（経度）１度の距離（m）
    /// </summary>
    private const float Lat2Meter = 111319.491f;

    [SerializeField]
    private string GoogleApiKey;

    [SerializeField]
    private LocationData mockLocation;

    [SerializeField]
    private TextMeshProUGUI locationText;

    [SerializeField]
    private RawImage mapImage;

    [SerializeField]
    private TextMeshProUGUI debugText;

    private LocationInfo prevLocation;

    IEnumerator Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("START : " + Time.time);
        Input.location.Start(3f);
        Input.compass.enabled = true;

        if (!Input.location.isEnabledByUser)
        {
            locationText.text = "Location services are not enabled by the user.";
            yield return new WaitForSeconds(2);
        }

        while (Input.location.status != LocationServiceStatus.Running)
        {
            yield return new WaitForSeconds(0.3f);
            locationText.text = "Waiting for location services to initialize.";
            yield return new WaitForSeconds(0.3f);
            locationText.text = "Waiting for location services to initialize..";
            yield return new WaitForSeconds(0.3f);
            locationText.text = "Waiting for location services to initialize...";
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            debugText.text = "Generate Map";
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(GenerateMap(Input.location.lastData.latitude, Input.location.lastData.longitude));
        }
#else 
        yield return null;
        debugText.text = "Using Mock Location";
        StartCoroutine(GenerateMap(mockLocation.Latitude, mockLocation.Longitude));
#endif
    }

    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            locationText.text = "Latitude: " + Input.location.lastData.latitude.ToString() + "\n" +
                "Longitude: " + Input.location.lastData.longitude.ToString() + "\n" +
                "Altitude: " + Input.location.lastData.altitude.ToString() + "\n" +
                "Horizontal Accuracy: " + Input.location.lastData.horizontalAccuracy.ToString() + "\n" +
                "Vertical Accuracy: " + Input.location.lastData.verticalAccuracy.ToString() + "\n" +
                "Timestamp: " + Input.location.lastData.timestamp.ToString();

            prevLocation = Input.location.lastData;

            if (getDistanceFromLocation(Input.location.lastData, prevLocation) > 2f)
            {
                debugText.text = "Distance: " + getDistanceFromLocation(Input.location.lastData, prevLocation).ToString() + "m\n" +
                    "Heading: " + Input.compass.trueHeading.ToString() + "\n" +
                    "Timestamp: " + Input.location.lastData.timestamp.ToString();
            }
        }
    }

    private float getDistanceFromLocation(LocationInfo curr, LocationInfo prev)
    {
        Vector3 cv = new Vector3((float)curr.longitude, 0, (float)curr.latitude);
        Vector3 pv = new Vector3((float)prev.longitude, 0, (float)prev.latitude);
        return Vector3.Distance(cv, pv) * Lat2Meter;
    }

    private IEnumerator GenerateMap(float lat, float lon)
    {
        // ベース URL
        string url = @"https://maps.googleapis.com/maps/api/staticmap?";
        // 中心座標
        url += "center=" + lat + "," + lon;
        // ズーム
        url += "&zoom=" + 14; // デフォルトで 0 なので適当なサイズにしておく
        // 画像サイズ（640x640まで）
        url += "&size=" + 640 + "x" + 640;
        // API Key（Google Maps Platform で発行されるキー）
        url += "&key=" + GoogleApiKey;

        Debug.Log("Map URL: " + url);

        url = UnityWebRequest.UnEscapeURL(url);
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            debugText.text = "Map Downloaded";
            mapImage.texture = DownloadHandlerTexture.GetContent(req);
        }
        else
        {
            debugText.text = "Map Download Failed: " + req.error;
            yield break;
        }
    }
}
