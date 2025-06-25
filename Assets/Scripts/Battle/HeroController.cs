using UnityEngine;

public class HeroController : MonoBehaviour
{
    public Status hStatus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hStatus.hp = 100;
        hStatus.attack = 10;
        hStatus.defense = 0;
        hStatus.attackSpeed = 1f;
        hStatus.isDead = false;
        hStatus.isEncountered = false;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
