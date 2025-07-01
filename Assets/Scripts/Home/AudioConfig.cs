using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioConfig :MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider bgmSlider;

    private void Start()
    {
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);
            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel,-80,0f);
            audioMixer.SetFloat("BGM", decibel);

        });

        seSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);
            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel, -80, 0f);
            audioMixer.SetFloat("SE", decibel);

        });
    }
}

