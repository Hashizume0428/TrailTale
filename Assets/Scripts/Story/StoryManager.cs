using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LocationLibrary;
using AILibrary;
using Cysharp.Threading.Tasks;
using StoryLibrary;

public class StoryManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private AIManager aiManager;

    [SerializeField]
    private MapLoader mapLoader;

    [SerializeField]
    private LogPathRenderer logPathRenderer;

    [SerializeField]
    private MapCameraController mapCameraController;

    [SerializeField]
    private NearByLocation nearByLocation;

    [SerializeField]
    private DisplayResponse displayResponse;

    [SerializeField]
    private List<LatLng> latLngList;

    [SerializeField]
    private EventData mockEventData;

    [SerializeField]
    private GameObject storyUI;

    [SerializeField]
    private GameObject mapUI;

    [SerializeField]
    private Loading loadingUI;

    public string bookName = "MyStoryBook";

    private int currentStoryIndex = 0;

    private EventData currentEventData;

    private async void Start()
    {
        mapUI.SetActive(true);
        storyUI.SetActive(false);

        // ローディングUIを表示
        loadingUI.ShowLoading();

        // ログの読み取り
        LocationLogReader locationLogReader = new LocationLogReader();
        string log = locationLogReader.Read();
        if (log == null)
        {
            Debug.LogError("ログが読み込めませんでした。");
            return;
        }
        latLngList = locationLogReader.ParseLogToLatLng(log);

        logPathRenderer.DrawLogPath(latLngList);
        // マップの初期化を待つ
        await mapLoader.Init(latLngList[0]);

        // ローディングUIを非表示
        loadingUI.HideLoading();


        displayResponse.RegisterNextButtonListener(Next);
    }

    public void StartStory()
    {
        storyUI.SetActive(true);
        mapUI.SetActive(false);

        // ストーリーの開始
        if (latLngList.Count > 0)
        {
            MakeStory(latLngList[currentStoryIndex], mockEventData);
        }
        else
        {
            Debug.LogError("ストーリーの開始に必要なログポイントがありません。");
        }
    }

    private async void MakeStory(LatLng latLng, EventData eventData)
    {
        currentEventData = eventData;

        loadingUI.ShowLoading();
        Debug.Log("近くのランドマークを検索しています...");
        string locationName = await nearByLocation.SearchNearByLocation(latLng);
        Debug.Log("ランドマークの検索が完了しました。AIにプロンプトを送信します...");
        await UniTask.Delay(100);
        ResponseContent responseContent = await aiManager.SendPrompt(locationName, eventData);
        Debug.Log("AIからの応答を受信しました。");
        displayResponse.DisplayResponseContent(responseContent);
        loadingUI.HideLoading();
    }

    public async void Next()
    {
        // 選択されたオプションに基づいてステータスを更新
        if (currentEventData.GetEventType() == EventData.EventType.Status)
        {
            Debug.Log("ステータスがアップデートされます");
            StatusEventData statusEventData = (StatusEventData)currentEventData;
            playerData.UpdateStatus(statusEventData.GetOption(displayResponse.SelectedOptionIndex).statusType,
                                    (int)statusEventData.GetOption(displayResponse.SelectedOptionIndex).statusChange);
        }
        else if (currentEventData.GetEventType() == EventData.EventType.Item)
        {

        }

        Story story = StoryParser.ParseFromResponseContent(displayResponse.ResponseContent, displayResponse.SelectedOptionIndex);
        StoryBookManager.SavePage(bookName, story);

        currentStoryIndex++;
        if (currentStoryIndex < latLngList.Count)
        {
            mapUI.SetActive(true);
            storyUI.SetActive(false);
            await UniTask.Delay(1000); // 少し待機してから次のストーリーを開始
            mapCameraController.NextIndex();
        }
        else
        {
            Debug.Log("すべてのストーリーが完了しました。");
        }
    }
}