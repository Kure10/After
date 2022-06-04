using UnityEngine;
using System.Collections.Generic;

namespace Audio
{
    [System.Serializable]
    public class AudioOption
    {
        public List<AudioBase> audiobases = new List<AudioBase>();
        private AudioSource source;

        public AudioSource Source { get { return source; } set { source = value; } }
    }

    [System.Serializable]
    public class AudioBase
    {
        public SFXEvent SFX_Event = SFXEvent.NoEvent;
        public AudioClip clip;
    }

    // Todo Pokud bude v AudioManageru v metode PlayAudio || StopAudio || RestartAudio moc parametru.
    // vytvorim novou clasu pro definovani techto parametru abych je tam mohl nacpat...
    //public class AudioPreSettings
    //{
    //    public SFXEvent SFX_Event = SFXEvent.NoEvent;

    //}

    public enum AudioType
    {
        None,
        SFX_01,
        BTN_01,
        SoundBackground_01,
        SoundEnviroment_01,
        SoundBattle_01
    }

    public enum SFXEvent
    {
        NoEvent,
        StartingSoundtrack,
        ButtonClick,
        ButtonHover,
        MapClose,
        MapOpen,
        BackInMainMenu,
        BattleStart,
    }
}

