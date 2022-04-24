using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using System;

public class SoundtrackPlayer : MonoBehaviour
{
    [SerializeField] List<Audio.AudioType> backgroundTypes = new List<Audio.AudioType>();

    [SerializeField] AudioSource soundtrackSource = null;

    // relationship between Audio background Type and AudioOption
    private Dictionary<Audio.AudioType, AudioOption> AudioBackgroundObject = new Dictionary<Audio.AudioType, AudioOption>(); 
    private List<int> clipsAmount = new List<int>();

    private Audio.AudioType _type = Audio.AudioType.SoundBackground_01;
    private SFXEvent _sfxEvent = SFXEvent.StartingSoundtrack;

    private void OnEnable()
    {
         AudioManager.AfterConfiguration += LoadBackgroundAudioObjects;
    }

    public void Start()
    {
        PrepareNewClipsByType(_type);
        SoundtrackSettingsChange(_type, _sfxEvent);
        _sfxEvent = SFXEvent.NoEvent;
        MoveClipAtTheEndOfRow(0);
    }

    public void Update()
    {
        if (!soundtrackSource.isPlaying)
        {
            NewBackgroundAudio(_type);
        }
    }

    public void SoundtrackSettingsChange(Audio.AudioType type, SFXEvent sfxEvent)
    {
        _sfxEvent = sfxEvent;
        _type = type;

        NewBackgroundAudio(_type , _sfxEvent);
    }

    private void NewBackgroundAudio(Audio.AudioType type, SFXEvent sFXEvent = SFXEvent.NoEvent)
    {
        int numberOfClips = -1;

        if(sFXEvent == SFXEvent.NoEvent)
        {
            numberOfClips = NextPseudoRandomAudio(type);
        }
        else
        {
            AudioManager.instance.PlayAudio(type, true, SFX_Event: sFXEvent);
            return;
        }

        AudioManager.instance.PlayAudio(type, true, numberOfNextClip: numberOfClips);
    }

    private int NextPseudoRandomAudio(Audio.AudioType type)
    {
        int numberOfClip = -1;
        float startingRNg = 15;
        float newgRNg = 0;
        float step = (100 - startingRNg) / clipsAmount.Count + 0.1f;
        newgRNg = startingRNg + newgRNg + step;

        for (int i = 0; i < clipsAmount.Count; i++)
        {
            float rng = UnityEngine.Random.Range(0f,100f);
            if (rng <= newgRNg)
            {
                numberOfClip = clipsAmount[i];
                MoveClipAtTheEndOfRow(numberOfClip);
                break;
            }

            newgRNg = newgRNg + step;
        }

        return numberOfClip;
    }

    private void MoveClipAtTheEndOfRow(int numClip)
    {
        clipsAmount.Remove(numClip);
        clipsAmount.Add(numClip);
    }

    private void PrepareNewClipsByType (Audio.AudioType type)
    {
        clipsAmount.Clear();
        foreach (KeyValuePair<Audio.AudioType, AudioOption> keyValue in AudioBackgroundObject)
        {
            if (keyValue.Key == type)
            {
                AudioOption tmp = keyValue.Value;
                for (int i = 0; i < tmp.audiobases.Count; i++)
                {
                    clipsAmount.Add(i);
                }

                break;
            }
        }
    }

    private void LoadBackgroundAudioObjects(Hashtable audioHash)
    {
        foreach (object type in backgroundTypes)
        {
            var audioType = (Audio.AudioType)type;
            AudioOption audioOption = (AudioOption)audioHash[audioType];

            AudioBackgroundObject.Add(audioType, audioOption);
        }
    }

    private void OnDisable()
    {
        AudioManager.AfterConfiguration -= LoadBackgroundAudioObjects;
    }
}
