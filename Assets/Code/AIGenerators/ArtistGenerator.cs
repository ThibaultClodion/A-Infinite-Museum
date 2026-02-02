using UnityEngine;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;

public class ArtistGenerator : MonoBehaviour
{
    private Client client = new Client(apiKey: APIKeys.GEMINI_API_KEY);

    public async Task GenerateAnArtist()
    {
        if(client == null)
        {
            Debug.LogError("Client is not initialized. Please check your API key.");
            return;
        }

        var config = new GenerateContentConfig();
        config.ResponseMimeType = "application/json";
        config.Seed = Random.Range(1, int.MaxValue);
        config.Temperature = 1.8f;
        config.TopP = 0.95f;

        config.ResponseJsonSchema = new Schema()
        {
            Type = Type.OBJECT,
            Properties = new Dictionary<string, Schema>()
            {
                { "name", new Schema() { Type = Type.STRING, Description = "The name of the artist." } },
                { "description", new Schema() { Type = Type.STRING, Description = "The description of the artist." } }
            },
            Required = new List<string>() { "name", "description" }
        };

        var response = await client.Models.GenerateContentAsync(
            model: "gemini-3-flash-preview",
            contents: "Generate a fictionnal painter name an description.",
            config
        );

        // Parse the JSON response
        string jsonResponse = response.Candidates[0].Content.Parts[0].Text;
        ArtistModel artist = JsonSerializer.Deserialize<ArtistModel>(jsonResponse);

        string name = artist.Name;
        string description = artist.Description;

        Debug.Log($"Artist Name: {name}");
        Debug.Log($"Artist Description: {description}");
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
    public string WallsColor { get; set; }

    [JsonPropertyName("floorColor")]
    public string floorColor { get; set; }

    [JsonPropertyName("roofColor")]
    public string roofColor { get; set; }

    [JsonPropertyName("frameColor")]
    public string frameColor { get; set; }

    [JsonPropertyName("frameInformationColor")]
    public string frameInformationColor { get; set; }

    [JsonPropertyName("paintingPrompts")]
    public string[] paintingPrompts { get; set; }
}