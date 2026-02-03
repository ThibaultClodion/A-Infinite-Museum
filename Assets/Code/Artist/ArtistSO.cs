using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtistSO", menuName = "Scriptable Objects/ArtistSO")]
public class ArtistSO : ScriptableObject
{
    [Header("Artist Data")]
    public TextAsset Json;
    public List<Sprite> Paintings;

    [HideInInspector] public ArtistModel ArtistModel;

    private void OnEnable()
    {
        ArtistModel = JsonSerializer.Deserialize<ArtistModel>(Json.text);
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
