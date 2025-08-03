using UnityEngine;
using TMPro;

public class CostTextController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] private GameObject hero;
    HeroController heroController;

    void Start()
    {
        heroController = hero.GetComponent<HeroController>();
    }

    void Update()
    {
        costText.text = "Cost: " + heroController.hStatus.cost.ToString();
    }
}
