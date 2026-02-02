using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;

[CreateAssetMenu(fileName = "DescriptionConversionTable", menuName = "Scriptable Objects/DescriptionConversionTable")]
public class DescriptionConversionTable : ScriptableObject
{
    public List<DescriptionMeshPair> descriptionToMesh;
    public List<DescriptionAnimationPair> descriptionToAnimation;
    public List<DescriptionVoicePair> descriptionToVoice;
}

[Serializable]
public class DescriptionMeshPair
{
    public string description;
    public GameObject meshPrefab;
}

[Serializable]
public class DescriptionAnimationPair
{
    public string description;
    public AnimationClip animationClip;
}

[Serializable]
public class DescriptionVoicePair
{
    public string voiceId;
    public string description;
    public string voiceName;
    public string labels;
}

[CustomEditor(typeof(DescriptionConversionTable))]
public class DescriptionConversionTableEditor : Editor
{
    private string _apiKey = "";
    private string _endpoint = "https://api.elevenlabs.io";
    private const string ApiKeyPrefKey = "ElevenLabs_ApiKey";
    private const string EndpointPrefKey = "ElevenLabs_Endpoint";

    private void OnEnable()
    {
        _apiKey = EditorPrefs.GetString(ApiKeyPrefKey, "");
        _endpoint = EditorPrefs.GetString(EndpointPrefKey, "https://api.elevenlabs.io");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DescriptionConversionTable table = (DescriptionConversionTable)target;

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("ElevenLabs Voice Fetcher", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUI.BeginChangeCheck();
        _apiKey = EditorGUILayout.TextField(new GUIContent("API Key", "Your ElevenLabs API key"), _apiKey);
        _endpoint = EditorGUILayout.TextField(new GUIContent("Endpoint", "ElevenLabs API endpoint"), _endpoint);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(ApiKeyPrefKey, _apiKey);
            EditorPrefs.SetString(EndpointPrefKey, _endpoint);
        }

        EditorGUILayout.Space(5);

        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_apiKey)))
        {
            if (GUILayout.Button("Fetch Voices from ElevenLabs", GUILayout.Height(40)))
            {
                FetchVoicesAsync(table);
            }
        }

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            EditorGUILayout.HelpBox("Please set your ElevenLabs API Key to fetch voices.", MessageType.Warning);
        }

        EditorGUILayout.Space(10);

        if (table.descriptionToVoice != null && table.descriptionToVoice.Count > 0)
        {
            EditorGUILayout.LabelField($"Voices loaded: {table.descriptionToVoice.Count}", EditorStyles.helpBox);
        }
    }

    private async void FetchVoicesAsync(DescriptionConversionTable table)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            EditorUtility.DisplayDialog("ElevenLabs", "Please set your API Key first.", "OK");
            return;
        }

        var baseUrl = _endpoint.TrimEnd('/');
        var url = baseUrl + "/v1/voices";

        try
        {
            EditorUtility.DisplayProgressBar("ElevenLabs", "Fetching voices…", 0.3f);

            using (var req = UnityWebRequest.Get(url))
            {
                req.SetRequestHeader("xi-api-key", _apiKey);

                var op = req.SendWebRequest();
                while (!op.isDone)
                {
                    await Task.Yield();
                }

                if (req.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(req.error + "\n" + req.downloadHandler?.text);
                }

                var json = req.downloadHandler.text;
                ParseAndApplyVoices(json, table);

                EditorUtility.SetDirty(table);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Success",
                    $"Successfully fetched {table.descriptionToVoice.Count} voices from ElevenLabs!", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ElevenLabs] Failed to fetch voices: {ex.Message}");
            EditorUtility.DisplayDialog("Error",
                "Failed to fetch voices. Check Console for details.", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void ParseAndApplyVoices(string json, DescriptionConversionTable table)
    {
        if (table.descriptionToVoice == null)
        {
            table.descriptionToVoice = new List<DescriptionVoicePair>();
        }
        else
        {
            table.descriptionToVoice.Clear();
        }

        int i = 0;
        while (i < json.Length)
        {
            var vidIdx = json.IndexOf("\"voice_id\"", i, StringComparison.Ordinal);
            if (vidIdx < 0) break;

            var voiceId = ExtractStringValue(json, vidIdx);
            i = vidIdx + 9;

            var nameIdx = json.IndexOf("\"name\"", i, StringComparison.Ordinal);
            var name = (nameIdx >= 0) ? ExtractStringValue(json, nameIdx) : null;

            var descIdx = json.IndexOf("\"description\"", i, StringComparison.Ordinal);
            var desc = (descIdx >= 0) ? ExtractStringValue(json, descIdx) : null;

            var labelsIdx = json.IndexOf("\"labels\"", i, StringComparison.Ordinal);
            string labelsFlat = null;
            if (labelsIdx >= 0)
            {
                int braceStart = json.IndexOf('{', labelsIdx);
                if (braceStart > 0)
                {
                    int braceEnd = FindMatchingBrace(json, braceStart);
                    if (braceEnd > braceStart)
                    {
                        var inner = json.Substring(braceStart + 1, braceEnd - braceStart - 1);
                        labelsFlat = FlattenLabels(inner);
                    }
                }
            }

            if (!string.IsNullOrEmpty(voiceId))
            {
                table.descriptionToVoice.Add(new DescriptionVoicePair
                {
                    description = !string.IsNullOrEmpty(desc) ? desc : "No description available",
                    voiceId = voiceId,
                    voiceName = !string.IsNullOrEmpty(name) ? name : voiceId,
                    labels = labelsFlat
                });
            }
        }
    }

    private static string ExtractStringValue(string json, int keyIdx)
    {
        int colon = json.IndexOf(':', keyIdx);
        if (colon < 0) return null;
        int q1 = json.IndexOf('"', colon + 1);
        if (q1 < 0) return null;
        int q2 = json.IndexOf('"', q1 + 1);
        if (q2 < 0) return null;
        return json.Substring(q1 + 1, q2 - q1 - 1);
    }

    private static int FindMatchingBrace(string s, int openIdx)
    {
        int depth = 0;
        for (int j = openIdx; j < s.Length; j++)
        {
            if (s[j] == '{') depth++;
            else if (s[j] == '}')
            {
                depth--;
                if (depth == 0) return j;
            }
        }
        return -1;
    }

    private static string FlattenLabels(string inner)
    {
        var cleaned = inner.Replace("\"", "").Trim();
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", "");
        cleaned = cleaned.Replace(":", "=");
        return cleaned;
    }
}