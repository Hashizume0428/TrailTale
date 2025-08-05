using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [Header("敵の設定")]
    [SerializeField] private GameObject[] enemyPrefab; // 生成する敵のプレハブ
    [SerializeField] private GameObject[] bossPrefab; // ボスのプレハブ
    [SerializeField] private int enemyTypeCount = 3; // 敵の種類数（外部から設定）
    [SerializeField] private float[] generateIntervals = { 10.0f, 20.0f, 4.0f }; // 各敵の生成間隔
    public int maxEnemies = 10; // 最大同時出現敵数

    [Header("Current Status")]
    [SerializeField] private int currentEnemyCount = 0; // 現在の敵の数

    public int EnemySumCount = 0; // 現在までの敵の総数
    public int maxEnemyCount = 10; // 敵の出現総数
    public int bossCount = 0; // ボスの数

    private float[] generateTimers; // 各敵タイプの生成タイマー

    private PlayerData playerData;

    // 敵生成の設定を格納する構造体
    [System.Serializable]
    public struct GeneratorSettings
    {
        public int enemyTypeCount; // 敵の種類数
        public float[] generateSpeed; // 各敵の生成速度配列
        public int enemyCount; // 現在の敵の数
        public int enemyMax; // 最大敵数
    }

    public GeneratorSettings gen; // 敵生成の設定

    public void Init(PlayerData playerData)
    {
        this.playerData = playerData;
        // 敵の種類数を設定
        gen.enemyTypeCount = enemyTypeCount;

        // 生成間隔配列の初期化
        if (generateIntervals == null || generateIntervals.Length != gen.enemyTypeCount)
        {
            generateIntervals = new float[gen.enemyTypeCount];
            // デフォルト値を設定
            for (int i = 0; i < gen.enemyTypeCount; i++)
            {
                generateIntervals[i] = 3.0f; // デフォルトの生成間隔
            }
        }

        // 生成速度配列の初期化
        gen.generateSpeed = new float[gen.enemyTypeCount];
        for (int i = 0; i < gen.enemyTypeCount; i++)
        {
            gen.generateSpeed[i] = generateIntervals[i];
        }

        // タイマー配列の初期化
        generateTimers = new float[gen.enemyTypeCount];
        for (int i = 0; i < gen.enemyTypeCount; i++)
        {
            generateTimers[i] = 0f;
        }

        // その他の設定
        gen.enemyCount = 0;
        gen.enemyMax = maxEnemies;
        currentEnemyCount = 0;

        if (playerData.battleType == (int)EventLibrary.BattleType.Boss)
        {
            bossCount = 1; // ボス戦の場合はボスの数を1に設定
            Invoke("GenerateBoss", 10.0f);
        }
        else
        {
            bossCount = 0; // 通常戦闘ではボスは存在しない
        }
    }
    
    void Update()
    {
        if (EnemySumCount < maxEnemyCount)
        {
            UpdateEnemyGeneration();
        }
        else
        {
            // print("Maximum enemy count reached: " + EnemySumCount);
            // 最大敵数に達した場合は生成を停止
            StopAllGeneration();
        }

    }

    // 敵生成の更新処理
    private void UpdateEnemyGeneration()
    {
        // 各敵タイプごとに生成処理を実行
        for (int enemyType = 0; enemyType < gen.enemyTypeCount; enemyType++)
        {
            UpdateEnemyTypeGeneration(enemyType);
        }
    }

    // 特定の敵タイプの生成処理
    private void UpdateEnemyTypeGeneration(int enemyType)
    {
        // タイマーの更新
        generateTimers[enemyType] += Time.deltaTime;

        // 生成条件のチェック
        bool canGenerate = generateTimers[enemyType] >= gen.generateSpeed[enemyType];
        bool hasSpace = gen.enemyCount < maxEnemyCount;
        bool hasPrefab = (enemyType < enemyPrefab.Length) && (enemyPrefab[enemyType] != null);

        if (canGenerate && hasSpace && hasPrefab)
        {
            GenerateEnemy(enemyType);
            // タイマーのリセット
            generateTimers[enemyType] -= gen.generateSpeed[enemyType];
        }
    }
    // 敵の生成処理
    private void GenerateEnemy(int enemyType)
    {
        // 生成位置の計算（ランダムで少し位置をずらす）
        Vector3 spawnPosition = new Vector3(2.8f, Random.Range(-0.2f, 0.3f), 0.0f);

        // 敵の生成
        GameObject newEnemy = Instantiate(enemyPrefab[enemyType], spawnPosition, Quaternion.identity);

        // カウンターの更新
        gen.enemyCount++;
        currentEnemyCount++;
        EnemySumCount++;

        // デバッグログ
        Debug.Log($"Enemy Type {enemyType} generated. Total enemies: {gen.enemyCount}");
    }

    // // 敵が破壊されたときの処理（EnemyControllerから呼び出し用）
    public void OnEnemyDestroyed()
    {
        gen.enemyCount--;
        currentEnemyCount--;

        if (gen.enemyCount < 0)
        {
            gen.enemyCount = 0;
            currentEnemyCount = 0;
        }
    }

    // 特定の敵タイプの生成間隔を動的に変更
    public void SetEnemyGenerationInterval(int enemyType, float newInterval)
    {
        if (enemyType >= 0 && enemyType < gen.enemyTypeCount)
        {
            gen.generateSpeed[enemyType] = newInterval;
            generateIntervals[enemyType] = newInterval;
        }
    }

    // 全ての敵タイプの生成を停止
    public void StopAllGeneration()
    {
        for (int i = 0; i < generateTimers.Length; i++)
        {
            generateTimers[i] = 0f;
        }
    }
    // ボスを生成するメソッド
    public void GenerateBoss()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Dragon);
        if (bossPrefab.Length > 0)
        {
            // ボスの生成位置を設定
            Vector3 spawnPosition = new Vector3(4.0f, 0.1f, 0.0f);
            GameObject boss = Instantiate(bossPrefab[0], spawnPosition, Quaternion.identity); // 0番目のプレハブをボスとして使用
        }
        bossPrefab = new GameObject[0]; // ボス生成後はプレハブを空にする
        Debug.Log("Boss generated!");
    }
}
