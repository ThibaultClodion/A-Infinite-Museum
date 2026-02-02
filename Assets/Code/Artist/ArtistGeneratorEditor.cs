using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArtistGenerator))]
public class ArtistGeneratorEditor : Editor
{
    private static bool isRunning = false;
    private static int artistCount = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtistGenerator script = (ArtistGenerator)target;

        EditorGUILayout.Space(10);

        // Display current status
        if (isRunning)
        {
            EditorGUILayout.HelpBox($"Currently generating artists... ({artistCount} created)", MessageType.Info);
        }

        using (new EditorGUI.DisabledScope(isRunning))
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

        using (new EditorGUI.DisabledScope(!isRunning))
        {
            if (GUILayout.Button("Stop Generating Artists", GUILayout.Height(40)))
            {
                isRunning = false;
                Debug.Log($"Stop requested. Total artists generated: {artistCount}");
            }
        }

        // Force repaint while running to update UI
        if (isRunning)
        {
            Repaint();
        }
    }

    private async void GenerateArtistInLoop(ArtistGenerator script)
    {
        if (isRunning)
        {
            Debug.LogWarning("Artist generation is already running.");
            return;
        }

        Debug.Log("Start to create artists in loop");

        isRunning = true;
        artistCount = 0;

        while (isRunning)
        {
            try
            {
                await script.GenerateAnArtist();
                artistCount++;
                Debug.Log($"Artist {artistCount} generated. Press 'Stop Generating Artists' to stop.");

                // Force UI update
                Repaint();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error generating artist: {ex.Message}");
                isRunning = false;
                break;
            }
        }

        Debug.Log($"Generation stopped. Total artists created: {artistCount}");
        isRunning = false;
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