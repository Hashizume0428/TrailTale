using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    private Image lineImage;

    private RectTransform rectTransform;

    private void Awake()
    {
        lineImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetPosition(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = start + direction / 2;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}