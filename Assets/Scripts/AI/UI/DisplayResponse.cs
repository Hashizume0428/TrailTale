using UnityEngine;
using TMPro;
using AILibrary;

/// <summary>
/// AIからのレスポンスをUIに表示するクラスです。
/// </summary>
public class DisplayResponse : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stageText;

    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [SerializeField]
    private Transform optionArea;

    [SerializeField]
    private OptionUI optionUIPrefab;

    private OptionUI[] optionUIs;

    public void DisplayResponseContent(ResponseContent responseContent)
    {
        stageText.text = responseContent.stage;
        descriptionText.text = responseContent.description;

        optionUIs = new OptionUI[responseContent.options.Length];
        for (int i = 0; i < optionUIs.Length && i < responseContent.options.Length; i++)
        {
            optionUIs[i] = Instantiate(optionUIPrefab, optionArea);
            optionUIs[i].SetText(responseContent.options[i].title);
        }
    }
}
