using UnityEngine;
using System.Collections.Generic;

public class MagicTriggerArea : MonoBehaviour
{
    public List<GameObject> MagicAreaEnemies = new List<GameObject>(); //MagicArea内の敵のGameObjectリスト
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            print("<color=purple>Hero encountered Enemy in Magic Area!</color>");
            //衝突した敵をMagicAreaEnemiesに登録する
            MagicAreaEnemies.Add(other.gameObject);
        }
    }
}
