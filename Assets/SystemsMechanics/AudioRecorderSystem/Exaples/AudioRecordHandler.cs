using System;
using AudioRecorderSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioRecordHandler : MonoBehaviour
{
    public UnityEvent OnStartRecording;
    public UnityEvent OnStopRecording;
    [SerializeField] private AudioRecorder _recorder;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _timeSamplesText;
    private RecordedClip _clip;

    private bool _lastStated;
    public async void Record()
    {
         _clip = await _recorder.StartRecording();
    }

    public void StopRecord()
    {
        if (_recorder.IsRecording)
        {
            _clip = _recorder.StopRecording();
        }
    }

    public void Play()
    {
        StopRecord();
        
        _clip.Play();
    }

    private void Update()
    {
        if (_lastStated != _recorder.IsRecording)
        {
            _lastStated = _recorder.IsRecording;
            if (_recorder.IsRecording)
            {
                OnStartRecording?.Invoke();
            }
            else
            {
                OnStopRecording?.Invoke();
            }
        }

        if (_clip != null && _timeText != null && _timeSamplesText != null)
        {
            _timeText.text = _clip.PlaybackTime.ToString();
            _timeSamplesText.text = _clip.PlaybackTimeSamples.ToString();
        }
    }
}
