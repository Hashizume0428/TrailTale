using UnityEngine;
using System.Collections.Generic;
using LocationLibrary;

public class LogPathRenderer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject pointObject;

    private List<GameObject> pointObjects = new List<GameObject>();

    public void ClearLogPath()
    {
        // 既存のポイントオブジェクトを削除
        if (pointObjects.Count > 0)
        {
            pointObjects.ForEach(obj => Destroy(obj));
            pointObjects.Clear();
        }

        // LineRendererの設定をクリア
        lineRenderer.positionCount = 0;
        Debug.Log("Log path cleared.");

        var logReader = new LocationLogReader();
        logReader.Clear();
    }

    public void DrawLogPath()
    {
        // 既存のポイントオブジェクトを削除
        if (pointObjects.Count > 0)
        {
            pointObjects.ForEach(obj => Destroy(obj));
            pointObjects.Clear();
        }


        var logReader = new LocationLogReader();
        string log = logReader.Read();
        if (log == null)
        {
            Debug.LogWarning("ログが読み込めませんでした。");
            return;
        }
        else
        {
            Debug.Log($"Log:{log}");
        }

        List<Vector2> points = ParseLogToVector(log);
        MapBounds bounds = CalculateBounds(points);
        Debug.Log($"Bounds: {bounds.MinLat}, {bounds.MinLon} - {bounds.MaxLat}, {bounds.MaxLon}");

        List<Vector3> worldPositions = new List<Vector3>();
        foreach (var point in points)
        {
            Vector3 worldPos = LatLonToMapPosition(point, bounds);
            worldPositions.Add(worldPos);
        }

        // ポイントオブジェクトを生成
        foreach (var pos in worldPositions)
        {
            GameObject pointObj = Instantiate(pointObject, pos, Quaternion.identity);
            pointObjects.Add(pointObj);
        }

        // LineRendererの設定
        lineRenderer.positionCount = worldPositions.Count;
        lineRenderer.SetPositions(worldPositions.ToArray());
        Debug.Log($"LineRenderer: {lineRenderer.positionCount} points set.");
    }

    private List<Vector2> ParseLogToVector(string log)
    {
        List<Vector2> points = new List<Vector2>();

        var lines = log.Split('\n');

        float baseLat = 0, baseLon = 0;
        bool first = true;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 2) continue;

            float lat = float.Parse(parts[0]);
            float lon = float.Parse(parts[1]);

            if (first)
            {
                //baseLat = lat;
                //baseLon = lon;
                first = false;
            }

            // 地球1度あたりの距離からおおよそメートル換算
            //float x = (lon - baseLon);
            //float y = (lat - baseLat);
            float x = lon;
            float y = lat;

            points.Add(new Vector2(x, y));
        }

        return points;
    }

    // 地図の境界を計算
    private MapBounds CalculateBounds(List<Vector2> points)
    {
        if (points.Count == 0)
        {
            return new MapBounds(0, 0, 0, 0);
        }

        float minLat = float.MaxValue;
        float minLon = float.MaxValue;
        float maxLat = float.MinValue;
        float maxLon = float.MinValue;

        foreach (var point in points)
        {
            float lat = point.y;
            float lon = point.x;

            if (lat < minLat) minLat = lat;
            if (lon < minLon) minLon = lon;
            if (lat > maxLat) maxLat = lat;
            if (lon > maxLon) maxLon = lon;
        }

        return new MapBounds(minLat, minLon, maxLat, maxLon);
    }

    private Vector3 LatLonToMapPosition(Vector2 point, MapBounds bounds)
    {
        float x = Mathf.InverseLerp(bounds.MinLon, bounds.MaxLon, point.x) - 0.5f;
        float y = Mathf.InverseLerp(bounds.MinLat, bounds.MaxLat, point.y) - 0.5f;

        return new Vector3(x * Constants.MAP_SIZE, y * Constants.MAP_SIZE, 0);
    }
}