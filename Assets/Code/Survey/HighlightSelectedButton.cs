using Oculus.Interaction.Samples;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSelectedButton : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;

    [Header("UI Elements")]
    [SerializeField] private List<Image> _buttonsImage;
    [SerializeField] private Animator _toogleAnimator;
    [SerializeField] private AnimatorController _validateController;
    [SerializeField] private AnimatorOverrideLayerWeigth _animatorOverrideLayerWeigth;

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

        _toogleAnimator.tag = "Validated";
        _toogleAnimator.runtimeAnimatorController = _validateController;

        // Reset the override layer weight to trigger the transition to the new state in the animator
        _animatorOverrideLayerWeigth.enabled = false;
        _animatorOverrideLayerWeigth.enabled = true;
    }
}
