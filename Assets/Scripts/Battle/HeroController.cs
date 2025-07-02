using UnityEngine;

public class HeroController : MonoBehaviour
{
    public Status hStatus;
    private GameObject enemy; //敵のGameObject
    private const float attackSpeedMax = 10f; //攻撃間隔の最大値
    private float timer = 0f; //攻撃タイマー

    void Start()
    {
        hStatus.hp = 100;
        hStatus.attack = 10;
        hStatus.defense = 0;
        hStatus.attackSpeed = 1f;
        hStatus.isDead = false;
        hStatus.isEncountered = false;
    }
    
    void Update()
    {
        if (hStatus.isEncountered)
        {
            //主人公の生存判定
            if (hStatus.hp > 0)//生存
            {
                //攻撃処理
                timer += Time.deltaTime;
                float waitTime = attackSpeedMax / hStatus.attackSpeed;
                if (timer >= waitTime)
                {
                    Attack();
                    print("timer: " + timer);
                    timer -= waitTime;
                }
            }
        }
    }
    // 主人公の衝突判定はHeroTriggerArea.csで行う
    void Attack()
    {
        //attackArea内の敵を取得
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        print("Enemy's HP: " + enemyController.eStatus.hp);
        enemyController.OnDamege(hStatus.attack);
        print("Hero attacks Enemy! Enemy's HP: " + enemyController.eStatus.hp);
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
}
