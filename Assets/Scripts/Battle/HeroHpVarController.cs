using UnityEngine;
using UnityEngine.UI;

public class HeroHpVarController : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private GameObject hero;
    HeroController heroController;
    void Start()
    {
        heroController = hero.GetComponent<HeroController>();
    }

    void Update()
    {
        slider.maxValue = heroController.hStatus.maxHP;
        slider.value = heroController.hStatus.hp;
    }
}
