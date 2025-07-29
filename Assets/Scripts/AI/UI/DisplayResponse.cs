using UnityEngine;
using TMPro;
using AILibrary;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

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
    private Button nextButton;

    [SerializeField]
    private OptionUI optionUIPrefab;

    private OptionUI[] optionUIs;

    private ResponseContent responseContent;
    public ResponseContent ResponseContent => responseContent;
    
    private int selectedOptionIndex = -1;
    public int SelectedOptionIndex => selectedOptionIndex;

    private void Start()
    {
        nextButton.interactable = false; // 初期状態では次へボタンを無効にする
    }

    public void RegisterNextButtonListener(Action listener)
    {
        nextButton.onClick.AddListener(() => listener());
    }

    /// <summary>
    /// AIからのレスポンス内容をUIに表示します。
    /// </summary>
    /// <param name="responseContent"></param>
    public void DisplayResponseContent(ResponseContent responseContent)
    {
        // 既存のオプションUIを削除
        if (optionUIs != null)
        {
            foreach (var optionUI in optionUIs)
            {
                Destroy(optionUI.gameObject);
            }
        }

        this.responseContent = responseContent;
        stageText.text = responseContent.stage;
        descriptionText.text = responseContent.description;
        resultText.text = string.Empty; // 結果テキストを初期化

        optionUIs = new OptionUI[responseContent.options.Length];
        for (int i = 0; i < optionUIs.Length && i < responseContent.options.Length; i++)
        {
            optionUIs[i] = Instantiate(optionUIPrefab, optionArea);
            optionUIs[i].SetText(responseContent.options[i].title);
            optionUIs[i].SetOnClickListener(DisplayResult, i);
        }
    }

    /// <summary>
    /// 選択されたオプションの結果を表示します。
    /// </summary>
    /// <param name="index"></param>
    public void DisplayResult(int index)
    {
        selectedOptionIndex = index;
        for (int i = 0; i < optionUIs.Length; i++)
        {
            OptionUI optionUI = optionUIs[i];
            optionUI.SetState(false); // すべてのオプションを非アクティブにする

            // 選択されたオプションのみ選択状態にする
            if (i == index)
            {
                optionUI.SetSelected(true);
            }
            else
            {
                optionUI.SetSelected(false);
            }
        }

        resultText.text = responseContent.options[index].result;
        nextButton.interactable = true; // 次へボタンを有効にする
    }
}
