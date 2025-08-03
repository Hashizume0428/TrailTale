using UnityEngine;
using System.Collections.Generic;
using LocationLibrary;

public class LogPathRenderer : MonoBehaviour
{
    // [SerializeField]
    // private CanvasLineRenderer canvasLineRenderer;

    // [SerializeField]
    // private RectTransform mapRect;
    public LineRenderer lineRenderer;

    [SerializeField]
    private GameObject[] pointObject;

    [SerializeField]
    private PlaceObject placeObject;

    [SerializeField]
    private MapLoader mapLoader;

    public List<GameObject> pointObjects = new List<GameObject>();
    public List<int> eventPointList = new List<int>();
    public List<PlaceObject> placeObjects = new List<PlaceObject>();

    private List<Vector2> logPathPoints = new List<Vector2>();

    public void ClearLogPath()
    {
        // 既存のポイントオブジェクトを削除
        if (pointObjects.Count > 0)
        {
            pointObjects.ForEach(obj => Destroy(obj));
            pointObjects.Clear();
            eventPointList.Clear();
            placeObjects.ForEach(obj => Destroy(obj));
            placeObjects.Clear();
        }

        // // CanvasLineRendererのラインを削除
        // canvasLineRenderer.ClearLines();
    }

    // public List<Vector2> GetAllLogPath(string log)
    // {
    //     if (log == null)
    //     {
    //         Debug.LogWarning("ログが読み込めませんでした。");
    //         return null;
    //     }
    //     else
    //     {
    //         Debug.Log($"Log:{log}");

    //         // 緯度経度形式のログをVector2のリストに変換
    //         List<Vector2> points = ParseLogToVector(log);
    //         return points;
    //     }
    // }

    // // 現在地前後の3点のログパスを取得
    // public List<Vector2> GetThreeLogPath(int currentIndex)
    // {
    //     if (logPathPoints.Count == 0)
    //     {
    //         logPathPoints = GetAllLogPath();
    //     }

    //     // 現在のインデックスから3つのログパスを取得
    //     int startIndex = Mathf.Max(0, currentIndex - 1);
    //     int endIndex = Mathf.Min(logPathPoints.Count - 1, currentIndex + 1);
    //     List<Vector2> threeLogPath = logPathPoints.GetRange(startIndex, endIndex - startIndex + 1);

    //     return threeLogPath;
    // }

    public void DrawLogPath(List<LatLng> latLngList, List<int> storyIndexList)
    {
        ClearLogPath(); // 既存のポイントオブジェクトを削除

        List<Vector3> mapPositions = new List<Vector3>();

        foreach (var logVec in latLngList)
        {
            Debug.Log($"Log Vector: {logVec}");
            Vector3 mapPos = mapLoader.LatLonToUnityLocalPosition(logVec.latitude, logVec.longitude);
            mapPositions.Add(mapPos);
        }

        for (int i = 0; i < mapPositions.Count; i++)
        {
            int pointIndex = 0;

            // Eventが発生する地点の場合ポイントの見た目を変更
            if (storyIndexList.Contains(i))
            {
                pointIndex = 1;
                eventPointList.Add(i);
            }

            GameObject pointObj = Instantiate(pointObject[pointIndex], mapLoader.mapContainer.transform);
            pointObj.transform.localPosition = mapPositions[i];
            pointObjects.Add(pointObj);
        }

        // LineRendererを使用してラインを描画
        lineRenderer.positionCount = mapPositions.Count;
        for (int i = 0; i < mapPositions.Count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(mapPositions[i].x, 1, mapPositions[i].y)); // Y座標は1に固定
        }
    }

    // 移動経路上のタイルを取得する
    public List<Vector2Int> GetLogPathTiles()
    {
        int radius = 1;
        HashSet<Vector2Int> uniqueTiles = new HashSet<Vector2Int>();

        for (int i = 0; i < pointObjects.Count - 1; i++)
        {
            Vector3 curPos = pointObjects[i].transform.localPosition;
            Vector3 nextPos = pointObjects[i + 1].transform.localPosition;
            Debug.Log($"Current Position: {curPos}, Next Position: {nextPos}");
            float dx = nextPos.x - curPos.x;
            float dy = nextPos.y - curPos.y;

            if (Mathf.Abs(dx) > Mathf.Abs(dy))
            {
                if (dx > 0)
                {
                    // X方向に移動
                    float d = dy / dx; // Yの変化量をXの変化量で割る
                    for (int x = Mathf.RoundToInt(curPos.x); x <= nextPos.x; x++)
                    {
                        // dxが15以上の場合、はじめと最後の10タイル以外スキップ
                        if (dx >= 15)
                        {
                            if (x > 10 && x < nextPos.x - 10)
                            {
                                continue;
                            }
                        }
                        float y = curPos.y + (x - curPos.x) * d;
                        uniqueTiles.Add(new Vector2Int(x, Mathf.RoundToInt(y)));
                        Debug.Log($"Tile: ({x}, {y})");
                    }
                }
                else
                {
                    // X方向に逆移動
                    float d = dy / dx; // Yの変化量をXの変化量で割る
                    for (int x = Mathf.RoundToInt(nextPos.x); x <= curPos.x; x++)
                    {
                        if (dx <= -15)
                        {
                            if (x < curPos.x - 10 && x > nextPos.x + 10)
                            {
                                continue;
                            }
                        }
                        float y = nextPos.y + (x - nextPos.x) * d;
                        uniqueTiles.Add(new Vector2Int(x, Mathf.RoundToInt(y)));
                        Debug.Log($"Tile: ({x}, {y})");
                    }
                }
            }
            else
            {
                if (dy > 0)
                {
                    // Y方向に移動
                    float d = dx / dy; // Xの変化量をYの変化量で割る
                    for (int y = Mathf.RoundToInt(curPos.y); y <= nextPos.y; y++)
                    {
                        // dyが15以上の場合、はじめと最後の10タイル以外スキップ
                        if (dy >= 15)
                        {
                            if (y > 10 && y < nextPos.y - 10)
                            {
                                continue;
                            }
                        }
                        float x = curPos.x + (y - curPos.y) * d;
                        uniqueTiles.Add(new Vector2Int(Mathf.RoundToInt(x), y));
                        Debug.Log($"Tile: ({x}, {y})");
                    }
                }
                else
                {
                    // Y方向に逆移動
                    float d = dx / dy; // Xの変化量をYの変化量で割る
                    for (int y = Mathf.RoundToInt(nextPos.y); y <= curPos.y; y++)
                    {
                        if (dy <= -15)
                        {
                            if (y < curPos.y - 10 && y > nextPos.y + 10)
                            {
                                continue;
                            }
                        }
                        float x = nextPos.x + (y - nextPos.y) * d;
                        uniqueTiles.Add(new Vector2Int(Mathf.RoundToInt(x), y));
                        Debug.Log($"Tile: ({x}, {y})");
                    }
                }
            }
        }

        var tilesToAdd = new List<Vector2Int>();

        foreach (var tile in uniqueTiles)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    // タイルの周囲のタイルも追加
                    tilesToAdd.Add(new Vector2Int(tile.x + x, tile.y + y));
                }
            }
        }

        uniqueTiles.UnionWith(tilesToAdd);

        List<Vector2Int> tileList = new List<Vector2Int>(uniqueTiles);
        Debug.Log($"<color=red>Total unique tiles: {tileList.Count}</color>");
        return tileList;
    }

    public void SetPlaceObjects(PlaceData placeData)
    {
        if (placeData == null)
        {
            Debug.LogWarning("PlaceData is null. Cannot set place objects.");
            return;
        }
        else
        {
            Debug.Log($"Setting place LatLng: {placeData.location.latitude}, {placeData.location.longitude}");
        }
        Vector3 position = mapLoader.LatLonToUnityLocalPosition(placeData.location.latitude, placeData.location.longitude);
        PlaceObject placeObj = Instantiate(placeObject, mapLoader.mapContainer.transform);
        placeObj.transform.localPosition = position;
        placeObj.SetText(placeData.displayName.text);
        placeObjects.Add(placeObj);
    }

    // public void DrawLogPath(MapBounds bounds, List<Vector2> points)
    // {
    //     Debug.Log("Drawing log path...");

    //     ClearLogPath();

    //     List<Vector2> canvasPositions = new List<Vector2>();
    //     foreach (var point in points)
    //     {
    //         Vector2 canvasPos = LatLonToMapPosition(point, bounds);
    //         Debug.Log($"canvasPosition: {canvasPos}");
    //         canvasPositions.Add(canvasPos);
    //     }

    //     // CanvasLineRendererを使用してラインを描画
    //     canvasLineRenderer.WriteLine(canvasPositions, mapRect.transform);

    //     // ポイントオブジェクトを生成
    //     foreach (var pos in canvasPositions)
    //     {
    //         GameObject pointObj = Instantiate(pointObject, mapRect.anchoredPosition, Quaternion.identity, mapRect.transform);
    //         pointObj.GetComponent<RectTransform>().anchoredPosition = pos;
    //         pointObjects.Add(pointObj);
    //     }
    // }

    // // 地図の境界を計算
    // public MapBounds CalculateBounds(List<Vector2> points)
    // {
    //     if (points.Count == 0)
    //     {
    //         return new MapBounds(0, 0, 0, 0);
    //     }

    //     float minLat = float.MaxValue;
    //     float minLon = float.MaxValue;
    //     float maxLat = float.MinValue;
    //     float maxLon = float.MinValue;

    //     foreach (var point in points)
    //     {
    //         Debug.Log($"Point: {point}");
    //         float lat = point.y;
    //         float lon = point.x;

    //         if (lat < minLat) minLat = lat;
    //         if (lon < minLon) minLon = lon;
    //         if (lat > maxLat) maxLat = lat;
    //         if (lon > maxLon) maxLon = lon;
    //     }

    //     Debug.Log($"Updated Bounds: {minLat}, {minLon} - {maxLat}, {maxLon}");

    //     return new MapBounds(minLat, minLon, maxLat, maxLon);
    // }

    // // 地図の中心を計算
    // public LatLng CalculateCenter(List<Vector2> points)
    // {
    //     LatLng center = new LatLng();
    //     double totalLat = 0;
    //     double totalLon = 0;
    //     foreach (var point in points)
    //     {
    //         totalLat += point.y;
    //         totalLon += point.x;
    //     }
    //     center.latitude = totalLat / points.Count;
    //     center.longitude = totalLon / points.Count;
    //     return center;
    // }

    // public int CalculateZoom(MapBounds bounds)
    // {
    //     // 簡易的なズーム計算
    //     float latRange = bounds.MaxLat - bounds.MinLat;
    //     float lonRange = bounds.MaxLon - bounds.MinLon;
    //     double maxDiff = Mathf.Max(latRange, lonRange);

    //         int zoom = 1;

    //     for (int z = 21; z >= 0; z--)
    //     {
    //         // 1ピクセルあたりの角度（概算）
    //         double degreesPerPixel = 360.0 / (256 * Mathf.Pow(2, z));
    //         if (maxDiff / degreesPerPixel <= 512 * 0.8)  // 余白を持たせる
    //         {
    //             zoom = z;
    //             break;
    //         }
    //     }
    //     return zoom;
    // }

    // private Vector2 LatLonToMapPosition(Vector2 point, MapBounds bounds)
    // {
    //     float x = Mathf.InverseLerp(bounds.MinLon, bounds.MaxLon, point.x) - 0.5f;
    //     float y = Mathf.InverseLerp(bounds.MinLat, bounds.MaxLat, point.y) - 0.5f;

    //     return new Vector2(x * mapRect.rect.width / 2, y * mapRect.rect.height / 2);
    // }
}