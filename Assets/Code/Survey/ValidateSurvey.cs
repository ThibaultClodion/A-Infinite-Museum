using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValidateSurvey : MonoBehaviour
{
    [Header("Survey")]
    [SerializeField] private Survey _survey;

    [Header("Survey Icon")]
    [SerializeField] private Image _surveyIcon;
    [SerializeField] private Color _surveyValidated;
    [SerializeField] private Color _surveyIncomplete;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI _validationText;
    [SerializeField] private List<Toggle> _disableTooglesAfterSubmit;

    public void SubmitSurvey()
    {
        if (_survey.SubmitSurvey())
        {
            _surveyIcon.color = _surveyValidated;
            _validationText.text = "Thank you for answering all questions !";

            foreach (Toggle toggle in _disableTooglesAfterSubmit)
            {
                toggle.interactable = false;
            }
        }
        else
        {
            _surveyIcon.color = _surveyIncomplete;
            _validationText.text = "Please answer all questions before submitting the survey.";
        }
    }
}
