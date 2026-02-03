using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "DescriptionConversionTable", menuName = "Scriptable Objects/DescriptionConversionTable")]
public class DescriptionConversionTableSO : ScriptableObject
{
    public List<DescriptionMeshPair> DescriptionToMesh;
    public List<DescriptionAnimationPair> DescriptionToAnimation;
    public List<DescriptionVoicePair> DescriptionToVoice;
}

[Serializable]
public class DescriptionMeshPair
{
    public string Description;
    public GameObject MeshPrefab;
}

[Serializable]
public class DescriptionAnimationPair
{
    public string Description;
    public AnimationClip AnimationClip;
}

[Serializable]
public class DescriptionVoicePair
{
    public string VoiceId;
    public string Description;
    public string VoiceName;
    public string Labels;
}

// This class is inspired by the ElevenLabs Unity SDK and use only to fetch voices from ElevenLabs API.
[CustomEditor(typeof(DescriptionConversionTableSO))]
public class DescriptionConversionTableEditor : Editor
{
    private string _apiKey = "";
    private string _endpoint = "https://api.elevenlabs.io";
    private const string c_apiKeyPrefKey = "ElevenLabs_ApiKey";
    private const string c_endpointPrefKey = "ElevenLabs_Endpoint";

    private void OnEnable()
    {
        _apiKey = EditorPrefs.GetString(c_apiKeyPrefKey, "");
        _endpoint = EditorPrefs.GetString(c_endpointPrefKey, "https://api.elevenlabs.io");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DescriptionConversionTableSO table = (DescriptionConversionTableSO)target;

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("ElevenLabs Voice Fetcher", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUI.BeginChangeCheck();
        _apiKey = EditorGUILayout.TextField(new GUIContent("API Key", "Your ElevenLabs API key"), _apiKey);
        _endpoint = EditorGUILayout.TextField(new GUIContent("Endpoint", "ElevenLabs API endpoint"), _endpoint);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(c_apiKeyPrefKey, _apiKey);
            EditorPrefs.SetString(c_endpointPrefKey, _endpoint);
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

        if (table.DescriptionToVoice != null && table.DescriptionToVoice.Count > 0)
        {
            EditorGUILayout.LabelField($"Voices loaded: {table.DescriptionToVoice.Count}", EditorStyles.helpBox);
        }
    }

    private async void FetchVoicesAsync(DescriptionConversionTableSO table)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            EditorUtility.DisplayDialog("ElevenLabs", "Please set your API Key first.", "OK");
            return;
        }

        string baseUrl = _endpoint.TrimEnd('/');
        string url = baseUrl + "/v1/voices";

        try
        {
            EditorUtility.DisplayProgressBar("ElevenLabs", "Fetching voices…", 0.3f);

            using UnityWebRequest req = UnityWebRequest.Get(url);
            req.SetRequestHeader("xi-api-key", _apiKey);

            UnityWebRequestAsyncOperation op = req.SendWebRequest();
            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(req.error + "\n" + req.downloadHandler?.text);
            }

            string json = req.downloadHandler.text;
            ParseAndApplyVoices(json, table);

            EditorUtility.SetDirty(table);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success",
                $"Successfully fetched {table.DescriptionToVoice.Count} voices from ElevenLabs!", "OK");
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

    private void ParseAndApplyVoices(string json, DescriptionConversionTableSO table)
    {
        if (table.DescriptionToVoice == null)
        {
            table.DescriptionToVoice = new List<DescriptionVoicePair>();
        }
        else
        {
            table.DescriptionToVoice.Clear();
        }

        int i = 0;
        while (i < json.Length)
        {
            int vidIdx = json.IndexOf("\"voice_id\"", i, StringComparison.Ordinal);
            if (vidIdx < 0)
            {
                break;
            }

            string voiceId = ExtractStringValue(json, vidIdx);
            i = vidIdx + 9;

            int nameIdx = json.IndexOf("\"name\"", i, StringComparison.Ordinal);
            string name = (nameIdx >= 0) ? ExtractStringValue(json, nameIdx) : null;

            int descIdx = json.IndexOf("\"description\"", i, StringComparison.Ordinal);
            string desc = (descIdx >= 0) ? ExtractStringValue(json, descIdx) : null;

            int labelsIdx = json.IndexOf("\"labels\"", i, StringComparison.Ordinal);
            string labelsFlat = null;
            if (labelsIdx >= 0)
            {
                int braceStart = json.IndexOf('{', labelsIdx);
                if (braceStart > 0)
                {
                    int braceEnd = FindMatchingBrace(json, braceStart);
                    if (braceEnd > braceStart)
                    {
                        string inner = json.Substring(braceStart + 1, braceEnd - braceStart - 1);
                        labelsFlat = FlattenLabels(inner);
                    }
                }
            }

            if (!string.IsNullOrEmpty(voiceId))
            {
                table.DescriptionToVoice.Add(new DescriptionVoicePair
                {
                    Description = !string.IsNullOrEmpty(desc) ? desc : "No description available",
                    VoiceId = voiceId,
                    VoiceName = !string.IsNullOrEmpty(name) ? name : voiceId,
                    Labels = labelsFlat
                });
            }
        }
    }

    private static string ExtractStringValue(string json, int keyIdx)
    {
        int colon = json.IndexOf(':', keyIdx);
        if (colon < 0)
        {
            return null;
        }

        int q1 = json.IndexOf('"', colon + 1);
        if (q1 < 0)
        {
            return null;
        }

        int q2 = json.IndexOf('"', q1 + 1);
        if (q2 < 0)
        {
            return null;
        }

        return json.Substring(q1 + 1, q2 - q1 - 1);
    }

    private static int FindMatchingBrace(string s, int openIdx)
    {
        int depth = 0;
        for (int j = openIdx; j < s.Length; j++)
        {
            if (s[j] == '{')
            {
                depth++;
            }
            else if (s[j] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return j;
                }
            }
        }
        return -1;
    }

    private static string FlattenLabels(string inner)
    {
        string cleaned = inner.Replace("\"", "").Trim();
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", "");
        cleaned = cleaned.Replace(":", "=");
        return cleaned;
    }
}
