using Meta.XR.BuildingBlocks.AIBlocks;
using UnityEngine;
using UnityEngine.Events;

public class DetectSpeech : MonoBehaviour
{
    [HideInInspector] public UnityEvent<string> OnPlayerSpoke = new UnityEvent<string>();

    [Header("Speech Settings")]
    [SerializeField] private SpeechToTextAgent _speechToText;
    [SerializeField] private float _minSpeechVolume = 0.08f;
    [Tooltip("Use to save API Calls")] [SerializeField] private bool _disableAPICalls = true;
    private bool _isListening = true;

    private AudioClip _microphoneClip;
    private string _microphoneName;
    private const int c_sampleWindow = 128;

    private void Start()
    {
        if (_disableAPICalls)
        {
            return;
        }
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected!");
            return;
        }

        _microphoneName = Microphone.devices[0];
        _microphoneClip = Microphone.Start(_microphoneName, true, 20, AudioSettings.outputSampleRate);
    }

    private void Update()
    {
        if(_disableAPICalls || !_isListening)
        {
            return;
        }
        else if (GetMicrophoneVolume() > _minSpeechVolume)
        {
            _isListening = false;
            _speechToText.StartListening();
            _speechToText.onTranscript.AddListener(EndSpeech);
        }
    }

    private void OnDestroy()
    {
        // Stop the microphone when the object is destroyed
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(_microphoneName);
        }
    }

    public void EndSpeech(string speechText)
    {
        // Speech To Text Agent hold the microphone (because it also start it) but DetectSpeech need it again after the end of transcript.
        _speechToText.onTranscript.RemoveListener(EndSpeech);
        _microphoneClip = Microphone.Start(_microphoneName, true, 20, AudioSettings.outputSampleRate);
        _isListening = true;

        OnPlayerSpoke.Invoke(speechText);
    }

    public float GetMicrophoneVolume()
    {
        if (_microphoneClip == null)
        {
            return 0f;
        }

        // Get the current position in the audio clip
        int microphonePosition = Microphone.GetPosition(_microphoneName);
        int startPosition = microphonePosition - c_sampleWindow;

        // Handle wrap-around for looping audio
        if (startPosition < 0)
        {
            return 0f;
        }

        // Get audio samples
        float[] samples = new float[c_sampleWindow];
        _microphoneClip.GetData(samples, startPosition);

        // Calculate average volume (RMS - Root Mean Square)
        float sum = 0f;
        for (int i = 0; i < c_sampleWindow; i++)
        {
            sum += samples[i] * samples[i];
        }

        // rmsValue is typically between 0.0 and 1.0 (can go higher if very loud).
        float rmsValue = Mathf.Sqrt(sum / c_sampleWindow);
        return rmsValue;
    }
}
