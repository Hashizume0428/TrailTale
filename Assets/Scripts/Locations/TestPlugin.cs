using UnityEngine;
using UnityEngine.Android;

public class TestPlugin : MonoBehaviour
{
    void Start()
    {
        #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        #endif
    }

    public void StartLocationService()
    {
        //LocationBridge plugin = new LocationBridge();
        LocationBridge.StartLocation();
    }

    public void StopLocationService()
    {
        //LocationBridge plugin = new LocationBridge();
        LocationBridge.StopLocation();
    }
}