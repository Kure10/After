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
                AudioManager.instance.PlayAudio(Audio.AudioType.BTN_01, true , SFX_Event:SFXEvent.ButtonClick);
            }
            if (Input.GetKeyUp(KeyCode.H))
            {
                audioController.StopAudio(Audio.AudioType.BTN_01, true);
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                audioController.RestartAudio(AudioType.BTN_01, true);
            }
        }
    }
}

