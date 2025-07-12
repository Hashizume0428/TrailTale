using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingPanel;

    [SerializeField]
    private TextMeshProUGUI loadingText;

    CancellationTokenSource cts;

    private void Start()
    {
        loadingPanel.SetActive(false);
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
            await UniTask.Delay(800, cancellationToken: token);
            loadingText.text = "ロード中.";
            await UniTask.Delay(800, cancellationToken: token);
            loadingText.text = "ロード中..";
            await UniTask.Delay(800, cancellationToken: token);
            loadingText.text = "ロード中...";
            await UniTask.Delay(800, cancellationToken: token);
        }
    }
}
