using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class SoundtrackPlayer : MonoBehaviour
{
    //[SerializeField] int randomLimit = 2;
    //[SerializeField] AudioSource soundtrackSource;
    //[SerializeField] private AudioObject _startingAudioObject = null;

    //// private float _clipTimer = 0; možna pro komplexnejsi rešení..

    //private List<AudioObject> _backgroundAudioObjectList = new List<AudioObject>();

    //private bool audioWasloaded = false;

    //private void OnEnable()
    //{
    //  // AudioManager.AfterConfigurate += LoadBackgroundAudioObjects;
    //}

    //private void Start()
    //{
    //    LoadBackgroundAudioObjects();
    //    PlayStartAudio();
    //}

    //private void Update()
    //{
    //    if(!soundtrackSource.isPlaying)
    //    {
    //        StartNewSoundtrackAudio();
    //    }
    //}

    //private void StartNewSoundtrackAudio ()
    //{
    //    int rng = Random.Range(0 ,_backgroundAudioObjectList.Count);
    //    AudioClip clip = GetRandomClipFromAudioObject(_backgroundAudioObjectList[rng]);
    //   // AudioManager.instance.PlayAudio(_backgroundAudioObjectList[rng].type, clip, soundtrackSource);
    //}

    //private void PlayStartAudio()
    //{
    //    if (_startingAudioObject == null)
    //        return;

    //    AudioClip startingAudioClip = GetRandomClipFromAudioObject(_startingAudioObject);

    //    if (startingAudioClip == null)
    //    {
    //        Debug.LogWarning("There is no starting Audio. Maybe should be some");
    //        startingAudioClip = GetRandomAudioClip();
    //    }

    // //   AudioManager.instance.PlayAudio(_startingAudioObject.type, startingAudioClip, soundtrackSource);
    //}

    //private AudioClip GetRandomAudioClip()
    //{
    //    AudioClip startingAudioClip = null;
    //    AudioObject audioObject = null;
    //    if (_backgroundAudioObjectList.Count > 0)
    //    {
    //        int rngA = Random.Range(0, _backgroundAudioObjectList.Count);
    //        audioObject = _backgroundAudioObjectList[rngA];
    //    }

    //    if (audioObject != null)
    //    {
    //        startingAudioClip = GetRandomClipFromAudioObject(audioObject);
          
    //    }
    //    else
    //    {
    //        Debug.LogWarning("AudioObject is Null something is wrong");
    //    }

    //    return startingAudioClip;
    //}

    //private AudioClip GetRandomClipFromAudioObject(AudioObject audioObject)
    //{
    //    AudioClip startingAudioClip = null;

    //    if (audioObject.audioTrack.Length > 0)
    //    {
    //        int rng = Random.Range(0, audioObject.audioTrack.Length);

    //        if (audioObject.audioTrack[rng].clips.Count > 0)
    //        {
    //            int rngClips = Random.Range(0, audioObject.audioTrack[rng].clips.Count);
    //            startingAudioClip = audioObject.audioTrack[rng].clips[rngClips];
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Starting Audio was not found. No clips in startingAudioTrack");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Starting Audio was not found. AudioTrack lenght is 0");
    //    }

    //    return startingAudioClip;
    //}


    //private void LoadBackgroundAudioObjects ()
    //{
    //    if (AudioManager.instance != null && !audioWasloaded)
    //    {
    //     //   _backgroundAudioObjectList.AddRange(AudioManager.instance.LoadBackgroundAudio());
    //        audioWasloaded = true;
    //    }
    //}

    //private void OnDisable()
    //{
    //    AudioManager.AfterConfigurate -= LoadBackgroundAudioObjects;
    //}
}
