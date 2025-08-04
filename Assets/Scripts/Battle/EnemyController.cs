using UnityEngine;
using UnityEngine.UI;

public struct EnemyStatus
{
    public int hp;
    public int MaxHP; // 最大HPを追加
    public int attack;
    public int defense;
    public float attackSpeed;
    public float transferSpeed;
    public bool isDead;
    public bool isEncountered;
}
public class EnemyController : MonoBehaviour
{
    Rigidbody2D rd;
    private GameObject hero;
    private GameObject magicTriggerArea;

    public EnemyStatus eStatus;
    public Slider hpSlider; // HPスライダー
    private float timer = 0f;//攻撃タイマー
    [Header("Enemy Settings")]
    [SerializeField] private int maxHP = 50; // 最大HP
    [SerializeField] private int attack = 10; // 攻撃力
    [SerializeField] private int defense = 0; // 防御力
    [SerializeField] private float attackSpeed = 5f; // 攻撃速度
    [SerializeField] private float transferSpeed = 0.025f; // 横移動速度
    [SerializeField] private const float attackSpeedMax = 10f; //攻撃間隔の最大値

    void Start()
    {
        rd = gameObject.GetComponent<Rigidbody2D>();

        hero = GameObject.Find("Hero");
        magicTriggerArea = GameObject.Find("MagicTriggerArea");
        //エネミーの初期ステータス
        eStatus.hp = maxHP; // 初期HPを設定
        eStatus.MaxHP = maxHP; // 最大HPを設定
        eStatus.attack = attack;
        eStatus.defense = defense;
        eStatus.attackSpeed = attackSpeed;
        eStatus.transferSpeed = transferSpeed;
        eStatus.isDead = false;

        // HPスライダーの初期設定
        if (hpSlider != null)
        {
            hpSlider.maxValue = eStatus.MaxHP;
            hpSlider.value = eStatus.hp;
        }
        eStatus.isEncountered = false;
    }
    void Update()
    {
        // print("Update");
        if (eStatus.isEncountered)
        {
            // print("Encountered");
            //エネミーの生存判定
            if (eStatus.hp > 0)//生存
            {
                //攻撃処理
                timer += Time.deltaTime;
                float waitTime = attackSpeedMax / eStatus.attackSpeed;
                if (timer >= waitTime)
                {
                    Attack();
                    print("timer: " + timer);
                    timer -= waitTime;
                }
            }
        }
    }
    void FixedUpdate()
    {
        //エネミーの横移動系
        if (!eStatus.isEncountered)
        {
            // print("move");
            transform.position = new Vector2(transform.position.x - eStatus.transferSpeed, transform.position.y);
            //rd.AddForce(new Vector2(eStatus.transferSpeed, 0));
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        // print("OnCollisionEnter");
        if (other.gameObject.tag == "HeroTriggerArea")
        {
            eStatus.isEncountered = true;
            print("Enemy encountered Hero!");
        }
    }
    void Attack()
    {
        HeroController heroController = hero.GetComponent<HeroController>();

        print("Hero's HP: " + heroController.hStatus.hp);
        heroController.OnDamage(eStatus.attack);
        print("Enemy attacks Hero! Hero's HP: " + heroController.hStatus.hp);
    }
    public bool OnDamage(int damage)
    {
        eStatus.hp -= Mathf.Max(1, damage - eStatus.defense); // 防御力を考慮してダメージを計算
        hpSlider.value = eStatus.hp; // HPスライダーの更新
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255); // ヒットアニメーションを再生
        Invoke("back", 0.2f); // 0.2秒後に元の色に戻す
        if (eStatus.hp <= 0)
        {
            eStatus.isDead = true;
            
            Invoke("DestroyEnemy", 0.1f); // 0.1秒後に敵を削除
            //敵のGameObjectをHeroControllerから削除
            HeroController heroController = hero.GetComponent<HeroController>();
            MagicTriggerAreaController magicTriggerAreaController = magicTriggerArea.GetComponent<MagicTriggerAreaController>();
            if (heroController.Enemies.Contains(gameObject))
            {
                heroController.Enemies.Remove(gameObject);
            }
            if (magicTriggerAreaController.MagicAreaEnemies.Contains(gameObject))
            {
                magicTriggerAreaController.MagicAreaEnemies.Remove(gameObject);
            }
            GameObject.Find("ScneneDirector").GetComponent<EnemyGenerator>().OnEnemyDestroyed();
            print("Enemy is dead!");
            
            return true;
        }

        return false;
    }

    void DestroyEnemy()
    {
        Destroy(gameObject); // 敵のGameObjectを削除
    }

    void back()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255); // 元の色に戻す
    }

}
