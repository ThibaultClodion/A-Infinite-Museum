using Oculus.Interaction.Samples;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSelectedButton : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private AnimatorController _greenAnimatorController;
    [SerializeField] private AnimatorController _defaultAnimatorController;

    [Header("UI Elements")]
    [SerializeField] private List<Animator> _buttonsAnimator;
    [SerializeField] private Animator _toogleAnimator;
    [SerializeField] private AnimatorController _validateController;
    [SerializeField] private AnimatorOverrideLayerWeigth _animatorOverrideLayerWeigth;

    public void HighlightSelected(Animator selectedAnimator)
    {
        foreach (Animator animator in _buttonsAnimator)
        {
            if (animator == selectedAnimator)
            {
                animator.runtimeAnimatorController = _greenAnimatorController;
            }
            else
            {
                animator.runtimeAnimatorController = _defaultAnimatorController;
            }
        }

        _toogleAnimator.tag = "Validated";
        _toogleAnimator.runtimeAnimatorController = _validateController;

        // Reset the override layer weight to trigger the transition to the new state in the animator
        _animatorOverrideLayerWeigth.enabled = false;
        _animatorOverrideLayerWeigth.enabled = true;
    }
}
