using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioConfig : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    // BGMのAudioSourceはAudioMixerで制御されるため、直接の設定は不要かもしれません。
    [SerializeField] AudioSource bgmAudioSource;
    // SEのAudioSourceをSE再生用として使用します。
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider bgmSlider;

    // 追加: クリック時に鳴らしたいSEのAudioClip
    [SerializeField] private AudioClip clickSFX;

    // PlayerPrefsのキー
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

    private void Start()
    {
        // ----------------------------------------------------
        // 1. 保存された音量設定を読み込み、スライダーに適用する
        // ----------------------------------------------------

        // BGMの音量を読み込み、スライダーに設定
        // デフォルトは0.75f (約-2.5dB) としますが、必要に応じて変更してください
        float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.75f);
        bgmSlider.value = savedBGMVolume;
        // AudioMixerにも初期音量を適用
        SetBGMVolume(savedBGMVolume);

        // SEの音量を読み込み、スライダーに設定
        float savedSEVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.75f);
        seSlider.value = savedSEVolume;
        // AudioMixerにも初期音量を適用
        SetSEVolume(savedSEVolume);

        // ----------------------------------------------------
        // 2. スライダーの値が変更されたときのリスナーを設定する
        // ----------------------------------------------------

        // BGMスライダーのリスナー
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            SetBGMVolume(value);
        });

        // SEスライダーのリスナー
        seSlider.onValueChanged.AddListener((value) =>
        {
            SetSEVolume(value);
        });

        // seAudioSourceの初期設定 (Inspectorで設定済みであれば不要ですが、念のため)
        if (seAudioSource != null)
        {
            seAudioSource.loop = false; // SEは通常ループしない
            seAudioSource.playOnAwake = false; // Awake時に自動再生しない
            // seAudioSource.spatialBlend = 0f; // 2Dサウンドとして再生する場合
        }
    }

    // BGMの音量を設定し、保存する関数
    private void SetBGMVolume(float value)
    {
        // スライダーの値 (0-1) をデシベルに変換
        value = Mathf.Clamp01(value);
        float decibel = 20f * Mathf.Log10(value);
        decibel = Mathf.Clamp(decibel, -80, 0f); // -80dB以下は通常ミュート扱い

        // AudioMixerに適用
        audioMixer.SetFloat("BGM", decibel);

        // PlayerPrefsにスライダーの生の値 (0-1) を保存
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save(); // 変更をすぐに保存
    }

    // SEの音量を設定し、保存する関数
    private void SetSEVolume(float value)
    {
        // スライダーの値 (0-1) をデシベルに変換
        value = Mathf.Clamp01(value);
        float decibel = 20f * Mathf.Log10(value);
        decibel = Mathf.Clamp(decibel, -80, 0f); // -80dB以下は通常ミュート扱い

        // AudioMixerに適用
        audioMixer.SetFloat("SE", decibel);

        // PlayerPrefsにスライダーの生の値 (0-1) を保存
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, value);
        PlayerPrefs.Save(); // 変更をすぐに保存
    }

    /// <summary>
    /// ボタンクリック時に呼び出すSE再生用関数
    /// </summary>
    public void PlayClickSFX()
    {
        if (seAudioSource != null && clickSFX != null)
        {
            // PlayOneShotを使用すると、既存の再生を中断せずに新しい音を重ねて再生できます。
            seAudioSource.PlayOneShot(clickSFX);
        }
        else
        {
            Debug.LogWarning("SE用のAudioSourceまたはクリックSEが設定されていません。");
        }
    }

    // スクリプトが破棄されるときにリスナーを解除 (メモリリーク対策)
    private void OnDestroy()
    {
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
        }
        if (seSlider != null)
        {
            seSlider.onValueChanged.RemoveListener(SetSEVolume);
        }
    }
}