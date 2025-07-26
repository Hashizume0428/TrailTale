using UnityEngine;

// --- 個々の地図タイルを管理するためのクラス ---
public class MapTile
{
    public GameObject GameObject { get; private set; }
    public int Z { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    public MapTile(GameObject go, int z, int x, int y)
    {
        GameObject = go;
        Z = z;
        X = x;
        Y = y;
    }

    public string GetTileKey()
    {
        return $"{Z}/{X}/{Y}";
    }

    public void DestroyTile()
    {
        if (GameObject != null)
        {
            GameObject.transform.SetParent(null);
            GameObject.Destroy(GameObject);
            GameObject = null;
        }
    }
}