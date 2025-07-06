using UnityEngine;
using System.Collections;
using LocationLibrary;
using Cysharp.Threading.Tasks;

public class StoryManager : MonoBehaviour
{
    [SerializeField]
    private AIManager aiManager;

    [SerializeField]
    private NearByLocation nearByLocation;

    [SerializeField]
    private LatLng latLng = new LatLng
    {
        latitude = 34.6851f,
        longitude = 135.8048f
    };

    private async void Start()
    {
        Debug.Log("近くのランドマークを検索しています...");
        string locationName = await nearByLocation.SearchNearByLocation(latLng);
        Debug.Log("ランドマークの検索が完了しました。AIにプロンプトを送信します...");
        await UniTask.Delay(100);
        await aiManager.SendPromptCoroutine(locationName);
        Debug.Log("AIからの応答を受信しました。");
    }
}