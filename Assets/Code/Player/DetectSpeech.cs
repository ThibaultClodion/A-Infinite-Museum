using Meta.XR.BuildingBlocks.AIBlocks;
using UnityEngine;

public class DetectSpeech : MonoBehaviour
{
    [SerializeField] private SpeechToTextAgent _speechToText;
    [SerializeField] private float _minSpeechVolume = 0.01f;

    private AudioClip _microphoneClip;
    private string _microphoneName;
    private const int c_sampleWindow = 128;

    private void Start()
    {
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
        float volume = GetMicrophoneVolume();
        
        if (volume > _minSpeechVolume)
        {
            _speechToText.StartListening();
            _speechToText.onTranscript.AddListener(EndSpeech);
        }
    }

    public void EndSpeech(string test)
    {
        // Speech To Text Agent hold the microphone (because it also start it) but DetectSpeech need it again after the end of transcript.
        _speechToText.onTranscript.RemoveListener(EndSpeech);
        _microphoneClip = Microphone.Start(_microphoneName, true, 20, AudioSettings.outputSampleRate);
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

    private void OnDestroy()
    {
        // Stop the microphone when the object is destroyed
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(_microphoneName);
        }
    }
}
