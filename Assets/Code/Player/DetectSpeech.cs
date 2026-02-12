using Oculus.Voice.Dictation;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DetectSpeech : MonoBehaviour
{
    [HideInInspector] public UnityEvent<string> OnPlayerSpoke = new UnityEvent<string>();

    [Header("Speech Settings")]
    [SerializeField] private AppDictationExperience _appDictationExperience;
    [SerializeField] private float _restartDelay = 1.5f;

    private void OnEnable()
    {
        if (_appDictationExperience != null)
        {
            _appDictationExperience.DictationEvents.OnFullTranscription.AddListener(OnTranscriptionComplete);
            _appDictationExperience.DictationEvents.OnError.AddListener(OnDictationError);
        }
    }

    private void OnDisable()
    {
        if (_appDictationExperience != null)
        {
            _appDictationExperience.DictationEvents.OnFullTranscription.RemoveListener(OnTranscriptionComplete);
            _appDictationExperience.DictationEvents.OnError.RemoveListener(OnDictationError);
        }

        StopAllCoroutines();
        _appDictationExperience.Deactivate();
    }

    private void Start()
    {
        StartListening();
    }

    public void StartListening()
    {
        _appDictationExperience.Activate();
    }

    private void OnTranscriptionComplete(string transcription)
    {
        if (string.IsNullOrWhiteSpace(transcription))
        {
            RestartListening();
            return;
        }
        
        // Invoke the event for any listeners
        OnPlayerSpoke.Invoke(transcription);

        // Restart listening after a short delay
        RestartListening();
    }

    private void OnDictationError(string errorType, string errorMessage)
    {
        RestartListening();
    }

    private void RestartListening()
    {
        StopAllCoroutines();
        StartCoroutine(RestartListeningCoroutine());
    }

    private IEnumerator RestartListeningCoroutine()
    {    
        yield return new WaitForSeconds(_restartDelay);

        StartListening();
    }
}
