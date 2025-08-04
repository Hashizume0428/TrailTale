using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LocationLibrary;
using AILibrary;
using Cysharp.Threading.Tasks;
using StoryLibrary;
using EventLibrary;

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
    private GameObject storyUI;

    [SerializeField]
    private GameObject mapUI;

    [SerializeField]
    private Loading loadingUI;

    [SerializeField]
    private Button startStoryButton;

    [SerializeField]
    private ConfirmPanel confirmPanel;

    [SerializeField]
    private ResultPanel resultPanel;

    [SerializeField]
    private StatusDataBase statusDataBase;

    [SerializeField]
    private ItemDataBase itemDataBase;

    public string bookName = "MyStoryBook";

    private List<LatLng> latLngList;

    private List<int> storyIndexList;

    private int currentStoryIndex = 0;

    private PlaceData currentPlaceData;

    private EventData currentEventData;

    private EventSet eventSet;

    private async void Start()
    {
        mapUI.SetActive(true);
        storyUI.SetActive(false);

        eventSet = GetComponent<EventSet>();

        // ローディングUIを表示
        loadingUI.ShowLoading();

        // ログの読み取り

        string log;

        // ログファイルから読み込む
        LocationLogReader locationLogReader = new LocationLogReader();

        if (playerData.GetCurrentLog() != "")
        {
            log = playerData.GetCurrentLog();
            currentStoryIndex = playerData.GetCurrentStoryIndex();
        }
        else
        {
            log = locationLogReader.Read();
            playerData.SetCurrentLog(log);
        }

        if (log == null)
        {
            Debug.LogError("ログが読み込めませんでした。");
            return;
        }
        latLngList = locationLogReader.ParseLogToLatLng(log);

        // ログの地点から最大10個のインデックスを選択
        storyIndexList = locationLogReader.SelectClampIndex(latLngList, 1, 10);

        logPathRenderer.DrawLogPath(latLngList, storyIndexList);

        // マップの初期化を待つ
        await mapLoader.Init(latLngList[0]);

        // 最初の地点のランドマークを取得
        currentPlaceData = await GetPlaceData(latLngList[storyIndexList[currentStoryIndex]]);
        logPathRenderer.SetPlaceObjects(currentPlaceData); // 地図上にランドマークを表示

        mapCameraController.currentIndex = storyIndexList[currentStoryIndex];
        await mapCameraController.MoveCameraToPosition(logPathRenderer.pointObjects[storyIndexList[currentStoryIndex]].transform.position, 0.1f);

        // ローディングUIを非表示
        loadingUI.HideLoading();

        // 確認画面のセットアップ
        confirmPanel.Setup(() =>
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.Click);
            Debug.Log("HOME画面に戻ります。");
            SceneLoader.Instance.LoadMainScene("home");
        });


        displayResponse.RegisterNextButtonListener(OnClickNext);
    }

    private async UniTask<PlaceData> GetPlaceData(LatLng latLng)
    {
        Debug.Log("近くのランドマークを検索しています...");
        PlaceData placeData = await nearByLocation.SearchNearByLocation(latLng);
        Debug.Log("ランドマークの検索が完了しました。AIにプロンプトを送信します...");
        return placeData;
    }

    public void StartStory()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);
        storyUI.SetActive(true);
        mapUI.SetActive(false);
        resultPanel.Hide();

        currentEventData = eventSet.GetEventData(currentStoryIndex, storyIndexList.Count - 1);

        // バトルイベントの場合
        if (currentEventData.GetEventType() == EventLibrary.EventType.Battle)
        {
            Debug.Log("バトルイベントが発生しました。");
            BattleEventData battleEventData = (BattleEventData)currentEventData;
            StartBattleEvent(battleEventData);
            return;
        }

        // それ以外ならストーリー開始
        if (latLngList.Count > 0)
        {
            MakeStory(latLngList[currentStoryIndex], currentEventData);
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
        await UniTask.Delay(100);
        ResponseContent responseContent = await aiManager.SendPrompt(currentPlaceData.displayName.text, eventData);
        Debug.Log("AIからの応答を受信しました。");
        displayResponse.DisplayResponseContent(responseContent);
        loadingUI.HideLoading();
    }

    private void StartBattleEvent(BattleEventData battleEventData)
    {
        playerData.battleType = (int)battleEventData.battleType;
        // バトルイベントの処理をここに実装
        Debug.Log("バトルイベントの種類: " + battleEventData.battleType);

        currentStoryIndex++;
        
        // TODO : Scene遷移処理
        SceneLoader.Instance.LoadMainScene("BattleScene");

        
    }

    // ストーリーの次へボタンが押されたとき
    public async void OnClickNext()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);
        startStoryButton.interactable = false;
        // 選択されたオプションに基づいてステータスを更新
        if (currentEventData.GetEventType() == EventLibrary.EventType.Status)
        {
            Debug.Log("ステータスがアップデートされます");
            StatusEventData statusEventData = (StatusEventData)currentEventData;
            playerData.UpdateStatus(statusEventData.GetOption(displayResponse.SelectedOptionIndex).statusType,
                                    (int)statusEventData.GetOption(displayResponse.SelectedOptionIndex).statusChange);

            resultPanel.Setup(
                statusDataBase.GetStatusByType(statusEventData.GetOption(displayResponse.SelectedOptionIndex).statusType).icon,
                (int)statusEventData.GetOption(displayResponse.SelectedOptionIndex).statusChange,
                "あなたは\n" + statusDataBase.GetStatusByType(statusEventData.GetOption(displayResponse.SelectedOptionIndex).statusType).name + "\nを得ました。",
                OnClickConfirmResult
            );
        }
        else if (currentEventData.GetEventType() == EventLibrary.EventType.Item)
        {
            Debug.Log("アイテムイベントが発生しました。");
            ItemEventData itemEventData = (ItemEventData)currentEventData;
            playerData.UpdateInventory(itemEventData.GetOption(displayResponse.SelectedOptionIndex).itemType, 1);

            resultPanel.Setup(
                itemDataBase.GetItemByType(itemEventData.GetOption(displayResponse.SelectedOptionIndex).itemType).icon,
                1,
                "あなたは\n" + itemDataBase.GetItemByType(itemEventData.GetOption(displayResponse.SelectedOptionIndex).itemType).itemName + "\nを手に入れました。",
                OnClickConfirmResult
            );
        }

        // リザルトパネルを表示
        resultPanel.Show();

        Story story = StoryParser.ParseFromResponseContent(displayResponse.ResponseContent, displayResponse.SelectedOptionIndex);
        StoryBookManager.SavePage(bookName, story);
    }

    // リザルトの確認ボタンを押したとき
    public async void OnClickConfirmResult()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);
        currentStoryIndex++;
        playerData.SetCurrentStoryIndex(currentStoryIndex);
        if (currentStoryIndex < latLngList.Count)
        {
            mapUI.SetActive(true);
            storyUI.SetActive(false);
            await UniTask.Delay(100); // 少し待機してから次のストーリーを開始

            // 次の地点のランドマークを取得
            currentPlaceData = await GetPlaceData(latLngList[storyIndexList[currentStoryIndex]]);
            logPathRenderer.SetPlaceObjects(currentPlaceData); // 地図上にランドマークを表示
            await mapCameraController.NextIndex();
            startStoryButton.interactable = true;
        }
        else
        {
            Debug.Log("すべてのストーリーが完了しました。");

            OnEndStory();
        }
    }

    // ストーリーを終了する
    public void OnEndStory()
    {
        playerData.SetCurrentStoryIndex(0);
        playerData.SetCurrentLog("");
    }
}