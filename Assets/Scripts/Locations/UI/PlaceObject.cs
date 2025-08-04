using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceObject : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer background;

    [SerializeField]
    private TextMeshPro placeNameText;

    [SerializeField]
    private MeshRenderer placeNameMeshRenderer;

    private void Start()
    {
        placeNameMeshRenderer.sortingOrder = 6;
    }

    public void SetText(string placeName)
    {

        placeNameText.text = placeName;
        AdjustBackgroundSize();
    }

    private void AdjustBackgroundSize()
    {
        float preferredWidth = placeNameText.preferredWidth;
        float preferredHeight = placeNameText.preferredHeight;

        background.size = new Vector2(preferredWidth * 3.5f + 0.3f, preferredHeight * 3.5f + 0.3f);
    }
}