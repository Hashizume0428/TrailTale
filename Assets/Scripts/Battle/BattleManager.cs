using UnityEngine;
using System;
using EventLibrary;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private EnemyGenerator enemyGenerator; // 敵生成の管理クラス

    [SerializeField]
    private HeroController heroController; // ヒーローの管理クラス

    [SerializeField]
    private ResultPanel resultPanel; // 結果画面のパネル

    [SerializeField]
    private StatusDataBase statusDataBase; // ステータスデータベース

    [SerializeField]
    private PlayerData playerData; // プレイヤーデータ
    
    private Status status; // ランダムに選ばれたステータス

    private void Start()
    {
        enemyGenerator.Init(playerData); // プレイヤーデータを敵生成に渡す
        heroController.Init(playerData); // プレイヤーデータをヒーローに渡す
        heroController.OnEnemyKilled += HandleEnemyKilled; // 敵を倒したときのイベントを登録
        status = statusDataBase.GetRandomStatus();
        resultPanel.Setup(status.icon, 1, "敵を全て倒した！\nあなたの\n" + status.name + "\nが1上がった！", () => { SceneLoader.Instance.LoadMainScene("Story"); }); // 結果パネルの初期化
    }

    public void HandleEnemyKilled(int killCount)
    {
        Debug.Log($"敵を倒しました。現在のキル数: {killCount}");
        if (killCount >= enemyGenerator.maxEnemyCount + enemyGenerator.bossCount)
        {
            Debug.Log("All enemies defeated!");
            ShowResult();
        }
    }

    public void ShowResult()
    {
        playerData.UpdateStatus(status.statusType, 1);
        // 勝利画面や結果画面を表示する処理
        Debug.Log("All enemies defeated! Showing result screen.");
        // ここに結果画面を表示するコードを追加
        resultPanel.Show();
    }
}