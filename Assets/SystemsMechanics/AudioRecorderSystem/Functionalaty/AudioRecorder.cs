using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace AudioRecorderSystem
{
    public class AudioRecorder : MonoBehaviour
    {
        [SerializeField] private int _maxDuration;
        [SerializeField] private int _audioSampleRate = 44100;
        [SerializeField] private RecordedClip _recordedClip;

        public bool IsRecording => _isRecording;

        private string _microphoneName;
        private bool _hasPermission;
        private DateTime _startRecordTime;
        private bool _isRecording;

        private void Start()
        {
            _hasPermission = GetPermission();

            if (_hasPermission == false)
            {
                AskForPermission(() => { _hasPermission = true; });
            }
            else
            {
                _microphoneName = Microphone.devices[0];
            }
        }

        private bool GetPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return Permission.HasUserAuthorizedPermission(Permission.Microphone);
#endif
            return true;
        }

        private void AskForPermission(Action onGranted)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Permission.HasUserAuthorizedPermission( Permission.Microphone) == false)
            {
                var callback = new PermissionCallbacks();
                callback.PermissionGranted += someText =>
                {
                    onGranted?.Invoke();
                };
                
                Permission.RequestUserPermission(Permission.Microphone, callback);
            }
#endif
            return;
        }

        public async UniTask<RecordedClip> StartRecording()
        {
            if (_hasPermission == false)
            {
                AskForPermission(() => { _hasPermission = true; });
                return null;
            }

            if (string.IsNullOrEmpty(_microphoneName))
            {
                _microphoneName = Microphone.devices[0];
            }

            _isRecording = true;
            _startRecordTime = DateTime.Now;


            var clip = Microphone.Start(_microphoneName, true, _maxDuration, _audioSampleRate);
            _recordedClip.SetupClip(clip);

            await UniTask.WaitWhile(() => Microphone.IsRecording(_microphoneName));
            if (_isRecording)
            {
                _isRecording = false;
                _recordedClip.SetClipLength((DateTime.Now - _startRecordTime).TotalSeconds);
            }

            return _recordedClip;
        }

        public RecordedClip StopRecording()
        {
            //stop recording and return new currentRecord
            if (_isRecording)
            {
                Microphone.End(_microphoneName);
                _recordedClip.SetClipLength((DateTime.Now - _startRecordTime).TotalSeconds);
                _isRecording = false;
            }

            return _recordedClip;
        }
    }
}