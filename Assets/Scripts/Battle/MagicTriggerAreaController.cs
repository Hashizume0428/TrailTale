using UnityEngine;
using System.Collections.Generic;

public class MagicTriggerAreaController : MonoBehaviour
{
    public List<GameObject> MagicAreaEnemies = new List<GameObject>(); //MagicArea内の敵のGameObjectリスト

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //衝突した敵をMagicAreaEnemiesに登録する
            if (!MagicAreaEnemies.Contains(other.gameObject))
            {
                MagicAreaEnemies.Add(other.gameObject);
                print("<color=cyan>Enemy entered magic area: " + other.gameObject.name + "</color>");
            }
        }
    }
}
