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
    private RawImage mapImage;

    private LocationInfo prevLocation;

    private async void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("START : " + Time.time);
        Input.location.Start(3f);
        Input.compass.enabled = true;

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled by the user.");
            await Task.Delay(2000);
        }

        // Locationサービスが開始されるまで待機
        Debug.Log("Waiting for location service to start...");
        while (Input.location.status != LocationServiceStatus.Running)
        {
            await Task.Delay(1000);
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            Debug.Log("Generating map with current location...");
            await Task.Delay(500);
            GenerateMap(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }
#else
        Debug.Log("Using Mock Location");

        GenerateMap(mockLocation.Latitude, mockLocation.Longitude);
#endif
    }

    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            string locationInfo = "Latitude: " + Input.location.lastData.latitude.ToString() + "\n" +
                "Longitude: " + Input.location.lastData.longitude.ToString() + "\n" +
                "Altitude: " + Input.location.lastData.altitude.ToString() + "\n" +
                "Horizontal Accuracy: " + Input.location.lastData.horizontalAccuracy.ToString() + "\n" +
                "Vertical Accuracy: " + Input.location.lastData.verticalAccuracy.ToString() + "\n" +
                "Timestamp: " + Input.location.lastData.timestamp.ToString();

            prevLocation = Input.location.lastData;

            if (getDistanceFromLocation(Input.location.lastData, prevLocation) > 2f)
            {
                string distanceInfo = "Distance: " + getDistanceFromLocation(Input.location.lastData, prevLocation).ToString() + "m\n" +
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

    private async void GenerateMap(float lat, float lon)
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

        url += "&style=feature:all|element:geometry|color:0xe0e0e0";
        url += "&style=feature:landscape|element:geometry.fill|color:0xdcd2c8";
        url += "&style=feature:road|element:geometry|visibility:simplified";
        url += "&style=feature:poi|element:labels|visibility:off";
        url += "&style=feature:administrative|element:labels|visibility:off";
        url += "&style=element:labels|visibility:off";

        Debug.Log("Map URL: " + url);

        MapLoader mapLoader = new MapLoader();

        Texture2D mapTexture = await mapLoader.LoadMapAsync(url);

        mapImage.texture = mapTexture;
    }
}
