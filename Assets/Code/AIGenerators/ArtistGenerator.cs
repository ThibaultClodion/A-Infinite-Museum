using UnityEngine;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.IO;

public class ArtistGenerator : MonoBehaviour
{
    [Header("Generation Parameters")]
    [SerializeField] private DescriptionConversionTable descriptionConversionTable;

    private Client client = new Client(apiKey: APIKeys.GEMINI_API_KEY);

    private const string GENERATED_ARTISTS_FOLDER_NAME = "GeneratedArtists";

    public async Task GenerateAnArtist()
    {
        if(client == null)
        {
            Debug.LogError("Client is not initialized. Please check your API key.");
            return;
        }

        Debug.Log("Start artist creation (the process can take up to 30 seconds)");

        // Configure content generation parameters to get a random artist
        var config = new GenerateContentConfig();
        config.ResponseMimeType = "application/json";
        config.Seed = Random.Range(1, int.MaxValue);
        config.Temperature = 1.8f;
        config.TopP = 0.95f;

        // Prepare enum lists for mesh, animation, and voice descriptions
        List<string> meshDescriptions = descriptionConversionTable.descriptionToMesh
            .Select(pair => pair.description)
            .ToList();

        List<string> animationDescriptions = descriptionConversionTable.descriptionToAnimation
            .Select(pair => pair.description)
            .ToList();

        List<string> voiceDescriptions = descriptionConversionTable.descriptionToVoice
            .Select(pair => pair.description + " " + pair.voiceName + " " + pair.labels)
            .ToList();

        // Define the JSON schema for the response
        config.ResponseJsonSchema = new Schema()
        {
            Type = Type.OBJECT,
            Properties = new Dictionary<string, Schema>()
            {
                { "name", new Schema() 
                    { 
                        Type = Type.STRING, Description = "The name of the artist." 
                    } 
                },
                { "description", new Schema() 
                    { 
                        Type = Type.STRING, Description = "The description of the artist." 
                    } 
                },
                { "meshDescription", new Schema()
                    {
                        Type = Type.STRING,
                        Description = "A description of the 3D mesh representing the artist.",
                        Enum = meshDescriptions
                    }
                },
                { "animationDescription", new Schema()
                    {
                        Type = Type.STRING,
                        Description = "A description of the animation used by the artist.",
                        Enum = animationDescriptions
                    }
                },
                { "voiceDescription", new Schema()
                    {
                        Type = Type.STRING,
                        Description = "A description of the voice of the artist.",
                        Enum = voiceDescriptions
                    }
                },
                { "wallsColor", new Schema()
                    {
                        Type = Type.ARRAY,
                        Description = "The RGB color of the walls in the artist's studio (3 integers 0-255).",
                        Items = new Schema() { Type = Type.INTEGER }
                    }
                },
                { "floorColor", new Schema()
                    {
                        Type = Type.ARRAY,
                        Description = "The RGB color of the floor in the artist's studio (3 integers 0-255).",
                        Items = new Schema() { Type = Type.INTEGER }
                    }
                },
                { "roofColor", new Schema()
                    {
                        Type = Type.ARRAY,
                        Description = "The RGB color of the roof in the artist's studio (3 integers 0-255).",
                        Items = new Schema() { Type = Type.INTEGER }
                    }
                },
                { "frameColor", new Schema()
                    {
                        Type = Type.ARRAY,
                        Description = "The RGB color of the frames used by the artist (3 integers 0-255).",
                        Items = new Schema() { Type = Type.INTEGER }
                    }
                },
                { "frameInformationColor", new Schema()
                    {
                        Type = Type.ARRAY,
                        Description = "The RGB color of the frame above the painting displaying information (3 integers 0-255).",
                        Items = new Schema() { Type = Type.INTEGER }
                    }
                },
                { "paintingPrompts", new Schema()
                    {
                        Type = Type.ARRAY,
                        Description = "A list of 6 painting prompts that the artist have painted and exposes in his museum.",
                        Items = new Schema() { Type = Type.STRING }
                    }
                }
            },

            Required = new List<string>() { "name", "description", "meshDescription", "animationDescription", "voiceDescription", 
                "wallsColor", "floorColor", "roofColor", "frameColor", "frameInformationColor", "paintingPrompts" }
        };

        var response = await client.Models.GenerateContentAsync(
            model: "gemini-3-flash-preview",
            contents: $"Generate a fictional painter name and description. " +
            $"IMPORTANT: Choose randomly and vary your selection from these mesh descriptions: {string.Join(", ", meshDescriptions)}. Do not favor any particular option. Use it to fulfill the other parameters with creative variety.",
            config
        );

        // Parse the JSON response
        string jsonResponse = response.Candidates[0].Content.Parts[0].Text;
        ArtistModel artist = JsonSerializer.Deserialize<ArtistModel>(jsonResponse);

        // Save the artist data to a JSON file
        SaveJSONFile(jsonResponse, artist);
    }

    public void SaveJSONFile(string jsonContent, ArtistModel artist)
    {
        // Save the JSON file in a folder named GENERATED_ARTISTS_FOLDER inside the Assets folder
        string baseGeneratedArtistsPath = Path.Combine(Application.dataPath, GENERATED_ARTISTS_FOLDER_NAME);

        // Create a safe folder name from the artist name
        string safeFolderName = string.Join("_", artist.Name.Split(Path.GetInvalidFileNameChars()));
        string artistFolderPath = Path.Combine(baseGeneratedArtistsPath, safeFolderName);

        // Check if the artist folder already exists
        if (Directory.Exists(artistFolderPath))
        {
            Debug.LogWarning($"Artist '{artist.Name}' already exists. Skipping generation.");
            return;
        }

        // Create the artist's folder
        Directory.CreateDirectory(artistFolderPath);

        // Create the JSON file path
        string filePath = Path.Combine(artistFolderPath, $"{safeFolderName}.json");

        // Write the JSON to file with pretty formatting
        var options = new JsonSerializerOptions { WriteIndented = true };
        string formattedJson = JsonSerializer.Serialize(artist, options);
        System.IO.File.WriteAllText(filePath, formattedJson);

        Debug.Log($"Artist created, named : {artist.Name}");

#if UNITY_EDITOR
        // Refresh the asset database to show the new file in Unity Editor
        UnityEditor.AssetDatabase.Refresh();
#endif
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
    public int[] floorColor { get; set; }

    [JsonPropertyName("roofColor")]
    public int[] roofColor { get; set; }

    [JsonPropertyName("frameColor")]
    public int[] frameColor { get; set; }

    [JsonPropertyName("frameInformationColor")]
    public int[] frameInformationColor { get; set; }

    [JsonPropertyName("paintingPrompts")]
    public string[] paintingPrompts { get; set; }
}