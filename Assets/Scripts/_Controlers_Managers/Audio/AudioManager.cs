using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public static Action<Hashtable> AfterConfiguration;

        public bool debug;

        [SerializeField] AudioSource backgroundAudioSource;
        [SerializeField] AudioSource buttonAudioSource;

        private Hashtable m_AudioTable; // relationship of audio types (key) and tracks (value)
        private Hashtable m_JobTable;   // relationship between audio types (key) and jobs (value)

        private string pathBackground = "ScriptableObject/Audio/Background";
        private string pathBSF = "ScriptableObject/Audio/BSF";

        private class AudioJob
        {
            public AudioAction action;
            public AudioType type;
            public bool fade;
            public WaitForSeconds delay;
            public SFXEvent SFX_Event = SFXEvent.NoEvent;
            public int numberOfNextClip = -1;

            public AudioJob(AudioAction _action, AudioType _type, bool _fade, float _delay, SFXEvent _SFX_Event = SFXEvent.NoEvent, int _numberOfNextClip = -1)
            {
                action = _action;
                type = _type;
                fade = _fade;
                delay = _delay > 0f ? new WaitForSeconds(_delay) : null;
                SFX_Event = _SFX_Event;
                numberOfNextClip = _numberOfNextClip;
            }
        }

        private void Awake()
        {
            if (!instance)
            {
                Configure();
            }
        }

        private void Start()
        {
            AfterConfiguration.Invoke(m_AudioTable);
        }

        private void OnDisable()
        {
            Dispose();
        }

        // Todo Mysteri Error když je fade defaultni tak nekdy corutina je null WTf ?? WTF ?? to same s _delay
        public void PlayAudio(AudioType _type, bool _fade , float _delay = 0.0F , SFXEvent SFX_Event = SFXEvent.NoEvent, int numberOfNextClip = -1)
        {
            AddJob(new AudioJob(AudioAction.START, _type, _fade, _delay, SFX_Event, numberOfNextClip));
        }

        public void StopAudio(AudioType _type, bool _fade = false, float _delay = 0.0F)
        {
            AddJob(new AudioJob(AudioAction.STOP, _type, _fade, _delay));
        }

        public void RestartAudio(AudioType _type, bool _fade = false, float _delay = 0.0F)
        {
            AddJob(new AudioJob(AudioAction.RESTART, _type, _fade, _delay));
        }

        private void Configure()
        {
            instance = this;
            m_AudioTable = new Hashtable();
            m_JobTable = new Hashtable();
            GenerateAudioTable();
        }

        private void Dispose()
        {
            // cancel all jobs in progress
            foreach (DictionaryEntry _kvp in m_JobTable)
            {
                Coroutine _job = (Coroutine)_kvp.Value;
                StopCoroutine(_job);
            }
        }

        private void AddJob(AudioJob _job)
        {
            // cancel any job that might be using this job's audio source
            RemoveConflictingJobs(_job.type);

            Coroutine _jobRunner = StartCoroutine(RunAudioJob(_job));
            if (_jobRunner == null)
            {
                Log("Audio JobRunner is null. Probably cant find audio or source OR mistery Error with button sound");
                return;
            }

            m_JobTable.Add(_job.type, _jobRunner);
            Log("Starting job on [" + _job.type + "] with operation: " + _job.action);
        }

        private void RemoveJob(AudioType _type)
        {
            if (!m_JobTable.ContainsKey(_type))
            {
                Log("Trying to stop a job [" + _type + "] that is not running.");
                return;
            }
            Coroutine _runningJob = (Coroutine)m_JobTable[_type];
            StopCoroutine(_runningJob);
            m_JobTable.Remove(_type);
        }

        private void RemoveConflictingJobs(AudioType _type)
        {
            // cancel the job if one exists with the same type
            if (m_JobTable.ContainsKey(_type))
            {
                RemoveJob(_type);
            }

            //// cancel jobs that share the same audio track
            //AudioType _conflictAudio = AudioType.None;
            //AudioTrack _audioTrackNeeded = GetAudioTrack(_type, "Get Audio Track Needed");
            //foreach (DictionaryEntry _entry in m_JobTable)
            //{
            //    AudioType _audioType = (AudioType)_entry.Key;
            //    AudioTrack _audioTrackInUse = GetAudioTrack(_audioType, "Get Audio Track In Use");
            //    if (_audioTrackInUse.source == _audioTrackNeeded.source)
            //    {
            //        _conflictAudio = _audioType;
            //        break;
            //    }
            //}
            //if (_conflictAudio != AudioType.None)
            //{
            //    RemoveJob(_conflictAudio);
            //}
        }

        private IEnumerator RunAudioJob(AudioJob _job)
        {
            if (_job.delay != null) yield return _job.delay;

            AudioOption audioOption = GetAudioOption(_job.type); // track existence should be verified by now
            audioOption.Source.clip = GetAudioClip(audioOption, _job.SFX_Event, _job.numberOfNextClip);

            if (audioOption.Source == null || audioOption.Source.clip == null)
                Log("Source or clip is null. Check audioObject in unity.");

            float _initial = 0f;
            float _target = 0.3f;
            switch (_job.action)
            {
                case AudioAction.START:
                    audioOption.Source.Play();
                    break;
                case AudioAction.STOP when !_job.fade:
                    audioOption.Source.Stop();
                    break;
                case AudioAction.STOP:
                    _initial = 0.3f;
                    _target = 0f;
                    break;
                case AudioAction.RESTART:
                    audioOption.Source.Stop();
                    audioOption.Source.Play();
                    break;
            }

            // fade volume
            if (_job.fade)
            {
                float _duration = 1.0f;
                float _timer = 0.0f;

                while (_timer <= _duration)
                {
                    audioOption.Source.volume = Mathf.Lerp(_initial, _target, _timer / _duration);
                    _timer += Time.deltaTime;
                    yield return null;
                }

                // if _timer was 0.9999 and Time.deltaTime was 0.01 we would not have reached the target
                // make sure the volume is set to the value we want
                audioOption.Source.volume = _target;

                if (_job.action == AudioAction.STOP)
                {
                    audioOption.Source.Stop();
                }
            }

            m_JobTable.Remove(_job.type);
            Log("Job count: " + m_JobTable.Count);
        }

        private void GenerateAudioTable()
        {
            LoadAudioBackground();
            LoadAudioBTNSound();
        }

        private void LoadAudioBTNSound()
        {
            AudioObject[] audioObjecs = (AudioObject[])UnityEngine.Resources.LoadAll<AudioObject>(pathBSF);

            foreach (AudioObject _audioObj in audioObjecs)
            {
                // do not duplicate keys
                if (m_AudioTable.ContainsKey(_audioObj.type))
                {
                    LogWarning("You are trying to register audio [" + _audioObj.type + "] that has already been registered.");
                }
                else
                {
                    _audioObj.audioOption.Source = ChoiseRightAudioSource(_audioObj.type);
                    m_AudioTable.Add(_audioObj.type, _audioObj.audioOption);
                    Log("Registering audio [" + _audioObj.type + "]");
                }
            }
        }

        private void LoadAudioBackground()
        {
            AudioObject[] audioObjecs = (AudioObject[])UnityEngine.Resources.LoadAll<AudioObject>(pathBackground);

            foreach (AudioObject _audioObj in audioObjecs)
            {
                // do not duplicate keys
                if (m_AudioTable.ContainsKey(_audioObj.type))
                {
                    LogWarning("You are trying to register audio [" + _audioObj.type + "] that has already been registered.");
                }
                else
                {
                    _audioObj.audioOption.Source = ChoiseRightAudioSource(_audioObj.type);
                    m_AudioTable.Add(_audioObj.type, _audioObj.audioOption);
                    Log("Registering audio [" + _audioObj.type + "]");
                }
            }
        }

        private AudioSource ChoiseRightAudioSource(AudioType type)
        {
            AudioSource source = null;

            switch (type)
            {
                case AudioType.None:
                    break;
                case AudioType.SFX_01:
                    break;
                case AudioType.BTN_01:
                    source = buttonAudioSource;
                    break;
                case AudioType.SoundBackground_01:
                    source = backgroundAudioSource;
                    break;
                default:
                    break;
            }

            if (source == null)
                LogError($"Source is null. From audiotype {type}. Check references in script");

            return source;
        }

        private AudioOption GetAudioOption(AudioType _type, string _job = "")
        {
            if (!m_AudioTable.ContainsKey(_type))
            {
                LogWarning("You are trying to <color=#fff>" + _job + "</color> for [" + _type + "] but no track was found supporting this audio type.");
                return null;
            }

            return (AudioOption)m_AudioTable[_type];
        }

        private AudioClip GetAudioClip(AudioOption option , SFXEvent sfxEvent, int clipNumber)
        {
            if (option.audiobases.Count <= 0)
                LogError("GetAudioClip failed -> audio option is missing.");

            if(sfxEvent == SFXEvent.NoEvent)
            {
                int count = option.audiobases.Count;

                if (clipNumber!= -1 && clipNumber <= count)
                    return option.audiobases[clipNumber].clip;
                else
                {
                    int rng = UnityEngine.Random.Range(0, count);
                    return option.audiobases[rng].clip;
                }
            }

            foreach (AudioBase audioBase in option.audiobases)
            {
                if (audioBase.SFX_Event == sfxEvent)
                    return audioBase.clip;
            }

            return null;
        }

        private void Log(string _msg)
        {
            if (!debug) return;
            Debug.Log("[Audio Controller]: " + _msg);
        }

        private void LogWarning(string _msg)
        {
            if (!debug) return;
            Debug.LogWarning("[Audio Controller]: " + _msg);
        }

        private void LogError(string _msg)
        {
            if (!debug) return;
            Debug.LogError("[Audio Manager]: " + _msg);
        }

        private enum AudioAction
        {
            START,
            STOP,
            RESTART
        }
    }
}