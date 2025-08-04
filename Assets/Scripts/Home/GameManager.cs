using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Button を使用するために必要です
using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshProUGUI を使用するために必要です

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject configPanel;

    [SerializeField] PlayerData playerData; // PlayerDataの参照をInspectorから設定

    // シーン遷移までのラグ時間（秒）
    [Header("Scene Transition Settings")]
    [SerializeField] private float sceneTransitionDelay = 0.8f;

    // ★★★GameManagerのシングルトンパターンは削除されたままです★★★
    // private static GameManager instance = null;
    // public static GameManager Instance { get { return instance; } }
    //
    // void Awake()
    // {
    //      if (instance != null && instance != this)
    //      {
    //          Destroy(this.gameObject);
    //          return;
    //      }
    //      instance = this;
    //      DontDestroyOnLoad(this.gameObject);
    // }
    // ★★★GameManagerのシングルトンパターンは削除されたままです★★★


    // UIボタンのリスナーを保持するリスト (このロジックはGameManagerをシングルトン化しない場合は通常不要ですが、
    // 以前のOnSceneLoadedForSEメソッドの残骸なので、もし使わないなら削除できます。)
    private List<Button> registeredButtons = new List<Button>();

    // --- 新規追加部分 ---
    [Header("Route Info Text Toggle")]
    [SerializeField] private TextMeshProUGUI routeInfoDisplayTextBox; // 経路情報を表示するUIテキストボックス（TextMeshProUGUI型）
    private bool isRouteInfoAcquiring = false; // 経路情報が取得中かどうかの状態

    // PlayerPrefsで使用するキー
    private const string ROUTE_INFO_STATUS_KEY = "IsRouteInfoAcquiring";

    void Start()
    {
        // アプリケーション起動時やシーンロード時にPlayerPrefsから状態を読み込む
        // PlayerPrefs.GetIntは、キーが存在しない場合に指定したデフォルト値（ここでは0）を返します。
        // 0をfalse、1をtrueとして扱います。
        isRouteInfoAcquiring = (PlayerPrefs.GetInt(ROUTE_INFO_STATUS_KEY, 0) == 1);

        // UIテキストボックスが設定されていれば、初期表示を更新
        if (routeInfoDisplayTextBox != null)
        {
            UpdateRouteInfoDisplayText();
        }
        else
        {
            Debug.LogWarning("[GameManager] Start: routeInfoDisplayTextBoxが設定されていません。Inspectorで設定してください。");
        }
    }

    // --- 既存のメソッド（変更なし） ---

    public void StartButton()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Page);
        Debug.Log("Start Button clicked.");
        StartCoroutine(LoadSceneWithDelay("home")); // コルーチンを開始
    }

    public void ShowConfigPanel()
    {
        if (configPanel != null)
        {
            configPanel.SetActive(true);
            Debug.Log("Config Panel shown.");
        }
        else
        {
            Debug.LogWarning("Config Panel is not assigned in GameManager.");
        }
    }

    public void HideConfigPanel()
    {
        if (configPanel != null)
        {
            configPanel.SetActive(false);
            Debug.Log("Config Panel hidden.");
        }
        else
        {
            Debug.LogWarning("Config Panel is not assigned in GameManager.");
        }
    }

    public void ItemPageButton()
    {
        Debug.Log("ItemPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Item")); // コルーチンを開始
    }

    public void StatusPageButton()
    {
        Debug.Log("StatusPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Status")); // コルーチンを開始
    }

    public void ScenarioPageButton()
    {
        Debug.Log("ScenarioPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Story")); // コルーチンを開始
    }

    public void SettingButton()
    {
        Debug.Log("SettingButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Setting")); // コルーチンを開始
    }

    public void BacknumberButton()
    {
        Debug.Log("BacknumberButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Backnumber")); // コルーチンを開始
    }

    public void TitleButton()
    {
        Debug.Log("TitleButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Title")); // コルーチンを開始
    }

    public void HomeButton()
    {
        Debug.Log("HomeButton clicked.");
        StartCoroutine(LoadSceneWithDelay("home")); // コルーチンを開始
    }

    public void ResetButton()
    {
        playerData.Delete(); // PlayerDataを削除
        TitleButton();
    }

    /// <summary>
    /// 指定された時間待機した後、シーンをロードするコルーチン
    /// </summary>
    /// <param name="sceneName">ロードするシーンの名前</param>
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        Debug.Log($"Waiting for {sceneTransitionDelay} seconds before loading scene: {sceneName}");
        yield return new WaitForSeconds(sceneTransitionDelay); // 指定された秒数待機

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName); // シーンをロード
    }

    // UIボタンのOnClickイベントから直接呼び出すSE再生メソッド
    /// <summary>
    /// UIボタンのOnClickイベントから直接呼び出して、クリックSEを再生します。
    /// このメソッドは、既存のボタン機能に影響を与えません。
    /// </summary>
    public void PlayClickSFXOnButton()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClickSFX();
            Debug.Log("[GameManager] UIボタンクリックSEを再生しました。");
        }
        else
        {
            Debug.LogWarning("[GameManager] SoundManagerのインスタンスが見つからないため、クリックSEを再生できません。");
        }
    }

    /// <summary>
    /// UIボタンクリックで経路情報表示テキストを切り替えるメソッド。
    /// このメソッドは、UIボタンのOnClickイベントに設定します。
    /// </summary>
    public void ToggleRouteInfoText()
    {
        if (routeInfoDisplayTextBox == null)
        {
            Debug.LogWarning("[GameManager] ToggleRouteInfoText: routeInfoDisplayTextBoxが設定されていません。Inspectorで設定してください。");
            return;
        }

        isRouteInfoAcquiring = !isRouteInfoAcquiring; // 状態を反転させる

        // PlayerPrefsに状態を保存
        PlayerPrefs.SetInt(ROUTE_INFO_STATUS_KEY, isRouteInfoAcquiring ? 1 : 0);
        PlayerPrefs.Save(); // 変更を保存 (即座に保存する場合。通常はアプリケーション終了時に自動保存されますが、明示的に呼ぶことも可能です)

        UpdateRouteInfoDisplayText(); // 表示を更新
    }

    /// <summary>
    /// 現在のisRouteInfoAcquiringの状態に基づいてテキスト表示を更新するヘルパーメソッド
    /// </summary>
    private void UpdateRouteInfoDisplayText()
    {
        if (routeInfoDisplayTextBox == null) return; // 念のためnullチェック

        if (isRouteInfoAcquiring)
        {
            routeInfoDisplayTextBox.text = "【経路情報取得中】";
            Debug.Log("[GameManager] 経路情報表示: 取得中");
        }
        else
        {
            routeInfoDisplayTextBox.text = "【経路情報取得停止中】";
            Debug.Log("[GameManager] 経路情報表示: 取得停止中");
        }
    }
}

  