using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using System.Linq;
using LocationLibrary;
using System.Collections.Generic; // Dictionary を使うために必要
using Cysharp.Threading.Tasks;

// --- JSON レスポンスを格納するためのクラス ---
[Serializable]
public class SessionResponse
{
    public string session;
    public long expiry;
}

public class MapLoader : MonoBehaviour
{
    [SerializeField] private string apiKey = "YOUR_YOUR_Maps_API_KEY_HERE";
    [SerializeField] private SpriteRenderer tilePrefab; // ★新しいフィールド: タイル表示用のSpriteRendererを持つPrefab

    private string sessionToken = "";
    private DateTime sessionExpiryTime = DateTime.MinValue;
    private const float sessionRefreshThresholdSeconds = 60f;

    private const string createSessionUrl = "https://asia-northeast1-vivid-pact-460416-b0.cloudfunctions.net/get_map_session_token";
    private const string tileBaseUrl = "https://tile.googleapis.com/v1/2dtiles/{0}/{1}/{2}?session={3}&key={4}";

    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // --- タイルグリッド管理用の変数 ---
    private int currentZoom = 17; // 現在のズームレベル
    public int centerTileX = 6294; // 中心タイルのX座標
    public int centerTileY = 13288; // 中心タイルのY座標

    [SerializeField] private int gridRadius = 2; // 中心から周囲にロードするタイルの半径 (例: 2で5x5グリッド)

    // ロード済みのタイルを保存するDictionary (キー: "Z/X/Y")
    private Dictionary<string, MapTile> loadedTiles = new Dictionary<string, MapTile>();

    // タイルオブジェクトをまとめる親オブジェクト
    [HideInInspector]
    public GameObject mapContainer;

    // --- カメラの参照とスクロール速度 ---
    public Camera mainCamera; // シーンのメインカメラ
    [SerializeField] private float scrollSpeed = 50f; // カメラのスクロール速度
    [SerializeField] private float zoomSpeed = 10f; // ズーム速度
    [SerializeField] private float minZoomSize = 0.75f; // カメラの最小ズームサイズ
    [SerializeField] private float maxZoomSize = 50f; // カメラの最大ズームサイズ

    public LogPathRenderer logPathRenderer; // ログパスレンダラーの参照

    // Unityワールド座標における1タイルのサイズ (PPUをテクスチャ幅に設定した場合、1ユニットとなる)
    private float tileSizeInUnityUnits = 1f; // ★Sprite.CreateのPPU設定により変わる可能性あり

    void Awake()
    {
        // マップコンテナがなければ作成
        mapContainer = new GameObject("MapContainer");
        mapContainer.transform.position = Vector3.zero; // 原点に配置
        mapContainer.transform.rotation = Quaternion.Euler(90, 0, 0); // 2D表示のためX軸90度回転 (オプション)
    }

    public void Init(double initialLat, double initialLon)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_YOUR_Maps_API_KEY_HERE")
        {
            Debug.LogError("APIキーが設定されていません。Inspectorで設定してください。");
            return;
        }
        if (tilePrefab == null)
        {
            Debug.LogError("タイル表示用のSpriteRenderer Prefabがアサインされていません。Inspectorで設定してください。");
            enabled = false;
            return;
        }
        if (mainCamera == null)
        {
            Debug.LogError("カメラが設定されていないため、スクロールできません。");
            // enabled = false; // デバッグのために続行するが、注意を促す
        }

        // 初期ズームレベルでの中心タイルXYZ座標を計算
        // これが初期のcenterTileX, centerTileYになります
        Vector2Int initialTileXY = LatLonToTileXY(initialLat, initialLon, currentZoom);
        centerTileX = initialTileXY.x;
        centerTileY = initialTileXY.y;

        // カメラの初期位置を地図の中心に設定
        // 中心タイル (0,0) が Unity のワールド座標の原点になるように設定
        // mapContainer が (0,0,0) で X軸90度回転している前提
        mainCamera.transform.position = new Vector3(centerTileX, 10, -centerTileY); // 地図から少し浮かせた位置 (Y軸が高さ)
        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0); // 真上から見下ろすように回転
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 1f; // 初期ズームサイズ

        InitializeMap().Forget(); // 非同期初期化を開始
    }

    // 地図の初期化とメインループ
    private async UniTask InitializeMap()
    {
        await GetSessionToken();

        // 初期タイルのロード
        //await LoadSurroundingTiles(centerTileX, centerTileY, currentZoom, gridRadius);

        LoadLogPathTiles().Forget(); // ログパスタイルのロードを開始

        // マップのスクロールを監視するメインループ
        //ManageTilesContinuously().Forget();
    }

    private async UniTask LoadLogPathTiles()
    {
        List<Vector2Int> logPathTiles = logPathRenderer.GetLogPathTiles();

        await UniTask.Delay(100);

        // 10タイルずつロードする
        foreach (var chunk in logPathTiles.Chunk(10))
        {
            await UniTask.WhenAll(chunk.Select(tile => FetchAndDisplayTile(currentZoom, tile.x, -tile.y)));
            await UniTask.Delay(100);
        }

        Debug.Log($"<color=green>ログパスのタイルを{logPathTiles.Count}個ロードしました。</color>");
    }

    private async UniTask GetSessionToken()
    {
        // セッション作成リクエストのボディ
        string jsonBody = "{\"mapType\": \"satellite\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(createSessionUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("セッショントークン取得リクエストを送信中...");
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("セッショントークン取得エラー: " + request.error);
                Debug.LogError("レスポンス: " + request.downloadHandler.text);
            }
            else
            {
                string jsonResponseText = request.downloadHandler.text;
                Debug.Log("セッション取得レスポンス: " + jsonResponseText);

                try
                {
                    // JsonUtilityを使ってレスポンスをパース
                    SessionResponse response = JsonUtility.FromJson<SessionResponse>(jsonResponseText);
                    sessionToken = response.session;

                    Debug.Log("Session Token (JsonUtility): " + sessionToken);

                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSONパースエラー: " + e.Message);
                    sessionToken = "";
                }
            }
        }
    }

    // --- タイル管理と更新のメインコルーチン ---
    private async UniTask ManageTilesContinuously()
    {
        while (true)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                Debug.LogError("セッショントークンが失効しました。タイルロードを停止します。");
                return;
            }

            // --- ★カメラの移動と中心タイル座標の更新ロジック★ ---
            HandleCameraScroll();

            // カメラの現在のワールド座標の中心タイルを計算
            // カメラの Y 軸が上方向 (UnityのY軸) で、マップが XZ 平面（mapContainerがX軸90度回転）にあると仮定
            // カメラの transform.position.x が地図のX軸に対応
            // カメラの transform.position.z が地図のY軸に対応 (mapContainerが回転しているため)

            // カメラの移動量に基づいて中心タイルを更新する、より正確なロジック
            // このロジックは、地図の原点を中心タイル(0,0)とした座標系に変換し、そこからカメラの位置を計算します。
            Vector3 cameraLocalPos = mapContainer.transform.InverseTransformPoint(mainCamera.transform.position);

            // mapContainerがX軸90度回転しているため、mapContainerのローカルYがUnityのワールドZに相当
            // mapContainerのローカルXがUnityのワールドXに相当
            int newCenterX = (int)Math.Round(cameraLocalPos.x / tileSizeInUnityUnits);
            int newCenterY = (int)Math.Round(-cameraLocalPos.y / tileSizeInUnityUnits); // mapContainerのローカルY
            // Debug.Log($"Camera Local Position: {cameraLocalPos}, Calculated Tile: ({calculatedTileX}, {calculatedTileY})");

            // ズームレベルが変わるとタイル座標のスケールも変わるため、一旦静的座標を使う
            // 後で動的なズームに対応させる

            // もし中心タイルが移動したら、新しいタイルをロードし、古いタイルをアンロード
            if (newCenterX != centerTileX || newCenterY != centerTileY)
            {
                Debug.Log("<color=yellow>中心タイルが移動しました: " + newCenterX + ", " + newCenterY + "</color>");
                centerTileX = newCenterX;
                centerTileY = newCenterY;
                await LoadSurroundingTiles(centerTileX, centerTileY, currentZoom, gridRadius);
            }

            await UniTask.Yield(); // 1フレーム待つ
        }
    }

    // --- ユーザー入力によるカメラのスクロール処理 ---
    private void HandleCameraScroll()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/Dキーまたは左右矢印
        float vertical = Input.GetAxis("Vertical");     // W/Sキーまたは上下矢印

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical); // Y軸は固定
                                                                      // mapContainerをX軸90度回転させている場合、カメラのZは地図のY（南北）に相当するので、
                                                                      // verticalはz、horizontalはxに適用。
                                                                      // ※カメラがワールドのZ軸方向を向いていると仮定。

        // カメラのForward方向が地図の北方向（地図の-Z方向）になるように調整
        // 地図のZ軸をY軸に変換している場合、カメラの移動方向も調整する必要がある
        // ここでは簡単に、カメラのローカル座標で移動する
        // mapContainerの回転によっては、カメラのローカルXが地図のX、ローカルZが地図のYに相当する場合が多い
        mainCamera.transform.position += mainCamera.transform.right * horizontal * scrollSpeed * Time.deltaTime;
        mainCamera.transform.position += mainCamera.transform.forward * vertical * scrollSpeed * Time.deltaTime; // forward は通常Z軸方向

        // マウスドラッグでのスクロール（2Dの場合）
        if (Input.GetMouseButton(0)) // 左クリックを押しっぱなし
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // マウスの動きに合わせてカメラを移動 (逆方向になることが多いので-を付ける)
            // Pixel Per Unit設定やカメラのOrthographicSizeによってスケール調整が必要
            mainCamera.transform.position -= mainCamera.transform.right * mouseX * scrollSpeed * 0.1f * Time.deltaTime;
            mainCamera.transform.position -= mainCamera.transform.up * mouseY * scrollSpeed * 0.1f * Time.deltaTime; // Orthographicカメラならupが上下
        }

        // --- ★タッチ入力でのスクロール (モバイル向け) ---
        if (Input.touchCount == 1) // 指1本でのタッチの場合
        {
            Touch touch = Input.GetTouch(0); // 1本目の指の情報を取得

            // 指が移動した場合
            if (touch.phase == TouchPhase.Moved)
            {
                // touch.deltaPosition は、前フレームからのタッチ位置の差分（ピクセル単位）
                Vector2 touchDelta = touch.deltaPosition;

                // カメラのorthographicSizeに応じて移動量をスケール
                // カメラのorthographicSize * 2f はワールド空間でのカメラの高さ
                // Screen.height は画面のピクセル高さ
                float cameraWorldHeight = mainCamera.orthographicSize * 2f;
                float worldUnitsPerPixel = cameraWorldHeight / Screen.height;

                // タッチの移動ピクセルをワールド単位に変換し、カメラを移動
                // マウスドラッグと同様に、地図を「引っ張る」動きなので、移動方向を反転
                Vector3 touchMoveDelta = new Vector3(-touchDelta.x * worldUnitsPerPixel, -touchDelta.y * worldUnitsPerPixel, 0);

                // カメラのローカル軸に変換して移動
                mainCamera.transform.position += mainCamera.transform.TransformDirection(touchMoveDelta);
            }
        }

        // ズーム（マウスホイール）
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Orthographicカメラの場合、sizeを変更
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = Mathf.Min(Mathf.Max(minZoomSize, mainCamera.orthographicSize - scroll * 10f), maxZoomSize); // ズーム速度調整
                // ★注意: ズームレベルが変わると、取得するタイルのZ座標も変更する必要がある
                // ここで currentZoom を更新し、再度 LoadSurroundingTiles を呼び出すロジックが必要
            }
            // Perspectiveカメラの場合、Z座標を変更
            else
            {
                mainCamera.transform.position += mainCamera.transform.forward * scroll * 10f; // ズーム速度調整
            }
        }

        // --- ★ピンチジェスチャーでのズーム (モバイル向け) ---
        if (Input.touchCount == 2) // 指2本でのタッチの場合 (ピンチイン/アウト)
        {
            // 最初の2本の指の情報を取得
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 前フレームでのそれぞれの指の位置を計算
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 前フレームと現フレームでの指の間の距離を計算
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // ズームの変化量
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // orthographicSize を更新
            if (mainCamera.orthographic)
            {
                float newSize = mainCamera.orthographicSize + deltaMagnitudeDiff * zoomSpeed * 0.1f * Time.deltaTime; // ズーム感度調整
                mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoomSize, maxZoomSize);
            }
        }
    }


    // --- 周囲のタイルをロード・アンロードするメインロジック ---
    private async UniTask LoadSurroundingTiles(int centerX, int centerY, int zoom, int radius)
    {
        HashSet<string> tilesToKeep = new HashSet<string>(); // 今回表示するタイルを追跡

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int tileX = centerX + x;
                int tileY = centerY + y;
                string tileKey = $"{zoom}/{tileX}/{tileY}";

                tilesToKeep.Add(tileKey); // 表示するタイルとして追加

                // すでにロード済みで、かつ正しい場所にいるか
                if (loadedTiles.ContainsKey(tileKey))
                {
                    // 必要であれば、位置や表示順（ソートオーダー）を再確認
                    // Debug.Log($"Tile {tileKey} already loaded.");
                    continue;
                }

                // 新しいタイルをロード
                FetchAndDisplayTile(zoom, tileX, tileY).Forget();
            }
        }

        // 画面外に出て不要になったタイルをアンロード（破棄）
        List<string> tilesToRemove = new List<string>();
        foreach (var entry in loadedTiles)
        {
            if (!tilesToKeep.Contains(entry.Key))
            {
                tilesToRemove.Add(entry.Key);
            }
        }

        foreach (string key in tilesToRemove)
        {
            if (loadedTiles.TryGetValue(key, out MapTile tile))
            {
                tile.DestroyTile();
                loadedTiles.Remove(key);
                Debug.Log($"Tile {key} unloaded.");
            }
        }

        await UniTask.Yield();
    }

    // --- 緯度経度 <-> タイル座標 変換ヘルパーメソッド ---
    // 参考: https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
    public static Vector2Int LatLonToTileXY(double lat, double lon, int zoom)
    {
        // 緯度経度をラジアンに変換
        double latRad = lat * Mathf.Deg2Rad;

        // n = 2^zoom
        double n = Math.Pow(2, zoom);

        // Xタイル座標
        int tileX = (int)Math.Floor((lon + 180.0) / 360.0 * n);

        // Yタイル座標 (メルカトル投影)
        int tileY = (int)Math.Floor((1.0 - Math.Log(Math.Tan(latRad) + 1 / Math.Cos(latRad)) / Math.PI) / 2.0 * n);

        return new Vector2Int(tileX, tileY);
    }

    public static Vector2 TileXYToLatLon(int tileX, int tileY, int zoom)
    {
        double n = Math.Pow(2, zoom);

        // 経度 (Lon)
        double lon = tileX / n * 360.0 - 180.0;

        // 緯度 (Lat)
        double latRad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * tileY / n)));
        double lat = latRad * Mathf.Rad2Deg;

        return new Vector2((float)lat, (float)lon);
    }

    // Unityワールド座標への変換 (このロジックは、Unityでのタイルの表示スケールに強く依存します)
    // MapContainerのローカル座標系における緯度経度の位置を返します
    public Vector3 LatLonToUnityLocalPosition(double lat, double lon)
    {
        // 緯度経度からピクセル座標に変換 (Webメルカトル基準)
        // ズームレベル0で256x256ピクセルと仮定
        double n = Math.Pow(2, currentZoom);
        double tileWorldSizePx = n * 256; // 世界全体でのピクセルサイズ (ズームレベル0で256)

        // 経度からXピクセル
        double pixelX = (lon + 180.0) / 360.0 * tileWorldSizePx;

        // 緯度からYピクセル (メルカトル投影)
        double latRad = lat * Mathf.Deg2Rad;
        Debug.Log($"latRad: {latRad}");
        double pixelY = (1.0 - Math.Log(Math.Tan(latRad) + 1 / Math.Cos(latRad)) / Math.PI) / 2.0 * tileWorldSizePx;

        // ここで、現在の中心タイルの原点からのオフセットを考慮
        // 現在の中心タイル (centerTileX, centerTileY) が Unity のワールド原点に相当すると仮定
        // pixelX, pixelY は絶対ピクセル座標なので、そこから中心タイルのピクセル座標を引く
        // centerTileX, centerTileY は現在のズームレベルでの中心タイル座標
        // double centerTilePixelX = centerTileX * 256; // 中心タイルの左上ピクセルX座標
        // double centerTilePixelY = centerTileY * 256; // 中心タイルの左上ピクセルY座標

        // ピクセル座標をUnityユニットに変換 (PPU=256の場合、1タイル=1ユニットなので、1ピクセル=1/256ユニット)
        // newSprite の PPU を 256 に設定したと仮定 (texture.width)
        // タイルのピボットを中央に設定しているため、-0.5f で中心を調整
        double unityX = pixelX / 256f * tileSizeInUnityUnits - 0.5f;
        double unityY = pixelY / 256f * tileSizeInUnityUnits - 0.5f;

        // mapContainer が X軸90度回転している場合、地図のY軸がUnityのZ軸に対応
        return new Vector3((float)unityX, (float)-unityY, 0); // X, Y平面に配置
    }

    private float GetTileScaleForZoom(int zoomLevel)
    {
        // 例えば、ズームレベル18が1ユニット（基準）
        // ズームレベル17は2ユニット
        // ズームレベル16は4ユニット
        // ズームレベルNのタイルは、基準となるズームレベルに対する2の冪乗倍のスケールを持つ
        return tileSizeInUnityUnits * Mathf.Pow(2, 18 - zoomLevel);
    }

    // --- 個々のタイルをフェッチして表示するコルーチン ---
    private async UniTask FetchAndDisplayTile(int z, int x, int y)
    {
        //Debug.Log($"Fetching tile: {z}/{x}/{y}");
        string tileUrl = string.Format(tileBaseUrl, z, x, y, sessionToken, apiKey);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(tileUrl))
        {
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"タイル画像取得エラー ({z}/{x}/{y}): {request.error}");
                // Debug.LogError($"レスポンス: {request.downloadHandler.text}"); // エラー時のレスポンスも確認
            }
            else
            {
                Debug.Log(request.downloadHandler.text); // レスポンスの内容をデバッグログに出力
                Texture2D texture = DownloadHandlerTexture.GetContent(request);


                // タイルオブジェクトを生成
                SpriteRenderer newTileSpriteRenderer = Instantiate(tilePrefab, mapContainer.transform);

                //float currentTileScale = GetTileScaleForZoom(z); // そのタイルのズームレベルに応じたスケールを取得
                //newTileSpriteRenderer.transform.localScale = Vector3.one * currentTileScale; // スケールを適用

                // Texture2D から Sprite を作成
                Sprite newSprite = Sprite.Create(texture,
                                                new Rect(0, 0, texture.width, texture.height),
                                                new Vector2(0.5f, 0.5f), // ピボットを中央に
                                                texture.width // Pixels Per Unit (PPU) をテクスチャ幅に設定すると、1単位が1ピクセルになる
                                                );

                newTileSpriteRenderer.sprite = newSprite;
                newTileSpriteRenderer.name = $"Tile_{z}_{x}_{y}";

                // --- ここが重要: タイルのUnityワールド座標を計算 ---
                // このロジックは、ゲーム内の地図のスケールと原点に依存します。
                // 例: タイルが1 Unityユニット = 1テクスチャ幅 になるようにPPUを設定した場合
                float tileSizeInUnits = texture.width / newSprite.pixelsPerUnit; // 1ユニット=1ピクセルなら1

                // 地図の原点からのオフセットを計算
                float tileWorldX = x * tileSizeInUnits;
                float tileWorldY = y * tileSizeInUnits; // Yは地図の南北方向

                // mapContainer のローカル座標で位置を設定
                // mapContainerがX軸90度回転している場合、ローカルYがワールドのZになる
                newTileSpriteRenderer.transform.localPosition = new Vector3(tileWorldX, -tileWorldY, 0f); // X, Y平面に配置

                // レイヤー順序（z値）を設定して、タイル同士の重なりを防ぐ
                // 例えば、全てのタイルのZ値を0に固定すればOK。または、カメラからの距離でソート。
                // 2Dゲームなら SpriteRenderer.sortingOrder を使うのが一般的。
                newTileSpriteRenderer.sortingOrder = 0;

                loadedTiles[new MapTile(newTileSpriteRenderer.gameObject, z, x, y).GetTileKey()] = new MapTile(newTileSpriteRenderer.gameObject, z, x, y);
                Debug.Log($"Tile {z}/{x}/{y} loaded and displayed.");
            }
        }
    }
}
