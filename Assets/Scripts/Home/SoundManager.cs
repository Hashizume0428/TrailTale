using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // コルーチンを使うために必要

public class SoundManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static SoundManager Instance { get; private set; }

    // AudioSourceの参照
    [SerializeField] private AudioSource bgmAudioSource; // BGM再生用
    [SerializeField] private AudioSource seAudioSource;  // SE再生用 (汎用SEやクリック音用)

    // AudioMixerの参照と公開パラメーターのパス
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_MIXER_PARAM = "BGMVolumeParam";
    private const string SE_MIXER_PARAM = "SEVolumeParam";

    // BGMとSEのサウンドデータリスト
    [SerializeField] private List<BGMSoundData> bgmSoundDatas;
    [SerializeField] private List<SESoundData> seSoundDatas;

    // UIスライダーの参照 (InitializeSlidersで設定する)
    private Slider bgmSlider;
    private Slider seSlider;

    // PlayerPrefsのキー
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

    // AudioClip clickSFXは不要なため削除済み


    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移時に破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // 既にインスタンスがあれば自身を破棄
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // PlayerPrefsから保存された音量を読み込み、AudioMixerに適用
        LoadVolumeSettings();

        // Start時に現在ロードされているシーンのBGMを再生する 
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        // SE AudioSourceの初期設定 (念のため)
        if (seAudioSource != null)
        {
            seAudioSource.loop = false;
            seAudioSource.playOnAwake = false;
        }

        // AudioMixerが設定されているか確認
        if (audioMixer == null)
        {
            Debug.LogError("SoundManager: AudioMixerが設定されていません。オーディオミキサーによる音量制御は行われません。");
        }
        if (bgmAudioSource != null && bgmAudioSource.outputAudioMixerGroup == null)
        {
            Debug.LogWarning("SoundManager: BGM AudioSourceのOutputがAudioMixerGroupに設定されていません。");
        }
        if (seAudioSource != null && seAudioSource.outputAudioMixerGroup == null)
        {
            Debug.LogWarning("SoundManager: SE AudioSourceのOutputがAudioMixerGroupに設定されていません。");
        }
    }

    private void OnDestroy()
    {
        // スクリプトが破棄されるときにリスナーを解除
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
        }
        if (seSlider != null)
        {
            seSlider.onValueChanged.RemoveListener(SetSEVolume);
        }
    }

    /// <summary>
    /// シーンがロードされたときに呼び出されます。
    /// BGMの切り替えと、そのシーンの「設定を開く」ボタンへのリスナー設定を行います。
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SoundManager] Scene Loaded: {scene.name} (Mode: {mode})");

        // 以前のスライダーのリスナーを解除し、参照をクリア
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            bgmSlider = null;
        }
        if (seSlider != null)
        {
            seSlider.onValueChanged.RemoveListener(SetSEVolume);
            seSlider = null;
        }

        // 新しいBGMを再生する前に、現在再生中のBGMを停止する
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            Debug.Log("[SoundManager] 現在再生中のBGMを停止しました。");
        }


        // シーン名に応じてBGMを再生
        switch (scene.name)
        {
            case "home":
            case "Backnumber":
            case "Item":
            case "Status":
            case "Title":
                Debug.Log($"[SoundManager] シーン '{scene.name}' のBGMをHomeに設定します。");
                PlayBGM(BGMSoundData.BGM.Home);
                break;
            case "BattleScene":
                Debug.Log($"[SoundManager] シーン '{scene.name}' のBGMをBattle01に設定します。");
                PlayBGM(BGMSoundData.BGM.Battle01);
                break;
            case "Story":
                Debug.Log($"[SoundManager] シーン '{scene.name}' のBGMをStoryに設定します。");
                PlayBGM(BGMSoundData.BGM.Story);
                break;
            default:
                Debug.Log($"[SoundManager] シーン '{scene.name}' に対応する特定のBGMは設定されていません。");
                break;
        }

        // ★★★ シーンロード時に設定ボタンを検索し、リスナーを設定する ★★★
        StartCoroutine(ListenForConfigButtonDelayed(scene));
    }

    /// <summary>
    /// スライダーの初期化を少し遅延させて実行するコルーチン。
    /// </summary>
    private IEnumerator InitializeSlidersDelayed()
    {
        // 現在のフレームの終わりまで待機。これにより、UI要素が完全にアクティブになる機会を与えます。
        yield return new WaitForEndOfFrame();

        Debug.Log("[SoundManager] Performing delayed slider search and initialization.");

        // 以前のスライダーのリスナーを解除し、参照をクリア (再初期化のため)
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            bgmSlider = null;
        }
        if (seSlider != null)
        {
            seSlider.onValueChanged.RemoveListener(SetSEVolume);
            seSlider = null;
        }

        // 現在ロードされている全てのスライダーを検索 (アクティブ/非アクティブ問わず)
        // Resources.FindObjectsOfTypeAllは重いため、OnSceneLoadedで毎回呼ぶのは注意。
        // ただし、この場合はUIボタンクリック時のみにInitializeSliders()が呼ばれるので問題ない。
        Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();

        foreach (Slider slider in allSliders)
        {
            // 現在アクティブなシーンに属するスライダーのみを対象とする
            if (slider.gameObject.scene != SceneManager.GetActiveScene()) continue;

            if (slider.CompareTag("BGM_Slider"))
            {
                bgmSlider = slider;
                Debug.Log($"[SoundManager] BGMスライダーをResources.FindObjectsOfTypeAllで検出: {slider.gameObject.name}");
            }
            else if (slider.CompareTag("SE_Slider"))
            {
                seSlider = slider;
                Debug.Log($"[SoundManager] SEスライダーをResources.FindObjectsOfTypeAllで検出: {slider.gameObject.name}");
            }

            // 両方のスライダーが見つかれば、これ以上探す必要はない
            if (bgmSlider != null && seSlider != null)
            {
                break;
            }
        }

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            bgmSlider.value = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.75f);
            Debug.Log($"[SoundManager] BGMスライダー初期値設定: {bgmSlider.value}");
        }
        else
        {
            Debug.LogWarning("[SoundManager] BGM_Sliderタグを持つスライダーがResources.FindObjectsOfTypeAllでも見つかりませんでした。");
        }

        if (seSlider != null)
        {
            seSlider.onValueChanged.AddListener(SetSEVolume);
            seSlider.value = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.75f);
            Debug.Log($"[SoundManager] SEスライダー初期値設定: {seSlider.value}");
        }
        else
        {
            // ... （中略）
        }
    }

    /// <summary>
    /// シーンロード時に、そのシーンの「設定を開く」ボタンを探し、クリックリスナーを登録するコルーチン。
    /// UIが完全にロードされてからボタンを見つけることを試みます。
    /// </summary>
    /// <param name="scene">現在のシーン</param>
    private IEnumerator ListenForConfigButtonDelayed(Scene scene)
    {
        yield return null; // 1フレーム待って、UI要素が完全にロードされるのを待つ

        Debug.Log($"[SoundManager] シーン '{scene.name}' のConfigButtonを検索中...");

        // そのシーン内の全てのButtonコンポーネントを検索 (アクティブ/非アクティブ問わず)
        Button[] allButtonsInScene = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button button in allButtonsInScene)
        {
            // 現在アクティブなシーンに属するボタンで、かつConfigButtonタグを持つものを探す
            if (button.gameObject.scene != scene) continue;

            if (button.CompareTag("ConfigButton")) // ConfigButtonは、設定パネルを開くボタンに割り当てるタグ
            {
                // 既存のリスナーを一度削除してから追加（二重登録防止）
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => StartCoroutine(InitializeSlidersDelayed())); // InitializeSlidersをコルーチンで遅延実行
                Debug.Log($"[SoundManager] シーン '{scene.name}' のConfigButton '{button.gameObject.name}' にリスナーを登録しました。");
                // 設定ボタンは複数ある可能性があるので、breakしない
            }
        }
    }

    /// <summary>
    /// BGMを再生します。
    /// </summary>
    /// <param name="bgm">再生するBGMの種類</param>
    public void PlayBGM(BGMSoundData.BGM bgm)
    {
        Debug.Log($"[SoundManager] PlayBGMメソッドが呼び出されました。BGM: {bgm}");
        BGMSoundData data = bgmSoundDatas.Find(d => d.bgm == bgm);
        if (data != null && data.audioClip != null)
        {
            if (bgmAudioSource == null) { Debug.LogError("[SoundManager] BGM AudioSourceが割り当てられていません！"); return; }

            if (bgmAudioSource.clip == data.audioClip && bgmAudioSource.isPlaying)
            {
                Debug.Log($"[SoundManager] BGM '{bgm}' は既に再生中です。");
                return;
            }

            bgmAudioSource.clip = data.audioClip;
            bgmAudioSource.volume = data.volume;
            bgmAudioSource.loop = true; // この行を追加
            bgmAudioSource.Play();
            Debug.Log($"[SoundManager] BGM '{bgm}' を再生しました。 AudioClip: {data.audioClip.name}");
        }
        else
        {
            Debug.LogWarning($"[SoundManager] BGM '{bgm}' のデータまたはオーディオクリップが見つかりません。");
        }
    }

    /// <summary>
    /// SEを再生します。
    /// </summary>
    /// <param name="se">再生するSEの種類</param>
    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = seSoundDatas.Find(d => d.se == se);
        if (data != null && data.audioClip != null)
        {
            if (seAudioSource == null) { Debug.LogError("[SoundManager] SE AudioSourceが割り当てられていません！"); return; }

            seAudioSource.PlayOneShot(data.audioClip, data.volume);
            Debug.Log($"[SoundManager] SE '{se}' を再生しました。 AudioClip: {data.audioClip.name}");
        }
        else
        {
            Debug.LogWarning($"[SoundManager] SE '{se}' のデータまたはオーディオクリップが見つかりません。");
        }
    }

    /// <summary>
    /// UIボタンクリック時の汎用SEを再生します。
    /// このメソッドは、SEリストのSESoundData.SE.Clickを再生します。
    /// </summary>
    public void PlayClickSFX()
    {
        Debug.Log("[SoundManager] PlayClickSFXが呼ばれました。");
        PlaySE(SESoundData.SE.Click);
    }

    /// <summary>
    /// BGMの音量を設定し、PlayerPrefsに保存します。UIスライダーからの変更を受け取ります。
    /// </summary>
    /// <param name="value">スライダーの値 (0-1)</param>
    public void SetBGMVolume(float value)
    {
        Debug.Log($"[SoundManager] SetBGMVolume called. Value: {value}.");
        if (audioMixer == null)
        {
            Debug.LogError("[SoundManager] SetBGMVolume: AudioMixerがnullです。");
            return;
        }

        value = Mathf.Clamp01(value);
        float decibel = (value > 0) ? 20f * Mathf.Log10(value) : -80f;
        decibel = Mathf.Clamp(decibel, -80f, 0f);

        audioMixer.SetFloat(BGM_MIXER_PARAM, decibel);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
        Debug.Log($"[SoundManager] BGM Volume set to {decibel} dB. Saved to PlayerPrefs: {value}");
    }

    /// <summary>
    /// SEの音量を設定し、PlayerPrefsに保存します。UIスライダーからの変更を受け取ります。
    /// </summary>
    /// <param name="value">スライダーの値 (0-1)</param>
    public void SetSEVolume(float value)
    {
        Debug.Log($"[SoundManager] SetSEVolume called. Value: {value}.");
        if (audioMixer == null)
        {
            Debug.LogError("[SoundManager] SetSEVolume: AudioMixerがnullです。");
            return;
        }

        value = Mathf.Clamp01(value);
        float decibel = (value > 0) ? 20f * Mathf.Log10(value) : -80f;
        decibel = Mathf.Clamp(decibel, -80f, 0f);

        audioMixer.SetFloat(SE_MIXER_PARAM, decibel);
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, value);
        PlayerPrefs.Save();
        Debug.Log($"[SoundManager] SE Volume set to {decibel} dB. Saved to PlayerPrefs: {value}");
    }

    /// <summary>
    /// PlayerPrefsから保存された音量設定を読み込み、AudioMixerに適用します。
    /// このメソッドはStart時に一度呼び出され、全体の初期音量を設定します。
    /// スライダーの初期値設定はInitializeSliders内で行われます。
    /// </summary>
    private void LoadVolumeSettings()
    {
        Debug.Log("[SoundManager] Loading volume settings from PlayerPrefs.");
        float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.75f);
        SetBGMVolume(savedBGMVolume);

        float savedSEVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.75f);
        SetSEVolume(savedSEVolume);
        Debug.Log($"[SoundManager] Loaded BGM: {savedBGMVolume}, SE: {savedSEVolume}");
    }
}

// BGMサウンドデータの構造体
[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        None,
        Title, //タイトル画面のBGM
        Home,　//ホーム画面のBGM
        Battle01,//バトル画面のBGM
        Battle02,//バトル画面のBGM
        Story,//ストーリー画面のBGM
       
    }

    public BGM bgm;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}

// SEサウンドデータの構造体
[System.Serializable]
public class SESoundData
{
    public enum SE
    {
        None,
        Attack, //攻撃SE
        Damage, //ダメージSE
        Magic, //魔法SE
        Defense, //防御SE
        HP_Item, //ポーション回復音
        Click, //ボタンクリックSE
        Dragon, //ドラゴン（ボス）SE
        Page,
        Get,
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}