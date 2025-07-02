using UnityEngine;
using TMPro; // TextMeshProUGUIを使う場合は必須です。
using System.Collections; // コルーチンを使う場合は必須です。
using System.Collections.Generic; // Queueを使う場合は必須です。

// MonoBehaviourを継承することでオブジェクトにコンポーネントとして
// アタッチすることができるようになる
public class TextManager : MonoBehaviour
{
    // SerializeFieldと書くとprivateなパラメーターでも
    // インスペクター上で値を変更できる
    [SerializeField]
    private TextMeshProUGUI mainText; // メインのテキスト表示用
    // private TextMeshProUGUI nameText; // 名前の表示用 → 誰が話しているかの機能は削除済みのため、ここでは削除しませんが、もしnameTextが使われていない場合は削除してください。

    [Header("Text Settings")] // インスペクターでの表示を分かりやすくする
    [SerializeField]
    private float captionSpeed = 0.05f; // 1文字表示ごとの待機時間 (秒)

    // テキスト分割用の定数
    // private const char SEPARATE_MAIN_START = '「'; // 不要であれば削除
    // private const char SEPARATE_MAIN_END = '」';   // 不要であれば削除
    private const char SEPARATE_PAGE = '&'; // ページ区切り文字

    // テスト用のテキスト。名前のフォーマットが不要になるため修正しました。
    [TextArea(3, 10)] // インスペクターで複数行入力できるようにする
    [SerializeField]
    private string _fullStoryText =
        "Hello,World!&これはテキスト表示のサンプルです&こんにちは！&次のページはこれで終わりだよ。"; // 名前表示を削除した場合のサンプル

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
        // 初期化処理を開始
        Init();
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
    /// キューから1文字を取り出して表示する
    /// キューが空になったらfalseを返す
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
    /// 文字送りするコルーチン
    /// </summary>
    private IEnumerator ShowChars(float wait)
    {
        // OutputCharメソッドがfalseを返す(=キューが空になる)までループする
        while (OutputChar())
        {
            yield return new WaitForSeconds(wait); // wait秒だけ待機
        }
        _displayCoroutine = null; // コルーチンが終了したらnullにする
        yield break;
    }

    /// <summary>
    /// 1行のテキストを読み込み、キューに格納し、文字送りを開始する
    /// </summary>
    private void ReadLine(string text)
    {
        // 既存の文字送りコルーチンがあれば停止
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }

        // 名前表示機能が不要になったため、名前の分割や設定に関する処理を削除します。
        // もし以前の_fullStoryTextの形式を維持している場合、ここでの変更も考慮してください。
        // （例えば、"ナレーター「セリフ」"のような形式から「ナレーター」と「」を削除する必要がある場合）
        // 現在の_fullStoryTextのフォーマット（名前なし）に合わせて調整しました。

        mainText.text = ""; // メインテキストを一度クリア
        _charQueue = SeparateCharacters(text); // テキスト全体を文字キューに変換

        // 新しい文字送りコルーチンを開始し、参照を保持
        _displayCoroutine = StartCoroutine(ShowChars(captionSpeed));
    }

    /// <summary>
    /// 全文を瞬時に表示する
    /// </summary>
    private void OutputAllChar()
    {
        // 文字送りコルーチンが実行中であれば停止
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }

        // キューが空になるまで残りの文字を全て表示
        while (OutputChar()) ;
    }

    /// <summary>
    /// 初期化する（最初のページを読み込む）
    /// </summary>
    private void Init()
    {
        _pageQueue = SeparatePages(_fullStoryText, SEPARATE_PAGE);
        ShowNextPage();
    }

    /// <summary>
    /// 次のページ（行）を表示する
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
    /// クリックしたときの処理（全文表示または次のページへ進む）
    /// </summary>
    private void OnClick()
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
}