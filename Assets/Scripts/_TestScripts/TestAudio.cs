using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class TestAudio : MonoBehaviour
    {
        public AudioManager audioController;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                audioController.PlayAudio(AudioType.SoundBackground_01, true);
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                audioController.StopAudio(AudioType.SoundBackground_01, true);
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                audioController.RestartAudio(AudioType.SoundBackground_01, true);
            }
            if (Input.GetKeyUp(KeyCode.Y))
            {
                audioController.PlayAudio(AudioType.SFX_01, true);
            }
            if (Input.GetKeyUp(KeyCode.H))
            {
                audioController.StopAudio(AudioType.SFX_01, true);
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                audioController.RestartAudio(AudioType.SFX_01, true);
            }
        }
    }
}

