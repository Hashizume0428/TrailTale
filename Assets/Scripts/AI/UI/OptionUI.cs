using UnityEngine;
using TMPro;

public class OptionUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI optionText;

    public void SetText(string text)
    {
        optionText.text = text;
    }
}