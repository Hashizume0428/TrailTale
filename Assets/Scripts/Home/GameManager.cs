using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // コルーチンを使用するために必要です

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject configPanel;

    // シーン遷移までのラグ時間（秒）
    [Header("Scene Transition Settings")]
    [SerializeField] private float sceneTransitionDelay = 0.8f;

    // --- シングルトンパターン (もしGameManagerもDontDestroyOnLoadなら) ---
    // もしこのGameManagerがDontDestroyOnLoadで永続化されているなら、
    // 以下のシングルトン実装をAwakeに追加してください。
    // private static GameManager instance = null;
    // public static GameManager Instance { get { return instance; } }
    // void Awake() {
    //     if (instance != null && instance != this) { Destroy(this.gameObject); return; }
    //     instance = this;
    //     DontDestroyOnLoad(this.gameObject);
    // }

    // --- 既存のメソッド（修正） ---

    public void StartButton() // メソッド名をStartBottonからStartButtonに修正しました（推奨）
    {
        Debug.Log("Start Button clicked.");
        StartCoroutine(LoadSceneWithDelay("home")); // コルーチンを開始
    }

    public void ShowConfigPanel() // メソッド名をShowconfigPanelからShowConfigPanelに修正しました（推奨）
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

    public void HideConfigPanel() // メソッド名をHideConfigPanelからHideConfigPanelに修正しました（推奨）
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

    public void ItemPageButton() // メソッド名をItemPageBottonからItemPageButtonに修正しました（推奨）
    {
        Debug.Log("ItemPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Item")); // コルーチンを開始
    }

    public void StatusPageButton() // メソッド名をStatusPageBottonからStatusPageButtonに修正しました（推奨）
    {
        Debug.Log("StatusPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Status")); // コルーチンを開始
    }

    public void ScenarioPageButton() // メソッド名をScenarioPageBottonからScenarioPageButtonに修正しました（推奨）
    {
        Debug.Log("ScenarioPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("ScenarioTest")); // コルーチンを開始
    }

    public void SettingButton() // メソッド名をSettingBottonからSettingButtonに修正しました（推奨）
    {
        Debug.Log("SettingButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Setting")); // コルーチンを開始
    }

    public void BacknumberButton() // メソッド名をBackbumerBottonからBacknumberButtonに修正しました（推奨）
    {
        Debug.Log("BacknumberButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Backnumber")); // コルーチンを開始
    }

    public void TitleButton() // メソッド名をTitleBottonからTitleButtonに修正しました（推奨）
    {
        Debug.Log("TitleButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Title")); // コルーチンを開始
    }

    public void HomeButton() // メソッド名をHomeBottonからHomeButtonに修正しました（推奨）
    {
        Debug.Log("HomeButton clicked.");
        StartCoroutine(LoadSceneWithDelay("home")); // コルーチンを開始
    }

    // --- 新規追加メソッド ---

    /// <summary>
    /// 指定された時間待機した後、シーンをロードするコルーチン
    /// </summary>
    /// <param name="sceneName">ロードするシーンの名前</param>
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // ここで、クリックされたボタンを無効化するなどのUIフィードバックを入れると良いでしょう
        // 例: EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;

        Debug.Log($"Waiting for {sceneTransitionDelay} seconds before loading scene: {sceneName}");
        yield return new WaitForSeconds(sceneTransitionDelay); // 指定された秒数待機

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName); // シーンをロード
    }
}