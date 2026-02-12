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

    private string _actualSurveyLine;
    private string _wordToDescribeRoom;
    private int[] _likertResponses = new int[4];

    public void InitializeSurvey(ArtistSO artist)
    {
        CreateFileIfNecessary();

        _actualSurveyLine = artist.ArtistModel.Name + c_surveyColumnSeparator;
    }

    public void ChangeWordToDescribeRoom(string newWord)
    {
        _wordToDescribeRoom = newWord;
    }

    public void ChangeLikertResponse(int index, int newValue)
    {
        _likertResponses[index] = newValue;
    }

    public void AddLineToSurvey()
    {
        _actualSurveyLine += _wordToDescribeRoom + c_surveyColumnSeparator;

        foreach (int response in _likertResponses)
        {
            _actualSurveyLine += response.ToString() + c_surveyColumnSeparator;
        }

        // Remove the last separator
        _actualSurveyLine = _actualSurveyLine.TrimEnd(c_surveyColumnSeparator.ToCharArray());

        // Add the line to the file
        File.AppendAllText(c_surveyFolder + c_surveyFileName, _actualSurveyLine + "\n");
    }

    private void CreateFileIfNecessary()
    {
        // Ensure the directory exists
        if (!Directory.Exists(c_surveyFolder))
        {
            Directory.CreateDirectory(c_surveyFolder);
            Debug.Log($"Created directory: {c_surveyFolder}");
        }

        string filePath = c_surveyFolder + c_surveyFileName;

        // Create file with header if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, c_headerLine + "\n");
            Debug.Log($"Created survey file: {filePath}");
        }
    }
}
