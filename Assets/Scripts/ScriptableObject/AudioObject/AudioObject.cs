using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    [CreateAssetMenu(menuName = "ScriptableObject/AudioObject", fileName = "_NewAudio")]
    public class AudioObject : ScriptableObject
    {
        public AudioType type;
        public AudioOption audioOption;
    }
}


