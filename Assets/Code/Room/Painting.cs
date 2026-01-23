using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Painting : MonoBehaviour
{
    [Header("Painting")]
    [SerializeField] private Image paintingImage;
    [SerializeField] private Renderer frameRenderer;

    [Header("Informations")]
    [SerializeField] private Renderer informationFrameRenderer;
    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField] private TextMeshProUGUI artistNameText;

    public void ChangePainting(Sprite sprite, Color frameColor)
    {
        paintingImage.sprite = sprite;
        frameRenderer.material.color = frameColor;

        AdaptSize(sprite);
    }

    private void AdaptSize(Sprite sprite)
    {
        transform.localScale = new Vector3(1, sprite.rect.size.y / 1000, sprite.rect.size.x / 1000);
    }

    public void ChangeInformation(Color frameColor, string promptUsed, string artistName)
    {
        informationFrameRenderer.material.color = frameColor;
        informationText.text = promptUsed;
        artistNameText.text = artistName;
    }
}
