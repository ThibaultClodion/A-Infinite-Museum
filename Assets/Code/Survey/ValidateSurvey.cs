using Oculus.Interaction.Samples;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class ValidateSurvey : MonoBehaviour
{
    [Header("Survey")]
    [SerializeField] private Survey _survey;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI _validationText;
    [SerializeField] private List<Toggle> _disableTooglesAfterSubmit;
    [SerializeField] private Animator _toogleAnimator;
    [SerializeField] private AnimatorController _validateController;
    [SerializeField] private AnimatorOverrideLayerWeigth _animatorOverrideLayerWeigth;

    public void SubmitSurvey()
    {
        if (_survey.SubmitSurvey())
        {
            _validationText.text = "Thank you for answering all questions !";

            foreach (Toggle toggle in _disableTooglesAfterSubmit)
            {
                toggle.interactable = false;
            }

            _toogleAnimator.tag = "Validated";
            _toogleAnimator.runtimeAnimatorController = _validateController;

            // Reset the override layer weight to trigger the transition to the new state in the animator
            _animatorOverrideLayerWeigth.enabled = false;
            _animatorOverrideLayerWeigth.enabled = true;
        }
        else
        {
            _validationText.text = "Please answer all questions before submitting the survey.";
        }
    }
}
