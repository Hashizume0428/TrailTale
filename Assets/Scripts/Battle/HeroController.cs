using UnityEngine;
using System.Collections.Generic;

public struct HeroStatus
{
    public int hp; // 現在のHP
    public int maxHP; // 最大HP
    public int attack; // 攻撃力
    public int defense; // 防御力
    public float attackSpeed; // 攻撃速度（通常の攻撃速度は2秒に一回,どれだけの割合短縮したいか0~1の間で）
    public float autoHealInterval; // 自動回復間隔
    public int autoHealValue; // 自動回復のスピード(1秒毎に回復するHPの値)
    public int cost; //現在のコスト
    public int maxCost; // コストの最大値
    public float autoCostInterval; // 自動コスト回復間隔
    public int autoCostValue; // 自動コスト回復の値
    public bool isDead; // 死亡フラグ
    public bool isEncountered; // 接触フラグ
}
public class HeroController : MonoBehaviour
{
    [Header("Hero Settings")]
    [SerializeField] private int maxHP = 100; // 最大HP
    [SerializeField] private int attack = 10; // 攻撃力
    [SerializeField] private int defense = 0; // 防御力
    [SerializeField] private float attackSpeed = 1f; // 攻撃速度（通常の攻撃速度は2秒に一回）
    [SerializeField] private int maxCost = 10; // コストの最大値
    [SerializeField] private float autoCostInterval = 2f; // 自動コスト回復間隔（秒）
    [SerializeField] private int autoHealValue = 2; // 自動回復のスピード(1秒毎に回復するHPの値)
    [SerializeField] private float autoHealInterval = 1f; // 自動回復の間隔
    [SerializeField] private int autoCostValue = 1; // 自動コスト回復の値

    public HeroStatus hStatus;
    [SerializeField] private GameObject enemy; //敵のGameObject
    public List<GameObject> Enemies = new List<GameObject>(); //敵のGameObjectリスト
    private const float attackSpeedMax = 2f; //攻撃間隔の最大値
    private float timer = 0f; //攻撃タイマー
    private int tempPreviousDefense; // 一時的に値を保存

    private float autoHealTimer = 0f; // 自動回復タイマー
    private float autoCostTimer = 0f; // 自動コスト回復タイマー
    void Start()
    {
        hStatus.hp = maxHP; //初期HPを設定
        hStatus.maxHP = maxHP; // 最大HPを設定
        hStatus.attack = attack;
        hStatus.defense = defense;
        hStatus.attackSpeed = attackSpeed;
        hStatus.autoHealInterval = autoHealInterval;
        hStatus.autoHealValue = autoHealValue; // 自動回復の値を設定
        hStatus.cost = maxCost; // 初期コストを設定
        hStatus.maxCost = maxCost; // 最大コストを設定
        hStatus.autoCostInterval = autoCostInterval; // 自動コスト回復間隔を設定
        hStatus.autoCostValue = autoCostValue; // 自動コスト回復の値を設定
        hStatus.isDead = false;
        hStatus.isEncountered = false;
    }

    void Update()
    {
        if (hStatus.isEncountered)
        {
            // print("encountered");
            //主人公の生存判定
            if (hStatus.hp > 0)//生存
            {
                //攻撃処理
                timer += Time.deltaTime;
                float waitTime = attackSpeedMax * (1f / hStatus.attackSpeed); // 攻撃間隔を計算
                if (timer >= waitTime)
                {
                    Attack();
                    // print("timer: " + timer);
                    timer -= waitTime;
                }
                // 自動回復処理
                autoHealTimer += Time.deltaTime;
                if (autoHealTimer >= autoHealInterval)
                {
                    AutoHeal();
                    autoHealTimer -= autoHealInterval;
                }
                
                // 自動コスト回復処理
                autoCostTimer += Time.deltaTime;
                if (autoCostTimer >= autoCostInterval)
                {
                    AutoCost();
                    autoCostTimer -= autoCostInterval;
                }
            }
        }
    }
    // 主人公の衝突判定はHeroTriggerArea.csで行う

    void Attack()
    {
        //attackArea内の敵のEnemyControllerを取得
        List<EnemyController> enemyControllers = new List<EnemyController>();
        foreach (GameObject enemy in Enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyControllers.Add(enemyController);
        }
        foreach (EnemyController enemyController in enemyControllers)
        {
            // print("Enemy's HP: " + enemyController.eStatus.hp);
            enemyController.OnDamege(hStatus.attack);
            // print("Hero attacks Enemy! Enemy's HP: " + enemyController.eStatus.hp);
        }
    }

    // 敵リストを取得するメソッド（読み取り専用）
    public List<GameObject> GetEnemies()
    {
        return new List<GameObject>(Enemies); // コピーを返して直接変更を防ぐ
    }

    // 敵を追加するメソッド
    public void AddEnemy(GameObject enemy)
    {
        if (enemy != null && !Enemies.Contains(enemy))
        {
            Enemies.Add(enemy);
        }
    }

    // 敵を削除するメソッド
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemy != null && Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
        }
    }

    public void OnDamege(int damage)
    {
        hStatus.hp -= damage * (100 - hStatus.defense) / 100;
        if (hStatus.hp <= 0)
        {
            hStatus.isDead = true;
            Destroy(gameObject);
        }
    }

    //防御力アビリティ起動時のメソッド
    // defense: 防御力の値（何％カットさせるか）
    // duration: 防御の持続時間
    public void ActivateDefense(int defense, int duration)
    {
        print($"<color=red>Defense activated! Previous defense: {hStatus.defense}%");
        tempPreviousDefense = hStatus.defense;
        hStatus.defense = (int)((1f - (1f - (hStatus.defense / 100f)) * (1f - (defense / 100f))) * 100f); // 防御力を上げる（カット率が乗算で軽減されていく）
        print($"<color=red>Defense activated! New defense: {hStatus.defense}%");
        Invoke(nameof(DeactivateDefense), duration); // 一定時間後に防御を解除
    }
    //防御力アビリティ解除のメソッド
    // tempPreviousDefense: 前の防御力の値
    private void DeactivateDefense()
    {
        print($"<color=red>Defense deactivated! Previous defense: {hStatus.defense}%");
        hStatus.defense = tempPreviousDefense; // 防御力を元に戻す
        print($"<color=red>Defense deactivated! New defense: {hStatus.defense}%");
    }

    public void Heal(int value)
    {
        if ((hStatus.hp + value) < hStatus.maxHP) // 最大HPを超えないように制限
        {
            hStatus.hp += value;
        }
        else
        {
            hStatus.hp = hStatus.maxHP; // 最大HPを超えないように制限
        }
    }

    void AutoHeal()
    {
        if (hStatus.hp < hStatus.maxHP) // HPが最大値未満の場合のみ回復
        {
            print($"<color=green>Hero auto-healed! Previous HP: {hStatus.hp}/{hStatus.maxHP}</color>");
            Heal(autoHealValue); // 自動回復の値で回復
            print($"<color=green>Hero auto-healed! Current HP: {hStatus.hp}/{hStatus.maxHP}</color>");
        }
    }
    public void AddCost(int value)
    {
        if ((hStatus.cost + value) <= hStatus.maxCost)
        {
            hStatus.cost += value;
        }
        else
        {
            hStatus.cost = hStatus.maxCost; // 最大コストを超えないように制限
        }
    }
    public void AutoCost()
    {
        if (hStatus.cost < hStatus.maxCost) // コストが最大値未満の場合のみ回復
        {
            print($"<color=green>Hero auto-costed! Previous Cost: {hStatus.cost}/{hStatus.maxCost}</color>");
            AddCost(autoCostValue); // 自動回復の値で回復
            print($"<color=green>Hero auto-costed! Current Cost: {hStatus.cost}/{hStatus.maxCost}</color>");
        }
    }
}
