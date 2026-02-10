using UnityEngine;
using Meta.XR.BuildingBlocks.AIBlocks;
using Google.GenAI;
using Google.GenAI.Types;

public class Artist : MonoBehaviour
{
    [Header("General Data")]
    [SerializeField] private DescriptionConversionTableSO _allDescriptionConversionTable;

    [Header("Components")]
    [SerializeField] private RuntimeAnimatorController _animatorController;
    [SerializeField] private AIProviderBase _textToSpeechAsset;
    [SerializeField] private TextToSpeechAgent _textToSpeech;
    private ArtistSO _artistSO;
    private Animator _animator;
    private AnimationClip _animationClip;
    private GameObject _lastMesh;

    public void InitializeArtist(ArtistSO artistSO, Transform spawnTransform)
    {
        _artistSO = artistSO;

        InitializeMesh();
        InitializeAnimation();
        InitializeVoice();

        // Change position and rotation
        transform.position = spawnTransform.position;
        transform.rotation = spawnTransform.rotation;
    }

    private void InitializeMesh()
    {
        if (_lastMesh != null)
        {
            Destroy(_lastMesh);
        }

        foreach (DescriptionMeshPair meshPair in _allDescriptionConversionTable.DescriptionToMesh)
        {
            if (meshPair.Description.Contains(_artistSO.ArtistModel.MeshDescription))
            {
                _lastMesh = Instantiate(meshPair.MeshPrefab, transform);
                _animator = _lastMesh.GetComponent<Animator>();
                break;
            }
        }
    }

    private void InitializeAnimation()
    {
        foreach (DescriptionAnimationPair animationPair in _allDescriptionConversionTable.DescriptionToAnimation)
        {
            if (animationPair.Description.Contains(_artistSO.ArtistModel.AnimationDescription))
            {
                _animationClip = animationPair.AnimationClip;
                break;
            }
        }

        _animator.runtimeAnimatorController = _animatorController;
        _animator.Play(_animationClip.name);
    }

    private void InitializeVoice()
    {
        foreach (DescriptionVoicePair voicePair in _allDescriptionConversionTable.DescriptionToVoice)
        {
            if (_artistSO.ArtistModel.VoiceDescription.Contains(voicePair.Description))
            {
                if (_textToSpeechAsset is ElevenLabsProvider elevenLabs)
                {
                    elevenLabs.SetVoice(voicePair.VoiceId);
                }
                break;
            }
        }
    }

    private async void RespondToPlayer(string playerSpeech)
    {
        string apiKey = UnityEditor.EditorPrefs.GetString(ArtistGeneratorEditor.c_apiKeyPrefKey, "");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogError("Gemini API Key is not set. Please configure it in the Inspector.");
            return;
        }

        Client client = new Client(apiKey: apiKey);

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: "gemini-2.5-flash-lite",
            contents: $"You are {_artistSO.ArtistModel.Name}, an artist who describe himself as a {_artistSO.ArtistModel.Description}." +
            $"Respond to a visitor who sayed : {playerSpeech} without being too long like in a normal conversation.");

        _textToSpeech.SpeakText(response.Candidates[0].Content.Parts[0].Text);
    }
}
