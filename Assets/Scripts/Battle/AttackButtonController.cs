using UnityEngine;
using System.Collections.Generic;

public class AttackButtonController : MonoBehaviour
{
    [Header("Attack Button Settings")]
    [SerializeField] int attackCost = 2;
    [SerializeField] int attackDamage = 50;
    [SerializeField] float attackCooldown = 3f; // floatに変更
    [SerializeField] private GameObject hero; // 主人公のGameObject

    private float cooldownTimer = 0f; //クールダウンタイマー
    private HeroController heroController; // HeroControllerの参照を保存
    
    // クールダウン状態を確認するプロパティ
    public bool IsOnCooldown => cooldownTimer < attackCooldown;
    public float CooldownProgress => Mathf.Clamp01(cooldownTimer / attackCooldown);
    public float RemainingCooldown => Mathf.Max(0f, attackCooldown - cooldownTimer);

    void Start()
    {
        heroController = hero.GetComponent<HeroController>();
        cooldownTimer = attackCooldown; // 初期状態ではクールダウン中でない
    }
    void Update()
    {
        // クールダウンタイマーを更新
        if (cooldownTimer < attackCooldown)
        {
            cooldownTimer += Time.deltaTime;
            // クールダウン中は灰色に
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false; // ボタンを無効化
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(128, 128, 128, 255); // 灰色に変更
        }
        // クールダウンが終了したらタイマーをリセット
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true; // ボタンを有効化
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(255, 255, 255, 255); // 元の色に戻す
        }
    }
    
    public void OnClick()
    {
        print("Attack button clicked!");
        
        // クールダウン中の場合は処理を停止
        if (IsOnCooldown)
        {
            print($"Attack is on cooldown! Remaining: {RemainingCooldown:F1}s");
            return;
        }
        
        // コストが足りない場合は処理を停止
        if (heroController.hStatus.cost < attackCost)
        {
            print($"Not enough cost! Required: {attackCost}, Current: {heroController.hStatus.cost}");
            return;
        }
        
        // 攻撃実行
        cooldownTimer = 0f; // クールダウンをリセット
        heroController.hStatus.cost -= attackCost;
        OnClickAttack();
        print($"Attack executed! Cooldown started ({attackCooldown}s)");
    }
    void OnClickAttack()
    {
        List<GameObject> heroControllerEnemies = hero.GetComponent<HeroController>().Enemies;
        //attackArea内の敵のEnemyControllerを取得
        List<EnemyController> enemyControllers = new List<EnemyController>();
        foreach (GameObject enemy in heroControllerEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyControllers.Add(enemyController);
        }
        foreach (EnemyController enemyController in enemyControllers)
        {
            print("Enemy's HP: " + enemyController.eStatus.hp);
            enemyController.OnDamage(attackDamage);
            print("Hero attacks Enemy !!!!! Enemy's HP: " + enemyController.eStatus.hp);
        }
    }
}
