using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AudioRecorderSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class RecordedClip : MonoBehaviour
    {
        [SerializeField] private AudioSource _originalAudioSource;

        public float PlaybackTime => _originalAudioSource.time;
        public int PlaybackTimeSamples => _originalAudioSource.timeSamples;
        
        
        private double _clipLength;
        private CancellationTokenSource _cts;
        private UniTask _current;

        private void Awake()
        {
            if (_originalAudioSource == null)
            {
                _originalAudioSource = GetComponent<AudioSource>();
            }
            
            _cts = new CancellationTokenSource();
        }

        public void Play()
        {
            Stop();
            
            _originalAudioSource.time = 0f;
            _originalAudioSource.Play();
            StopAfterComplete(_clipLength);
        }

        private async void StopAfterComplete(double clipLength)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }
            
            _current = UniTask.Delay(1000 * (int)clipLength ,cancellationToken: _cts.Token);
            await _current;
            Stop();
        }

        public void Stop()
        {
            if (_originalAudioSource.isPlaying)
            {
                //mylogs Probably remove this later
                if (Debug.isDebugBuild) Debug.Log($"<color=purple>STOPED</color>");

                _originalAudioSource.Stop();
            }
        }

        public void SetupClip(AudioClip clip)
        {
            _originalAudioSource.clip = clip;
            _originalAudioSource.loop = true;
        }

        public void SetClipLength(double totalSeconds)
        {
            _clipLength = totalSeconds;
        }
    }
}