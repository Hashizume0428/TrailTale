using UnityEngine;
using TMPro;

public class HeroHpTextTextController : MonoBehaviour
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
        costText.text = "HP: " + heroController.hStatus.hp.ToString();
    }
}
