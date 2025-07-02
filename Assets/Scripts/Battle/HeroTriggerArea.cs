using UnityEngine;

public class HeroTriggerArea : MonoBehaviour
{
    [SerializeField] private GameObject hero; // 主人公のGameObject
    void Start()
    {

    }
    void OnCollisionEnter2D(Collision2D other)
    { 
        if (other.gameObject.tag == "Enemy")
        {
            hero.GetComponent<HeroController>().hStatus.isEncountered = true;
            print("Hero encountered Enemy!");
        }
    }
}
