namespace LocationLibrary
{
    using System;

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