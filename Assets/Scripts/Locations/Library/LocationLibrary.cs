namespace LocationLibrary
{
    using System;

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
            MinLat = minLat;
            MinLon = minLon;
            MaxLat = maxLat;
            MaxLon = maxLon;
        }
    }
}