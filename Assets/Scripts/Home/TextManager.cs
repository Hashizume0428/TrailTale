using UnityEngine;
using TMPro; // TextMeshProUGUIを使用する場合は必須です。
using System.Collections; // コルーチンを使用する場合は必須です。
using System.Collections.Generic; // Queueを使用する場合は必須です。

// MonoBehaviourを継承することでオブジェクトにコンポーネントとして
// アタッチすることができるようになる
public class TextManager : MonoBehaviour
{
    // SerializeFieldと書くとprivateなパラメーターでも
    // インスペクター上で値を変更できる
    [SerializeField]
    private TextMeshProUGUI mainText; // メインのテキスト表示用

    [SerializeField]
    private TextMeshProUGUI speedDisplayText; // 現在の速度表示用

    [Header("Text Settings")] // インスペクターでの表示を分かりやすくする
    // captionSpeedはPlayerPrefsからロードするため、ここでは[SerializeField]を付けません。
    private float captionSpeed;

    // 新しく速度段階を定義する値。インスペクターから調整できるようにSerializeFieldも付けます。
    [SerializeField, Header("Caption Speeds (seconds per char)")]
    private float fastSpeed = 0.02f;     // 早い速度（秒/文字）
    [SerializeField]
    private float normalSpeed = 0.05f; // 普通の速度（秒/文字）
    [SerializeField]
    private float slowSpeed = 0.1f;    // 遅い速度（秒/文字）

    // テキストのページ区切り文字
    private const char SEPARATE_PAGE = '&';
    // PlayerPrefsで使用するキー名（定数）
    private const string CAPTION_SPEED_KEY = "CaptionSpeed"; // 正しいキー名

    // 表示するテキスト全体。インスペクターで複数行入力できるようにします。
    [TextArea(3, 10)]
    [SerializeField]
    private string _fullStoryText =
        "Hello,World!&これはテキスト表示のサンプルです&こんにちは！&次のページはこれで終わりだよ。";

    // 1文字ずつ表示するためのキュー
    private Queue<char> _charQueue;
    // ページ（行）ごとに表示するためのキュー
    private Queue<string> _pageQueue;

    // 現在実行中の文字送りコルーチンを保持
    private Coroutine _displayCoroutine;

    // MonoBehaviourを継承している場合限定で
    // 最初の更新関数(Updateメソッド)が呼ばれる時に最初に呼ばれる
    private void Start()
    {
        // PlayerPrefsから現在の表示速度をロードします。
        // もし保存された設定がなければ、normalSpeedをデフォルトとして使用します。
        captionSpeed = PlayerPrefs.GetFloat(CAPTION_SPEED_KEY, normalSpeed);

        // 初期化処理を開始
        Init();

        // Start()でもUpdateSpeedDisplayText()を呼び出しておくことで、
        // シーンロード時にオブジェクトがアクティブであればすぐに表示される。
        // ただし、OnEnable()でも呼び出すため、設定画面が非アクティブからアクティブになる際にも対応できる。
        UpdateSpeedDisplayText();
    }

    // ゲームオブジェクトがアクティブになるたびに呼び出される
    private void OnEnable()
    {
        // オブジェクトがアクティブになったときに、現在の速度表示を更新
        // これにより、設定画面が非アクティブからアクティブになった際にも表示が更新される
        // ただし、Start()よりOnEnable()の方が早く実行されることがあるため、
        // captionSpeedがまだロードされていない可能性も考慮する
        if (captionSpeed == 0 && PlayerPrefs.HasKey(CAPTION_SPEED_KEY)) // まだロードされていない、かつキーが存在する場合
        {
            captionSpeed = PlayerPrefs.GetFloat(CAPTION_SPEED_KEY, normalSpeed);
        }
        else if (captionSpeed == 0) // キーも存在しない場合
        {
            captionSpeed = normalSpeed;
        }

        UpdateSpeedDisplayText();
    }

    // MonoBehaviourを継承している場合限定で
    // 毎フレーム呼ばれる
    private void Update()
    {
        // 左(=0)クリックされたらOnClickメソッドを呼び出し
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    /// <summary>
    /// 文字列を指定した区切り文字ごとに区切り、キューに格納したものを返す（ページ区切り用）
    /// </summary>
    private Queue<string> SeparatePages(string str, char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs)
        {
            queue.Enqueue(l);
        }
        return queue;
    }

    /// <summary>
    /// 文を1文字ごとに区切り、キューに格納したものを返す（セリフの文字送り用）
    /// </summary>
    private Queue<char> SeparateCharacters(string str)
    {
        char[] chars = str.ToCharArray();
        Queue<char> charQueue = new Queue<char>();
        foreach (char c in chars)
        {
            charQueue.Enqueue(c);
        }
        return charQueue;
    }

    /// <summary>
    /// キューから1文字を取り出して表示し、キューが空になったらfalseを返します。
    /// </summary>
    private bool OutputChar()
    {
        if (_charQueue == null || _charQueue.Count <= 0)
        {
            return false; // キューに何も格納されていなければfalseを返す
        }
        mainText.text += _charQueue.Dequeue();
        return true;
    }

    /// <summary>
    /// 文字送りを行うコルーチン
    /// </summary>
    /// <param name="wait">次の文字を表示するまでの待機時間（秒）</param>
    private IEnumerator ShowChars(float wait)
    {
        // OutputCharメソッドがfalseを返す（キューが空になる）までループします
        while (OutputChar())
        {
            yield return new WaitForSeconds(wait); // 指定された時間だけ待機
        }
        _displayCoroutine = null; // コルーチンが終了したらnullにします
        yield break;
    }

    /// <summary>
    /// 1行のテキストを読み込み、文字キューに格納して文字送りを開始します。
    /// </summary>
    /// <param name="text">表示するテキスト行</param>
    private void ReadLine(string text)
    {
        // 既存の文字送りコルーチンがあれば停止します
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }

        mainText.text = ""; // メインテキストを一度クリア
        _charQueue = SeparateCharacters(text); // テキスト全体を文字キューに変換

        // 現在のcaptionSpeedを使って新しい文字送りコルーチンを開始し、参照を保持します
        _displayCoroutine = StartCoroutine(ShowChars(captionSpeed));
    }

    /// <summary>
    /// 全文を瞬時に表示します。
    /// </summary>
    private void OutputAllChar()
    {
        // 文字送りコルーチンが実行中であれば停止します
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }

        // キューが空になるまで残りの文字を全て表示します
        while (OutputChar()) ;
    }

    /// <summary>
    /// 初期化処理（最初のページを読み込みます）
    /// </summary>
    private void Init()
    {
        _pageQueue = SeparatePages(_fullStoryText, SEPARATE_PAGE);
        ShowNextPage();
    }

    /// <summary>
    /// 次のページ（行）を表示します。すべてのページ表示が完了したらfalseを返します。
    /// </summary>
    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0)
        {
            Debug.Log("すべてのテキストページを表示しました。");
            return false;
        }
        ReadLine(_pageQueue.Dequeue());
        return true;
    }

    /// <summary>
    /// クリックしたときの処理（文字送り中の場合は全文表示、完了していれば次のページへ進みます）
    /// </summary>
    public void OnClick() // UIボタンから呼び出せるようにpublicにしています
    {
        // まだ文字送り中であれば全文表示
        if (_charQueue != null && _charQueue.Count > 0)
        {
            OutputAllChar();
        }
        else
        {
            // 全文表示が終わっていれば次のページへ
            if (!ShowNextPage())
            {
                // 全てのページ表示が完了した場合の処理
                // 例: シーン遷移、特定のイベントの発生など
                Debug.Log("物語が終了しました！");
            }
        }
    }

    // --- 速度設定メソッド ---
    /// <summary>
    /// 表示速度を「早い」に設定し、PlayerPrefsに保存します。
    /// </summary>
    public void SetSpeedFast()
    {
        captionSpeed = fastSpeed;
        PlayerPrefs.SetFloat(CAPTION_SPEED_KEY, captionSpeed); // PlayerPrefsに速度を保存
        UpdateSpeedDisplayText(); // 速度表示テキストを更新
        Debug.Log("文字送り速度を「早い」に設定しました: " + captionSpeed);
    }

    /// <summary>
    /// 表示速度を「普通」に設定し、PlayerPrefsに保存します。
    /// </summary>
    public void SetSpeedNormal()
    {
        captionSpeed = normalSpeed;
        PlayerPrefs.SetFloat(CAPTION_SPEED_KEY, captionSpeed); // PlayerPrefsに速度を保存
        UpdateSpeedDisplayText(); // 速度表示テキストを更新
        Debug.Log("文字送り速度を「普通」に設定しました: " + captionSpeed);
    }

    /// <summary>
    /// 表示速度を「遅い」に設定し、PlayerPrefsに保存します。
    /// </summary>
    public void SetSpeedSlow()
    {
        captionSpeed = slowSpeed;
        PlayerPrefs.SetFloat(CAPTION_SPEED_KEY, captionSpeed); // PlayerPrefsに速度を保存
        UpdateSpeedDisplayText(); // 速度表示テキストを更新
        Debug.Log("文字送り速度を「遅い」に設定しました: " + captionSpeed);
    }

    /// <summary>
    /// 現在の表示速度に応じて、速度表示テキストを更新します。
    /// </summary>
    private void UpdateSpeedDisplayText()
    {
        if (speedDisplayText == null)
        {
            Debug.LogWarning("Speed Display Text (TextMeshProUGUI) is not assigned in the Inspector.");
            return;
        }

        string speedText = "";
        // 浮動小数点数の比較には Mathf.Approximately を使用します
        if (Mathf.Approximately(captionSpeed, fastSpeed))
        {
            speedText = "はやい";
        }
        else if (Mathf.Approximately(captionSpeed, normalSpeed))
        {
            speedText = "ふつう";
        }
        else if (Mathf.Approximately(captionSpeed, slowSpeed))
        {
            speedText = "ゆっくり";
        }
        else
        {
            speedText = "不明な速度"; // 予期しない値の場合
        }

        speedDisplayText.text = "現在の表示速度：" + speedText;
    }
}