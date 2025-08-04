using UnityEngine;
using UnityEngine.UI;

public class MpVarController : MonoBehaviour
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
        slider.maxValue = heroController.hStatus.maxCost;
        slider.value = heroController.hStatus.cost;
    }
}
