using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Painting : MonoBehaviour
{
    [Header("Painting")]
    [SerializeField] private Image _paintingImage;
    [SerializeField] private Renderer _frameRenderer;

    [Header("Informations")]
    [HideInInspector] public string PromptUsed;
    [SerializeField] private Renderer _informationFrameRenderer;
    [SerializeField] private TextMeshProUGUI _informationText;
    [SerializeField] private TextMeshProUGUI _artistNameText;

    public void ChangePainting(Sprite sprite, Color frameColor)
    {
        _paintingImage.sprite = sprite;
        _frameRenderer.material.color = frameColor;

        AdaptSize(sprite);
    }

    private void AdaptSize(Sprite sprite)
    {
        transform.localScale = new Vector3(1, sprite.rect.size.y / 1000, sprite.rect.size.x / 1000);
    }

    public void ChangeInformation(Color frameColor, string promptUsed, string artistName)
    {
        PromptUsed = promptUsed;

        _informationFrameRenderer.material.color = frameColor;
        _informationText.text = promptUsed;
        _artistNameText.text = artistName;
    }
}
