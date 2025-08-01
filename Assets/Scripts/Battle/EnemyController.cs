using UnityEngine;

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
    private float timer = 0f;//攻撃タイマー

    [SerializeField] private const float attackSpeedMax = 10f; //攻撃間隔の最大値

    void Start()
    {
        rd = gameObject.GetComponent<Rigidbody2D>();

        hero = GameObject.Find("Hero");
        magicTriggerArea = GameObject.Find("MagicTriggerArea");
        //エネミーの初期ステータス
        eStatus.hp = 50;
        eStatus.attack = 10;
        eStatus.defense = 0;
        eStatus.attackSpeed = 5f;
        eStatus.transferSpeed = 0.025f;
        eStatus.isDead = false;
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
        heroController.OnDamege(eStatus.attack);
        print("Enemy attacks Hero! Hero's HP: " + heroController.hStatus.hp);
    }
    public void OnDamege(int damage)
    {
        eStatus.hp -= damage * (100 - eStatus.defense) / 100;
        if (eStatus.hp <= 0)
        {
            eStatus.isDead = true;
            Destroy(gameObject);
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
        }
    }

}
