using UnityEngine;
using System.Collections.Generic;

namespace Audio
{
    [System.Serializable]
    public class AudioOption
    {
        public SFXEvent SFX_Event = SFXEvent.NoEvent;
        public AudioClip clip;
        private AudioSource source;

        public AudioSource Source { get { return source; } set { source = value; } }
    }

    public enum AudioType
    {
        None,
        SFX_01,
        BTN_01,
        SoundBackground_01
    }

    public enum SFXEvent
    {
        NoEvent,
        ButtonClick,
        ButtonHover,
        MapClose,
        MapOpen,
        BackInMainMenu
    }
}

