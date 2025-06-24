using UnityEngine;

public class LocationBridge
{
    public static void StartLocation()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                if (currentActivity == null)
                {
                    Debug.LogError("CurrentActivity is null!");
                    return;
                }

                using (AndroidJavaClass locationClass = new AndroidJavaClass("com.example.mylibrary.LocationController"))
                {
                    if (locationClass == null)
                    {
                        Debug.LogError("LocationController class not found!");
                        return;
                    }

                    locationClass.CallStatic("startLocationService", currentActivity);
                }
            }

        }
#endif
    }

    public static void StopLocation()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaClass locationClass = new AndroidJavaClass("com.example.mylibrary.LocationController"))
            {
                locationClass.CallStatic("stopLocationService", context);
            }
        }
#endif
    }
}
