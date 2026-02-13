using UnityEngine;
using System.IO;

public class Survey : MonoBehaviour
{
    private const string c_surveyFolder = "Assets/Data/Surveys/";
    private const string c_surveyFileName = "SurveyData.csv";
    private const string c_surveyColumnSeparator = ",";
    private const string c_headerLine = "Artist Name" + c_surveyColumnSeparator +
        "Which word best describes this room ?" + c_surveyColumnSeparator +
        "Did the artist seem to have a clear creative intent ?" + c_surveyColumnSeparator +
        "Did you notice a consistent style across all six paintings ?" + c_surveyColumnSeparator +
        "Did the artist seem to own the work and explain it convincingly ?" + c_surveyColumnSeparator +
        "How would you rate this artist overall ?";

    private string _actualSurveyLine = string.Empty;
    private string _wordToDescribeRoom = string.Empty;
    private string[] _likertResponses = new string[4];

    public void InitializeSurvey(ArtistSO artist)
    {
        CreateFileIfNecessary();

        for (int i = 0; i < _likertResponses.Length; i++)
        {
            _likertResponses[i] = string.Empty;
        }

        _actualSurveyLine = artist.ArtistModel.Name + c_surveyColumnSeparator;
    }

    public void ChangeWordToDescribeRoom(string newWord)
    {
        _wordToDescribeRoom = newWord;
    }

    private void ChangeLikertResponse(int index, string newValue)
    {
        _likertResponses[index] = newValue;
    }

    public void ChangeLikertResponse0(string newValue) => ChangeLikertResponse(0, newValue);
    public void ChangeLikertResponse1(string newValue) => ChangeLikertResponse(1, newValue);
    public void ChangeLikertResponse2(string newValue) => ChangeLikertResponse(2, newValue);
    public void ChangeLikertResponse3(string newValue) => ChangeLikertResponse(3, newValue);

    public bool SubmitSurvey()
    {
        if(string.IsNullOrEmpty(_wordToDescribeRoom))
        {
            return false;
        }
        foreach (string response in _likertResponses)
        {
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }
        }

        AddLineToSurvey();

        return true;
    }

    private void AddLineToSurvey()
    {
#if UNITY_EDITOR
        // This code will only run in the Unity Editor, ensuring that we don't attempt to write files in a build where it might not be allowed or could cause issues.
        _actualSurveyLine += _wordToDescribeRoom + c_surveyColumnSeparator;

        foreach (string response in _likertResponses)
        {
            _actualSurveyLine += response + c_surveyColumnSeparator;
        }

        // Remove the last separator
        _actualSurveyLine = _actualSurveyLine.TrimEnd(c_surveyColumnSeparator.ToCharArray());

        // Add the line to the file
        File.AppendAllText(c_surveyFolder + c_surveyFileName, _actualSurveyLine + "\n");
#endif
    }

    private void CreateFileIfNecessary()
    {
#if UNITY_EDITOR
        // This code will only run in the Unity Editor, ensuring that we don't attempt to write files in a build where it might not be allowed or could cause issues.

        // Ensure the directory exists
        if (!Directory.Exists(c_surveyFolder))
        {
            Directory.CreateDirectory(c_surveyFolder);
        }

        string filePath = c_surveyFolder + c_surveyFileName;

        // Create file with header if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, c_headerLine + "\n");
        }
#endif
    }
}
