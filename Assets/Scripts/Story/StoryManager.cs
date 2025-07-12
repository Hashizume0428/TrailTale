using UnityEngine;
using System.Collections;
using LocationLibrary;
using AILibrary;
using Cysharp.Threading.Tasks;
using StoryLibrary;

public class StoryManager : MonoBehaviour
{
    [SerializeField]
    private AIManager aiManager;

    [SerializeField]
    private NearByLocation nearByLocation;

    [SerializeField]
    private DisplayResponse displayResponse;

    [SerializeField]
    private LatLng[] _latLng;

    [SerializeField]
    private EventData _eventData;

    public string bookName = "MyStoryBook";

    private int currentStoryIndex = 0;

    private void Start()
    {
        displayResponse.RegisterNextButtonListener(NextStory);
        MakeStory(_latLng[currentStoryIndex], _eventData);
    }

    private async void MakeStory(LatLng latLng, EventData eventData)
    {
        displayResponse.ShowLoading();
        Debug.Log("近くのランドマークを検索しています...");
        string locationName = await nearByLocation.SearchNearByLocation(latLng);
        Debug.Log("ランドマークの検索が完了しました。AIにプロンプトを送信します...");
        await UniTask.Delay(100);
        ResponseContent responseContent = await aiManager.SendPrompt(locationName, eventData);
        Debug.Log("AIからの応答を受信しました。");
        displayResponse.DisplayResponseContent(responseContent);
        displayResponse.HideLoading();
    }

    public void NextStory()
    {
        Story story = StoryParser.ParseFromResponseContent(displayResponse.ResponseContent, displayResponse.SelectedOptionIndex);
        StoryBookManager.SavePage(bookName, story);

        currentStoryIndex++;
        if (currentStoryIndex < _latLng.Length)
        {
            MakeStory(_latLng[currentStoryIndex], _eventData);
        }
        else
        {
            Debug.Log("すべてのストーリーが完了しました。");
        }
    }
}