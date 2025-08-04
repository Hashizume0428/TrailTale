using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSEPlayer : MonoBehaviour
{
    // 再生したいSEの種類をインスペクターで設定
    public SESoundData.SE seType;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Buttonコンポーネントが見つかりません。このスクリプトはButtonにアタッチしてください。");
            return;
        }

        // ボタンのクリックイベントにPlaySEメソッドを登録
        button.onClick.AddListener(PlaySE);
    }

    /// <summary>
    /// 設定されたSEを再生する
    /// </summary>
    private void PlaySE()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(seType);
        }
        else
        {
            Debug.LogWarning("SoundManagerのインスタンスが見つかりません。");
        }
    }
}