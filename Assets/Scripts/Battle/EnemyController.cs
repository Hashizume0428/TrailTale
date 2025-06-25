using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameObject Enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Enemy = GetComponent<enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        Enemy.transform.position.x += 0.5f;
    }
}
