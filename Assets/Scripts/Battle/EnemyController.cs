using UnityEngine;

public struct Status
{
    public int hp;
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
    [SerializeField] GameObject hero;

    public Status eStatus;

    void Start()
    {
        //エネミーの初期ステータス
        eStatus.hp = 50;
        eStatus.attack = 10;
        eStatus.defense = 0;
        eStatus.attackSpeed = 1f;
        eStatus.transferSpeed = 1f;
        eStatus.isDead = false;
        eStatus.isEncountered = false;
    }

    void Update()
    {
        if (eStatus.isEncountered)
        {
            //エネミーの生存判定
            if (eStatus.hp > 0)//生存
            {
                //攻撃処理
                //Time.deltatimeを使って実装する
                hero.GetComponent<HeroController>().hStatus.hp -= (eStatus.attack * (100 - hero.GetComponent<HeroController>().hStatus.defense) / 100);
                print("Enemy attacks Hero! Hero's HP: " + hero.GetComponent<HeroController>().hStatus.hp);
            }
            //エネミーの生存判定
            else
            {
                eStatus.isDead = true;
                Destroy(gameObject);
            }
        }
    }
    void FixedUpdate()
    {
        //エネミーの横移動系
        if (!eStatus.isEncountered)
        {
            rd = gameObject.GetComponent<Rigidbody2D>();
            rd.AddForce(new Vector2(eStatus.transferSpeed, 0));
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "HeroTriggerArea")
        {
            eStatus.isEncountered = true;
        }
    }

}
