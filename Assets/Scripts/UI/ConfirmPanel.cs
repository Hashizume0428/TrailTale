using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject confirmArea;

    [SerializeField]
    private GameObject confirmPanel;

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private Button cancelButton;

    public void Setup(System.Action onConfirm)
    {
        confirmButton.onClick.AddListener(() => {
            onConfirm?.Invoke();
            confirmArea.SetActive(false);
        });
        cancelButton.onClick.AddListener(() => {
            Hide();
        });
    }

    public void Show()
    {
        confirmPanel.transform.localScale = Vector3.zero;
        confirmArea.SetActive(true);
        confirmPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        confirmPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => {
                confirmArea.SetActive(false);
            });
    }
}