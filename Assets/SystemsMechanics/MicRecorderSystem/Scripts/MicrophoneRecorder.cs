using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicRecorder;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneRecorder : MonoBehaviour
{
    public float minThreshold = 0;
    public float frequency = 0.0f;
    public int audioSampleRate = 44100;
    public string microphone;
    public FFTWindow fftWindow;
    public TMP_Dropdown micDropdown;

    private List<string> options = new List<string>();
    private int samples = 8192;
    private AudioSource audioSource;

    
    [SerializeField] private Button _stopRecording;
    [SerializeField] private Button _startRecording;
    private void Start()
    {
        //get components you'll need
        audioSource = GetComponent<AudioSource>();

        int index=0;
        // get all available microphones
        foreach (string device in Microphone.devices)
        {
            if (string.IsNullOrEmpty(microphone))
            {
                //mylogs Probably remove this later
                if (Debug.isDebugBuild) Debug.Log($"<color=purple>[{index++}] {device}</color>");

                //set default mic to first mic found.
                microphone = device;
            }

            options.Add(device);
        }

        microphone = options[PlayerPrefsManager.GetMicrophone()];
        minThreshold = PlayerPrefsManager.GetThreshold();
        //add mics to dropdown
        micDropdown.AddOptions(options);
        micDropdown.onValueChanged.AddListener(delegate { MicDropdownValueChangedHandler(micDropdown); });

        //initialize input with default mic
        UpdateMicrophone();
    }


    public List<AudioClip> clips = new List<AudioClip>();
    private void UpdateMicrophone()
    {
        StartListening(false);
    }


    [SerializeField] private float minNoiseTreshhold;
    public UnityEvent NoiseTrue;
    public UnityEvent NoiseFalse;

    [SerializeField] private int recordLength = 10;
    private void StartListening(bool saveRecords)
    {
        audioSource.Stop();
        

        if (saveRecords)
        {
            var clip = Microphone.Start(microphone, true, recordLength, audioSampleRate);
            clips.Add(clip);  
            audioSource.clip = clip;
            audioSource.loop = true;
        }
        else
        {
            var clip = Microphone.Start(microphone, false, recordLength, audioSampleRate);
            
            audioSource.clip = clip;
            audioSource.loop = false;
        }
        
        // Mute the sound with an Audio Mixer group becuase we don't want the player to hear it
        Debug.Log(Microphone.IsRecording(microphone).ToString());

        if (Microphone.IsRecording(microphone))
        {
            //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
            while (Microphone.GetPosition(microphone) > 0 == false)
            {
            }


           
            Debug.Log("recording started with " + microphone);

            // Start playing the audio source
            audioSource.Play();
            _isRecording = saveRecords;
            
            if (_isRecording == false)
            {
                StartCountdown();
            }
        }
        else
        {
            //microphone doesn't work for some reason

            Debug.Log(microphone + " doesn't work!");
        }
    }

    private async void StartCountdown()
    {
        await Task.Delay(1000 * recordLength);
        
        if (Application.isPlaying)
        {
            _isRecording = false;
            StartListening(false);
        }
    }
    public void MicDropdownValueChangedHandler(TMP_Dropdown mic)
    {
        microphone = options[mic.value];
        UpdateMicrophone();
    }

    private void OnDrawGizmos()
    {
        if (_isRecording)
        {
           Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawSphere(Vector3.zero,100);
    }

    private void Update()
    {
        if (minNoiseTreshhold < GetAveragedVolume())
        {
            SetNoiseState(true);
        }
        else
        {
            SetNoiseState(false);
        }
    }

    private bool _isNoiseTrue = false;
    private bool _isRecording = false;
    private void SetNoiseState(bool b)
    {
        if (_isNoiseTrue == false && b)
        {
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=green>set to true</color>");

            _isNoiseTrue = true;
            if (_isRecording == false)
            {
                StartListening(true);
            }
            
            NoiseTrue.Invoke();
            return;
        }

        if (_isNoiseTrue == true && b == false)
        {
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=red>Set to false</color>");
            _isNoiseTrue = false;
            NoiseFalse.Invoke();
            return;
        }
    }

    public float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audioSource.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }

        return a / 256;
    }

    public float GetFundamentalFrequency()
    {
        float fundamentalFrequency = 0.0f;
        float[] data = new float[samples];
        audioSource.GetSpectrumData(data, 0, fftWindow);
        float s = 0.0f;
        int i = 0;
        for (int j = 1; j < samples; j++)
        {
            if (data[j] > minThreshold) // volumn must meet minimum threshold
            {
                if (s < data[j])
                {
                    s = data[j];
                    i = j;
                }
            }
        }

        fundamentalFrequency = i * audioSampleRate / samples;
        frequency = fundamentalFrequency;
        return fundamentalFrequency;
    }
}