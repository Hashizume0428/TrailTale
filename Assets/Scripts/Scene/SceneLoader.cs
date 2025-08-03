using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private string _loadingSceneName = "LoadingScene"; // ロード中に表示するシーン名

    // シーンロード完了時に発火するイベント
    public event Action<string> OnSceneLoaded;
    // シーンアンロード完了時に発火するイベント
    public event Action<string> OnSceneUnloaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // シーンロード/アンロードイベントを購読
            SceneManager.sceneLoaded += OnSceneLoadedInternal;
            SceneManager.sceneUnloaded += OnSceneUnloadedInternal;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時にイベント購読を解除
        SceneManager.sceneLoaded -= OnSceneLoadedInternal;
        SceneManager.sceneUnloaded -= OnSceneUnloadedInternal;
    }

    // 内部的なシーンロードイベントハンドラ
    private void OnSceneLoadedInternal(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}, Mode: {mode}");
        OnSceneLoaded?.Invoke(scene.name); // 外部イベントを発火
    }

    // 内部的なシーンアンロードイベントハンドラ
    private void OnSceneUnloadedInternal(Scene scene)
    {
        Debug.Log($"Scene Unloaded: {scene.name}");
        OnSceneUnloaded?.Invoke(scene.name); // 外部イベントを発火
    }

    /// <summary>
    /// メインシーン（単一ロード）に遷移します。
    /// ロードシーンを介するかどうかは設定によります。
    /// </summary>
    /// <param name="sceneName">遷移先のシーン名</param>
    public void LoadMainScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(_loadingSceneName) && SceneManager.GetActiveScene().name != _loadingSceneName)
        {
            StartCoroutine(LoadSceneAsyncWithLoadingScreen(sceneName, LoadSceneMode.Single));
        }
        else
        {
            // 直接シーンをロード（同期ロード）
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// 既存のシーンに新しいシーンを加算ロードします。
    /// </summary>
    /// <param name="sceneName">加算ロードするシーン名</param>
    /// <returns>非同期操作</returns>
    public AsyncOperation LoadAdditiveSceneAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("LoadAdditiveSceneAsync: Scene name cannot be null or empty.");
            return null;
        }
        Debug.Log($"Loading scene additively: {sceneName}");
        // 加算ロードは通常、ロード画面を挟まずに直接実行することが多い
        // 必要であれば、ここでもロード進捗表示のUIなどを検討
        return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 指定されたシーンをアンロードします。
    /// </summary>
    /// <param name="sceneName">アンロードするシーン名</param>
    /// <returns>非同期操作</returns>
    public AsyncOperation UnloadSceneAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("UnloadSceneAsync: Scene name cannot be null or empty.");
            return null;
        }
        Debug.Log($"Unloading scene: {sceneName}");
        return SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// ロード画面を挟んで非同期でシーンをロードします。
    /// </summary>
    /// <param name="sceneName">遷移先のシーン名</param>
    /// <param name="mode">ロードモード (Single or Additive)</param>
    private IEnumerator LoadSceneAsyncWithLoadingScreen(string sceneName, LoadSceneMode mode)
    {
        // まずロードシーンを同期ロード
        yield return SceneManager.LoadSceneAsync(_loadingSceneName, LoadSceneMode.Single);

        // ロードシーンがロードされた後に、実際のシーンを非同期ロード
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
        asyncOperation.allowSceneActivation = false; // ロード完了後も自動でシーンを切り替えない

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            Debug.Log($"Loading progress: {progress * 100}%");

            if (asyncOperation.progress >= 0.9f)
            {
                // ここでロード画面の表示やアニメーションが完了するのを待つなど
                // 例: yield return new WaitForSeconds(1f); 
                
                asyncOperation.allowSceneActivation = true; // シーンのアクティベートを許可
            }
            yield return null;
        }
    }
}