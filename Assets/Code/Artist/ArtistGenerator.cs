using UnityEngine;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.IO;

public class ArtistGenerator : MonoBehaviour
{
    [Header("Generation Parameters")]
    [SerializeField] private DescriptionConversionTableSO _descriptionConversionTable;
    [Range(1.0f, 1.8f)]
    [SerializeField] private float _temperature = 1.8f;
    [Range(0.5f, 0.95f)]
    [SerializeField] private float _topP = 0.95f;

    private const string c_new_generated_artist_folder = "NewGeneratedArtists";
    private const string c_generated_artist_folder = "GeneratedArtists";

    public async Task GenerateAnArtist()
    {
#if UNITY_EDITOR
        string apiKey = UnityEditor.EditorPrefs.GetString(ArtistGeneratorEditor.c_apiKeyPrefKey, "");
        
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogError("Gemini API Key is not set. Please configure it in the Inspector.");
            return;
        }

        Client client = new Client(apiKey: apiKey);
#else
        Debug.LogError("Artist generation is only available in the Unity Editor.");
        return;
#endif

        // Configure the content generation parameters
        GenerateContentConfig config = new GenerateContentConfig
        {
            ResponseMimeType = "application/json",
            Seed = Random.Range(1, int.MaxValue),
            Temperature = _temperature,
            TopP = _topP
        };

        // Prepare enum lists for mesh, animation, and voice descriptions
        List<string> meshDescriptions = _descriptionConversionTable.DescriptionToMesh
            .Select(pair => pair.Description)
            .ToList();

        List<string> animationDescriptions = _descriptionConversionTable.DescriptionToAnimation
            .Select(pair => pair.Description)
            .ToList();

        List<string> voiceDescriptions = _descriptionConversionTable.DescriptionToVoice
            .Select(pair => pair.Description + " " + pair.VoiceName + " " + pair.Labels)
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
                        Description = "A list of 6 prompts that will be used to generate the paintings (based on the description of the artist).",
                        Items = new Schema() { Type = Type.STRING }
                    }
                }
            },

            Required = new List<string>() { "name", "description", "meshDescription", "animationDescription", "voiceDescription", 
                "wallsColor", "floorColor", "roofColor", "frameColor", "frameInformationColor", "paintingPrompts" }
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: "gemini-3-flash-preview",
            contents: $"Generates a fictional painter with his painting style, painting techniques, and everything that defines his art as a whole. " +
            $"You will use this mesh: {meshDescriptions[Random.Range(0, meshDescriptions.Count)]} ",
            config: config
        );

        // Parse the JSON response
        string jsonResponse = response.Candidates[0].Content.Parts[0].Text;
        ArtistModel artist = JsonSerializer.Deserialize<ArtistModel>(jsonResponse);

        // Save the artist data to a JSON file
        SaveJSONFile(jsonResponse, artist);
    }

    public void SaveJSONFile(string jsonContent, ArtistModel artist)
    {
        // Create a safe folder name from the artist name
        string baseGeneratedArtistsPath = Path.Combine(Application.dataPath, c_new_generated_artist_folder);
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

        // Create the JSON file path and write the content
        string filePath = Path.Combine(artistFolderPath, $"{safeFolderName}.json");
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string formattedJson = JsonSerializer.Serialize(artist, options);
        System.IO.File.WriteAllText(filePath, formattedJson);

        Debug.Log($"Artist created, named : {artist.Name}");

#if UNITY_EDITOR
        // Refresh the asset database to show the new file in Unity Editor
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
