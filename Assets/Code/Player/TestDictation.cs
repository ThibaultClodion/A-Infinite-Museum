using Meta.WitAi.Requests;
using Oculus.Voice;
using Oculus.Voice.Dictation;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TestDictation : MonoBehaviour
{
    [SerializeField] private AppDictationExperience _appDictationExperience;
    [SerializeField] private AppVoiceExperience _appVoiceExperience;

    private void OnEnable()
    {
        _appDictationExperience.DictationEvents.OnFullTranscription.AddListener(OnTranscriptionComplete);
        _appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnVoiceTranscription);
        _appVoiceExperience.VoiceEvents.OnError.AddListener(OnVoiceError);
    }

    private void OnDisable()
    {
        _appDictationExperience.DictationEvents.OnFullTranscription.RemoveListener(OnTranscriptionComplete);
        _appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnVoiceTranscription);
        _appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnVoiceError);
    }

    private void Start()
    {
        // Start dictation
        _appDictationExperience.Activate();

        // Test AppVoiceExperience with text input (async)
        TestVoiceExperience();
    }

    private async void TestVoiceExperience()
    {
        // Wait a moment to ensure initialization
        await Task.Delay(500);
        
        try
        {
            VoiceServiceRequest request = await _appVoiceExperience.Activate(
                "This is a test prompt to activate the voice experience and check if it works correctly.",
                new Meta.WitAi.Configuration.WitRequestOptions(),
                new Meta.WitAi.Requests.VoiceServiceRequestEvents()
            );

            if (request != null)
            {
                Debug.Log("AppVoiceExperience request sent successfully!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AppVoiceExperience error: {e.Message}");
        }
    }

    private void OnTranscriptionComplete(string transcription)
    {
        Debug.Log($"[Dictation] User said: {transcription}");
        StartCoroutine(WaitBeforeListening());
    }

    private void OnVoiceTranscription(string transcription)
    {
        Debug.Log($"[Voice] Transcription: {transcription}");
    }

    private void OnVoiceError(string errorType, string errorMessage)
    {
        Debug.LogError($"[Voice] Error [{errorType}]: {errorMessage}");
    }

    IEnumerator WaitBeforeListening()
    {
        yield return new WaitForSeconds(2f);
        _appDictationExperience.Activate();
    }
}
