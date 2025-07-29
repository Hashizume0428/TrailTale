using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using LocationLibrary;
using Cysharp.Threading.Tasks;

public class LocationManager : MonoBehaviour
{
    /// <summary>
    /// 緯度（経度）１度の距離（m）
    /// </summary>
    private const float Lat2Meter = 111319.491f;

    [SerializeField]
    private string GoogleApiKey;

    // [SerializeField]
    // private LatLng mockLocation;

    // [SerializeField]
    // private RawImage mapImage;

    private LocationInfo prevLocation;

    public int currentLogPointsIndex = 0;

    public LogPathRenderer logPathRenderer;
    
    public MapLoader mapLoader;

    private async void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("START : " + Time.time);
        Input.location.Start(3f);
        Input.compass.enabled = true;

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled by the user.");
            await UniTask.Delay(2000);
        }

        // Locationサービスが開始されるまで待機
        Debug.Log("Waiting for location service to start...");
        while (Input.location.status != LocationServiceStatus.Running)
        {
            await UniTask.Delay(1000);
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            Debug.Log("Generating map with current location...");
            await UniTask.Delay(500);
            LatLng currentLocation = new LatLng{latitude = Input.location.lastData.latitude, longitude = Input.location.lastData.longitude};
            //LoadMap(currentLocation, 14);
        }
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

    // public void SetCurrentLogPointsIndex(int index)
    // {
    //     currentLogPointsIndex += index;
    //     currentLogPointsIndex = Mathf.Clamp(currentLogPointsIndex, 0, logPathRenderer.GetAllLogPath().Count - 1);
    //     GenerateMap();
    // }

    // public void GenerateMap()
    // {
    //     logPathRenderer.ClearLogPath();
    //     List<Vector2> threeLogPath = logPathRenderer.GetThreeLogPath(currentLogPointsIndex);
    //     if (threeLogPath == null || threeLogPath.Count == 0)
    //     {
    //         Debug.LogWarning("No log path available.");
    //         return;
    //     }

    //     MapBounds bounds = logPathRenderer.CalculateBounds(threeLogPath);
    //     LatLng center = logPathRenderer.CalculateCenter(threeLogPath);
    //     int zoom = logPathRenderer.CalculateZoom(bounds);

    //     LoadMap(center, zoom);
    //     logPathRenderer.DrawLogPath(bounds, threeLogPath);

    // }

    // public async void LoadMap(LatLng latLng, int zoom)
    // {
    //     // ベース URL
    //     string url = @"https://maps.googleapis.com/maps/api/staticmap?";
    //     // 中心座標
    //     url += "center=" + latLng.latitude + "," + latLng.longitude;
    //     // ズーム
    //     url += "&zoom=" + zoom;
    //     // 画像サイズ（640x640まで）
    //     url += "&size=" + 512 + "x" + 512;
    //     // API Key（Google Maps Platform で発行されるキー）
    //     url += "&key=" + GoogleApiKey;

    //     // url += "&style=feature:all|element:geometry|color:0xe0e0e0";
    //     // url += "&style=feature:landscape|element:geometry.fill|color:0xdcd2c8";
    //     url += "&style=feature:road|element:geometry|visibility:simplified";
    //     url += "&style=feature:poi|element:labels|visibility:off";
    //     url += "&style=feature:administrative|element:labels|visibility:off";
    //     url += "&style=element:labels|visibility:off";
    //     url += "&maptype=satellite";

    //     Debug.Log("Map URL: " + url);

    //     MapLoader mapLoader = new MapLoader();

    //     Texture2D mapTexture = await mapLoader.LoadMapAsync(url);

    //     mapImage.texture = mapTexture;
    // }
}
