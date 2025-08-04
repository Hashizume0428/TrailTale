using UnityEngine;
using System.Collections.Generic;

public class MagicButonContoroller : MonoBehaviour
{
    [Header("Magic Button Settings")]
    [SerializeField] int magicCost = 7;
    [SerializeField] int magicDamageMultiple = 5;
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
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);
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
        SoundManager.Instance.PlaySE(SESoundData.SE.Magic);
        // 1. magicTriggerAreaオブジェクト自体のnullチェック
        if (magicTriggerArea == null)
        {
            print("MagicTriggerArea GameObject is null!");
            return;
        }
        
        // 2. MagicTriggerAreaコンポーネントのnullチェック
        MagicTriggerAreaController magicTriggerAreaComponent = magicTriggerArea.GetComponent<MagicTriggerAreaController>();
        if (magicTriggerAreaComponent == null)
        {
            print("MagicTriggerArea component not found!");
            return;
        }
        
        // 3. MagicAreaEnemiesリスト自体のnullチェック
        List<GameObject> magicTriggerAreaEnemies = magicTriggerAreaComponent.MagicAreaEnemies;

        heroController.Attack(magicTriggerAreaEnemies, heroController.hStatus.attack * magicDamageMultiple);
    }
}
