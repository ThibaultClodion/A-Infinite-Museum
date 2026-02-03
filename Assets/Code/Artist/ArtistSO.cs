using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtistSO", menuName = "Scriptable Objects/ArtistSO")]
public class ArtistSO : ScriptableObject
{
    [Header("Artist Data")]
    public TextAsset Json;
    public List<Sprite> Paintings = new List<Sprite>();

    [HideInInspector] public ArtistModel ArtistModel;

    public void RetrieveData()
    {
        // Clear previous data
        Json = null;
        Paintings.Clear();
        ArtistModel = null;

        // Get actual directory of the ScriptableObject
        string assetPath = AssetDatabase.GetAssetPath(this);
        string directoryPath = System.IO.Path.GetDirectoryName(assetPath);

        // List all files inside the directory
        for (int i = 0; i < AssetDatabase.FindAssets("", new[] { directoryPath }).Length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("", new[] { directoryPath })[i]);

            if (filePath.EndsWith(".json"))
            {
                Json = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);

            }
            else if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg"))
            {
                Sprite painting = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
                Paintings.Add(painting);
            }
        }

        if (Json != null)
        {
            ArtistModel = JsonSerializer.Deserialize<ArtistModel>(Json.text);

            // Mark the asset as dirty and save BEFORE renaming
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            // Change filename to artist name
            string safeName = string.Join("_", ArtistModel.Name.Split(System.IO.Path.GetInvalidFileNameChars()));
            string newAssetPath = System.IO.Path.Combine(directoryPath, safeName + ".asset");

            // Rename the asset
            AssetDatabase.RenameAsset(assetPath, safeName);
            AssetDatabase.SaveAssets();
        }
    }
}

[CustomEditor(typeof(ArtistSO))]
public class ArtistSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ArtistSO artistSO = (ArtistSO)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Retrieve data automatically"))
        {
            artistSO.RetrieveData();
            AssetDatabase.Refresh();
        }
    }
}

public class ArtistModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("meshDescription")]
    public string MeshDescription { get; set; }

    [JsonPropertyName("animationDescription")]
    public string AnimationDescription { get; set; }

    [JsonPropertyName("voiceDescription")]
    public string VoiceDescription { get; set; }

    [JsonPropertyName("wallsColor")]
    public int[] WallsColor { get; set; }

    [JsonPropertyName("floorColor")]
    public int[] FloorColor { get; set; }

    [JsonPropertyName("roofColor")]
    public int[] RoofColor { get; set; }

    [JsonPropertyName("frameColor")]
    public int[] FrameColor { get; set; }

    [JsonPropertyName("frameInformationColor")]
    public int[] FrameInformationColor { get; set; }

    [JsonPropertyName("paintingPrompts")]
    public string[] PaintingPrompts { get; set; }
}
