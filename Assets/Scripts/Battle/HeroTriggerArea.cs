using UnityEngine;
using System.Collections.Generic;

public class HeroTriggerArea : MonoBehaviour
{
    [SerializeField] private GameObject hero; // 主人公のGameObject
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //衝突判定係数を１にする
            hero.GetComponent<HeroController>().hStatus.isEncountered = true;
            print("Hero encountered Enemy!");
            //衝突した敵をEnemiesに登録する
            HeroController heroController = hero.GetComponent<HeroController>();
            heroController.Enemies.Add(other.gameObject);
        }
    }
}
