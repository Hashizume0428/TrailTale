using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Loading : MonoBehaviour
{
    public bool playOnAwake = false;

    [SerializeField]
    private GameObject loadingPanel;

    [SerializeField]
    private TextMeshProUGUI loadingText;

    [SerializeField]
    private Image loadingImage;

    [SerializeField]
    private Sprite[] loadingSprites;

    private int currentSpriteIndex = 0;

    CancellationTokenSource cts;

    private void Start()
    {
        if (playOnAwake)
        {
            ShowLoading();
        }
        else
        {
            loadingPanel.SetActive(false);
        }
    }

    public void ShowLoading()
    {
        loadingPanel.SetActive(true);

        cts = new CancellationTokenSource();
        var token = cts.Token;

        LoadingAsync(token).Forget(); // 非同期でローディングを表示
    }

    public void HideLoading()
    {
        cts.Cancel(); // キャンセルしてローディングを停止
        loadingPanel.SetActive(false);
    }

    public async UniTask LoadingAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            loadingText.text = "ロード中";
            UpdateImage();
            await UniTask.Delay(800, cancellationToken: token);
            loadingText.text = "ロード中.";
            UpdateImage();
            await UniTask.Delay(800, cancellationToken: token);
            loadingText.text = "ロード中..";
            UpdateImage();
            await UniTask.Delay(800, cancellationToken: token);
            loadingText.text = "ロード中...";
            UpdateImage();
            await UniTask.Delay(800, cancellationToken: token);
        }
    }

    private void UpdateImage()
    {
        currentSpriteIndex = (currentSpriteIndex + 1) % loadingSprites.Length;
        loadingImage.sprite = loadingSprites[currentSpriteIndex];
    }
}
