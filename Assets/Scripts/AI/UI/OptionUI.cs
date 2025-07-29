using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Button))]
public class OptionUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI optionText;

    private Image image;

    private Button button;

    public void SetText(string text)
    {
        optionText.text = text;
    }

    public void SetOnClickListener(Action<int> listener, int index)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => listener(index));
    }

    public void SetState(bool isActive)
    {
        button.interactable = isActive;
    }

    public void SetSelected(bool isSelected)
    {
        image = GetComponent<Image>();
        image.color = isSelected ? new Color(0, 0, 0, 1.0f) : new Color(0, 0, 0, 0.5f);
    }
}