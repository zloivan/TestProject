using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
   [SerializeField] private MicrophoneRecorder _recorder;

   private List<AudioClip> clips = new List<AudioClip>();
   
   [ContextMenu("Play")]
   public async void Play()
   {
      clips = _recorder.clips;
      var source = GetComponent<AudioSource>();
      for (int j = 0; j < clips.Count; j++)
      {
         source.PlayOneShot(clips[j]);

         while (source.isPlaying)
         {
            await Task.Delay(100);
         }
      }
   }
}
