using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using EventLibrary;
using System.Collections.Generic;

public class ResultPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject resultArea;

    [SerializeField]
    private GameObject resultPanel;

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private Image resultImage;

    [SerializeField]
    private TextMeshProUGUI resultCountText;

    [SerializeField]
    private TextMeshProUGUI resultDescriptionText;

    public void Setup(Sprite resultSprite, int count, string description, System.Action onConfirm)
    {
        // 登録されているリスナーをクリア
        confirmButton.onClick.RemoveAllListeners();

        resultImage.sprite = resultSprite;
        resultCountText.text = count.ToString();
        resultDescriptionText.text = description;

        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Hide();
        });
    }

    public void Show()
    {
        resultPanel.transform.localScale = Vector3.zero;
        resultArea.SetActive(true);
        resultPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);
        resultPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                resultArea.SetActive(false);
            });
    }
}