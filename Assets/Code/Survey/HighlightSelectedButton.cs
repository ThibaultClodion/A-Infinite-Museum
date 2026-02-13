using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSelectedButton : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;

    [Header("UI Elements")]
    [SerializeField] private List<Image> _buttonsImage;
    [SerializeField] private Image _questionIcon;

    public void HighlightSelected(Image selectedButton)
    {
        foreach (Image button in _buttonsImage)
        {
            if (button == selectedButton)
            {
                button.color = _selectedColor;
            }
            else
            {
                button.color = _defaultColor;
            }
        }

        _questionIcon.color = _selectedColor;
    }
}
