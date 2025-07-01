using System;

namespace LocationLibrary
{
    public static class Constants
    {
        public const float MAP_SIZE = 5.5f;
    }

    [Serializable]
    public struct LocationData
    {
        public float Latitude;
        public float Longitude;
    }

    public struct MapBounds
    {
        public float MinLat;
        public float MinLon;
        public float MaxLat;
        public float MaxLon;

        public MapBounds(float minLat, float minLon, float maxLat, float maxLon)
        {
            MinLat = float.MaxValue;
            MinLon = float.MaxValue;
            MaxLat = float.MinValue;
            MaxLon = float.MinValue;
        }
    }

    /// <summary>
    /// NearBySearchを行うときに、このクラスをJSONに変換して使う
    /// </summary>
    [Serializable]
    public class RequestData
    {
        public int maxResultCount;
        public string rankPreference;
        public LocationRestriction locationRestriction;
    }

    [Serializable]
    public class LocationRestriction
    {
        public Circle circle;
    }

    [Serializable]
    public class Circle
    {
        public LatLng center;
        public float radius;
    }

    [Serializable]
    public class LatLng
    {
        public double latitude;
        public double longitude;
    }
}