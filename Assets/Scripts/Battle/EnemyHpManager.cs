using UnityEngine;
using UnityEngine.UI;

public class EnemyHpManager : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private GameObject enemy; // 敵のGameObjectをInspectorから設定
    private EnemyController enemyController;
    void Start()
    {
        enemyController = enemy.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = enemyController.eStatus.MaxHP;
        slider.value = enemyController.eStatus.hp;
    }
}
