using UnityEngine;
using System.Collections;
using LocationLibrary;
using AILibrary;
using Cysharp.Threading.Tasks;

public class StoryManager : MonoBehaviour
{
    [SerializeField]
    private AIManager aiManager;

    [SerializeField]
    private NearByLocation nearByLocation;

    [SerializeField]
    private DisplayResponse displayResponse;

    [SerializeField]
    private LatLng _latLng = new LatLng
    {
        latitude = 34.6851f,
        longitude = 135.8048f
    };

    [SerializeField]
    private EventData _eventData;

    private void Start()
    {
        MakeStory(_latLng, _eventData);
    }

    private async void MakeStory(LatLng latLng, EventData eventData)
    {
        Debug.Log("近くのランドマークを検索しています...");
        string locationName = await nearByLocation.SearchNearByLocation(latLng);
        Debug.Log("ランドマークの検索が完了しました。AIにプロンプトを送信します...");
        await UniTask.Delay(100);
        ResponseContent responseContent = await aiManager.SendPrompt(locationName, eventData);
        Debug.Log("AIからの応答を受信しました。");
        displayResponse.DisplayResponseContent(responseContent);
    }
}