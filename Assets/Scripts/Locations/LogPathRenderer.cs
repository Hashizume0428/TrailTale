using UnityEngine;
using System.Collections.Generic;
using LocationLibrary;

public class LogPathRenderer : MonoBehaviour
{
    [SerializeField]
    private CanvasLineRenderer canvasLineRenderer;

    [SerializeField]
    private RectTransform mapRect;

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

        // CanvasLineRendererのラインを削除
        canvasLineRenderer.ClearLines();
    }

    public void DrawLogPath()
    {
        Debug.Log("Drawing log path...");
        
        ClearLogPath();

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

        // 緯度経度形式のログをVector2のリストに変換
        List<Vector2> points = ParseLogToVector(log);

        // マップの境界を計算
        MapBounds bounds = CalculateBounds(points);

        List<Vector2> canvasPositions = new List<Vector2>();
        foreach (var point in points)
        {
            Vector2 canvasPos = LatLonToMapPosition(point, bounds);
            Debug.Log($"canvasPosition: {canvasPos}");
            canvasPositions.Add(canvasPos);
        }

        // CanvasLineRendererを使用してラインを描画
        canvasLineRenderer.WriteLine(canvasPositions, mapRect.transform);

        // ポイントオブジェクトを生成
        foreach (var pos in canvasPositions)
        {
            GameObject pointObj = Instantiate(pointObject, mapRect.anchoredPosition, Quaternion.identity, mapRect.transform);
            pointObj.GetComponent<RectTransform>().anchoredPosition = pos;
            pointObjects.Add(pointObj);
        }
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
            Debug.Log($"Point: {point}");
            float lat = point.y;
            float lon = point.x;

            if (lat < minLat) minLat = lat;
            if (lon < minLon) minLon = lon;
            if (lat > maxLat) maxLat = lat;
            if (lon > maxLon) maxLon = lon;
        }

        Debug.Log($"Updated Bounds: {minLat}, {minLon} - {maxLat}, {maxLon}");

        return new MapBounds(minLat, minLon, maxLat, maxLon);
    }

    private Vector2 LatLonToMapPosition(Vector2 point, MapBounds bounds)
    {
        float x = Mathf.InverseLerp(bounds.MinLon, bounds.MaxLon, point.x) - 0.5f;
        float y = Mathf.InverseLerp(bounds.MinLat, bounds.MaxLat, point.y) - 0.5f;

        return new Vector2(x * mapRect.rect.width/2, y * mapRect.rect.height/2);
    }
}