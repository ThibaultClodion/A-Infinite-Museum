using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArtistGenerator))]
public class ArtistGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtistGenerator script = (ArtistGenerator)target;

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Generate An Artist", GUILayout.Height(40)))
        {
            GenerateArtistAsync(script);
        }
    }

    private async void GenerateArtistAsync(ArtistGenerator script)
    {
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