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
    // void OnClickMagic()
    // {
    //     List<GameObject> magicTriggerAreaEnemies = new List<GameObject>(); // MagicTriggerArea内の敵のGameObjectを取得
    //     magicTriggerAreaEnemies = magicTriggerArea.GetComponent<MagicTriggerArea>().MagicAreaEnemies;
    //     if (magicTriggerAreaEnemies == null)
    //     {
    //         print("magicTriggerAreaEnemies not found!");
    //         return;
    //     }
    //     //magicArea内の敵のEnemyController取得
    //     List<EnemyController> enemyControllers = new List<EnemyController>();
    //     foreach (GameObject enemy in magicTriggerAreaEnemies)
    //     {
    //         EnemyController enemyController = enemy.GetComponent<EnemyController>();
    //         enemyControllers.Add(enemyController);
    //     }
    //     foreach (EnemyController enemyController in enemyControllers)
    //     {
    //         print("Enemy's HP: " + enemyController.eStatus.hp);
    //         enemyController.OnDamage(magicDamage);
    //         print("Hero attacks Enemy !!!!! Enemy's HP: " + enemyController.eStatus.hp);
    //     }
    // }
        void OnClickMagic()
    {
        // 1. magicTriggerAreaオブジェクト自体のnullチェック
        if (magicTriggerArea == null)
        {
            print("MagicTriggerArea GameObject is null!");
            return;
        }
        
        // 2. MagicTriggerAreaコンポーネントのnullチェック
        MagicTriggerArea magicTriggerAreaComponent = magicTriggerArea.GetComponent<MagicTriggerArea>();
        if (magicTriggerAreaComponent == null)
        {
            print("MagicTriggerArea component not found!");
            return;
        }
        
        // 3. MagicAreaEnemiesリスト自体のnullチェック
        List<GameObject> magicTriggerAreaEnemies = magicTriggerAreaComponent.MagicAreaEnemies;
        if (magicTriggerAreaEnemies == null)
        {
            print("MagicAreaEnemies list is null!");
            return;
        }
        
        // 4. リストが空の場合のチェック
        if (magicTriggerAreaEnemies.Count == 0)
        {
            print("No enemies in magic area!");
            return;
        }
        
        // 5. 各敵オブジェクトとコンポーネントの安全な処理
        List<EnemyController> enemyControllers = new List<EnemyController>();
        foreach (GameObject enemy in magicTriggerAreaEnemies)
        {
            // 各enemyオブジェクトのnullチェック
            if (enemy == null)
            {
                print("Found null enemy in list, skipping...");
                continue;
            }
            
            // EnemyControllerコンポーネントのnullチェック
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController == null)
            {
                print($"EnemyController component not found on {enemy.name}, skipping...");
                continue;
            }
            
            enemyControllers.Add(enemyController);
        }
        
        // 6. 有効な敵コントローラーが存在するかチェック
        if (enemyControllers.Count == 0)
        {
            print("No valid enemy controllers found!");
            return;
        }
        
        // 7. 安全に攻撃処理を実行
        foreach (EnemyController enemyController in enemyControllers)
        {
            // さらに安全性を高めるため、実行時にも再チェック
            if (enemyController != null && enemyController.gameObject != null)
            {
                print("Enemy's HP: " + enemyController.eStatus.hp);
                enemyController.OnDamage(magicDamage);
                print("Hero attacks Enemy !!!!! Enemy's HP: " + enemyController.eStatus.hp);
            }
        }
    }
}
