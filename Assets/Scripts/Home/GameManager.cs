using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // コルーチンを使用するために必要です
using System.Collections.Generic; // List<Button> を使用するために必要

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject configPanel;

    // シーン遷移までのラグ時間（秒）
    [Header("Scene Transition Settings")]
    [SerializeField] private float sceneTransitionDelay = 0.8f;

    // ★★★ここからGameManagerのシングルトンパターンを削除★★★
    // private static GameManager instance = null;
    // public static GameManager Instance { get { return instance; } }
    //
    // void Awake()
    // {
    //     if (instance != null && instance != this)
    //     {
    //         Destroy(this.gameObject);
    //         return;
    //     }
    //     instance = this;
    //     DontDestroyOnLoad(this.gameObject);
    // }
    // ★★★ここまでGameManagerのシングルトンパターンを削除★★★


    // UIボタンのリスナーを保持するリスト (このロジックはGameManagerをシングルトン化しない場合は通常不要ですが、
    // 以前のOnSceneLoadedForSEメソッドの残骸なので、もし使わないなら削除できます。)
    private List<Button> registeredButtons = new List<Button>();


    // --- 既存のメソッド（変更なし） ---

    public void StartButton()
    {
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
        StartCoroutine(LoadSceneWithDelay("ScenarioTest")); // コルーチンを開始
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

    // --- 新規追加メソッド ---

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

    // ★★★ここから、以前の「GameManagerで自動的にボタンにリスナーを登録する」ロジック。
    // GameManagerをシングルトン化しない場合、このOnEnable/OnDisable/OnDestroyおよび関連メソッドは通常不要です。
    // もしGameManagerがシーンごとに配置されるなら、これらのメソッドは削除しても構いません。
    // (ただし、その場合はGameManagerのGameObjectを各シーンに配置し、
    //  configPanelなどの[SerializeField]をInspectorで各シーンで割り当て直す必要があります)
    /*
    void OnEnable()
    {
        // GameManagerがシングルトンでない場合、このイベント購読は推奨されません。
        // シーンごとにGameManagerがある場合、各GameManagerがシーンロードイベントを購読してしまいます。
        SceneManager.sceneLoaded += OnSceneLoadedForSE;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedForSE;
        RemoveAllButtonListeners();
    }

    void OnDestroy()
    {
        RemoveAllButtonListeners();
    }

    private void OnSceneLoadedForSE(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene Loaded: {scene.name} for SE registration.");
        RemoveAllButtonListeners();

        Button[] allButtonsInScene = Resources.FindObjectsOfTypeAll<Button>();
        
        foreach (Button button in allButtonsInScene)
        {
            if (button.gameObject.scene != scene) continue; 
            
            button.onClick.AddListener(PlayClickSFXForButton);
            registeredButtons.Add(button); 
        }
        Debug.Log($"[GameManager] {registeredButtons.Count} 個のボタンにSEリスナーを登録しました。");
    }

    private void PlayClickSFXForButton()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClickSFX();
        }
        else
        {
            Debug.LogWarning("[GameManager] SoundManagerのインスタンスが見つからないため、ボタンクリックSEを再生できません。");
        }
    }

    private void RemoveAllButtonListeners()
    {
        foreach (Button button in registeredButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveListener(PlayClickSFXForButton);
            }
        }
        registeredButtons.Clear();
        Debug.Log("[GameManager] 全てのボタンからSEリスナーを解除しました。");
    }
    */
}