using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(ArtistGenerator))]
public class ArtistGeneratorEditor : Editor
{
    public const string c_apiKeyPrefKey = "Gemini_ApiKey";
    private string _apiKey = "";

    private static bool s_isRunning = false;
    private static int s_artistCount = 0;

    private void OnEnable()
    {
        _apiKey = EditorPrefs.GetString(c_apiKeyPrefKey, "");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtistGenerator script = (ArtistGenerator)target;

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Gemini API Configuration", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUI.BeginChangeCheck();
        _apiKey = EditorGUILayout.TextField(new GUIContent("API Key", "Your Google Gemini API key"), _apiKey);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(c_apiKeyPrefKey, _apiKey);
        }

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            EditorGUILayout.HelpBox("Please set your Gemini API Key to generate artists.", MessageType.Warning);
        }

        EditorGUILayout.Space(10);

        // Display current status
        if (s_isRunning)
        {
            EditorGUILayout.HelpBox($"Currently generating artists... ({s_artistCount} created)", MessageType.Info);
        }

        using (new EditorGUI.DisabledScope(s_isRunning || string.IsNullOrWhiteSpace(_apiKey)))
        {
            if (GUILayout.Button("Generate An Artist", GUILayout.Height(40)))
            {
                GenerateArtistAsync(script);
            }

            if (GUILayout.Button("Generate Artists In Loop", GUILayout.Height(40)))
            {
                GenerateArtistInLoop(script);
            }
        }

        using (new EditorGUI.DisabledScope(!s_isRunning))
        {
            if (GUILayout.Button("Stop Generating Artists", GUILayout.Height(40)))
            {
                s_isRunning = false;
                Debug.Log($"Stop requested. Total artists generated: {s_artistCount}");
            }
        }

        // Force repaint while running to update UI
        if (s_isRunning)
        {
            Repaint();
        }
    }

    private async void GenerateArtistInLoop(ArtistGenerator script)
    {
        if (s_isRunning)
        {
            Debug.LogWarning("Artist generation is already running.");
            return;
        }

        Debug.Log("Start to create artists in loop");

        s_isRunning = true;
        s_artistCount = 0;

        while (s_isRunning)
        {
            try
            {
                await script.GenerateAnArtist();
                s_artistCount++;
                Debug.Log($"Artist {s_artistCount} generated. Press 'Stop Generating Artists' to stop.");

                // Force UI update
                Repaint();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error generating artist: {ex.Message}");
                s_isRunning = false;
                break;
            }
        }

        Debug.Log($"Generation stopped. Total artists created: {s_artistCount}");
        s_isRunning = false;
        Repaint();
    }

    private async void GenerateArtistAsync(ArtistGenerator script)
    {
        Debug.Log("Start artist creation (the process can take up to 30 seconds)");

        try
        {
            await script.GenerateAnArtist();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error generating artist: {ex.Message}");
        }
    }
}
#endif
