using UnityEngine;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;


public class GeminiAPI : MonoBehaviour
{
    private Client client;

    private async void Start()
    {
        client = new Client(apiKey: GeminiKeys.API_KEY);

        /*var models = await client.Models.ListAsync();
        await foreach (var m in models)
        {
            Debug.Log($"Modèle disponible : {m.Name} - Supporte GenerateContent: {m.DisplayName}");
        }*/

        string test = await GenerateArtist();
    }

    public async Task<string> GenerateArtist()
    {
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
            model: "gemini-2.0-flash-lite",
            contents: "Generate a fictionnal painter name an description that I could then reuse to generate 6 paintings in Gemini Nano Banana",
            config
        );

        string jsonResponse = response.Candidates[0].Content.Parts[0].Text;

        // Parse the JSON response
        ArtistModel artist = JsonSerializer.Deserialize<ArtistModel>(jsonResponse);

        string name = artist.Name;
        string description = artist.Description;

        Debug.Log($"Artist Name: {name}");
        Debug.Log($"Artist Description: {description}");

        return jsonResponse;
    }
}

public class ArtistModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}