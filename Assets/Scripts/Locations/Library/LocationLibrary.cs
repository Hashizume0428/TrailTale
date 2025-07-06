namespace LocationLibrary
{
    using System;

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
}