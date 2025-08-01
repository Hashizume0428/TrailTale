using UnityEngine;
using System.Collections.Generic;

public class MagicButonContoroller : MonoBehaviour
{
    [Header("Magic Button Settings")]
    [SerializeField] int magicCost = 7;
    [SerializeField] int magicDamage = 50;
    [SerializeField] int magicCooldown = 8;
    [SerializeField] private GameObject hero; // 主人公のGameObject
    [SerializeField] private GameObject magicTriggerArea; // MagicTriggerAreaのGameObject

    private HeroController heroController; // HeroControllerの参照を保存

    private float cooldownTimer = 0f; //クールダウンタイマー
    // クールダウン状態を確認するプロパティ
    public bool IsOnCooldown => cooldownTimer < magicCooldown;
    public float CooldownProgress => Mathf.Clamp01(cooldownTimer / magicCooldown);
    public float RemainingCooldown => Mathf.Max(0f, magicCooldown - cooldownTimer);

    void Start()
    {
        heroController = hero.GetComponent<HeroController>();
        cooldownTimer = magicCooldown; // 初期状態ではクールダウン中でない
    }
    void Update()
    {
        // クールダウンタイマーを更新
        if (cooldownTimer < magicCooldown)
        {
            cooldownTimer += Time.deltaTime;
        }
    }
    
    public void OnClick()
    {
        print("Magic button clicked!");
        
        // クールダウン中の場合は処理を停止
        if (IsOnCooldown)
        {
            print($"Magic is on cooldown! Remaining: {RemainingCooldown:F1}s");
            return;
        }
        
        // コストが足りない場合は処理を停止
        if (heroController.hStatus.cost < magicCost)
        {
            print($"Not enough cost! Required: {magicCost}, Current: {heroController.hStatus.cost}");
            return;
        }
        
        // 攻撃実行
        cooldownTimer = 0f; // クールダウンをリセット
        OnClickMagic();
        heroController.hStatus.cost -= magicCost;
        print($"Magic executed! Cooldown started ({magicCooldown}s)");
    }
    void OnClickMagic()
    {
        print("Magic attack executed!");
        MagicTriggerAreaController magicTriggerAreaController = magicTriggerArea.GetComponent<MagicTriggerAreaController>();
        List<GameObject> enemiesInArea = magicTriggerAreaController.MagicAreaEnemies;
        //magicArea内の敵のEnemyController取得
        List<EnemyController> enemyControllers = new List<EnemyController>();
        foreach (GameObject enemy in enemiesInArea)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyControllers.Add(enemyController);
        }
        foreach (EnemyController enemyController in enemyControllers)
        {
            print("Enemy's HP: " + enemyController.eStatus.hp);
            enemyController.OnDamege(magicDamage);
            print("Hero attacks Enemy !!!!! Enemy's HP: " + enemyController.eStatus.hp);
        }
    }
}
