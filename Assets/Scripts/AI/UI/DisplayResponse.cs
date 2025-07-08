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
    private TextMeshProUGUI resultText;

    [SerializeField]
    private OptionUI optionUIPrefab;

    private OptionUI[] optionUIs;

    private ResponseContent responseContent;

    public void DisplayResponseContent(ResponseContent responseContent)
    {
        this.responseContent = responseContent;
        stageText.text = responseContent.stage;
        descriptionText.text = responseContent.description;

        optionUIs = new OptionUI[responseContent.options.Length];
        for (int i = 0; i < optionUIs.Length && i < responseContent.options.Length; i++)
        {
            optionUIs[i] = Instantiate(optionUIPrefab, optionArea);
            optionUIs[i].SetText(responseContent.options[i].title);
            optionUIs[i].SetOnClickListener(DisplayResult, i);
        }
    }

    public void DisplayResult(int index)
    {
        foreach (var optionUI in optionUIs)
        {
            optionUI.SetState(false);
        }
        resultText.text = responseContent.options[index].result;
    }
}
