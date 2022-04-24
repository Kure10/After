using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class EnviromentPlayer : MonoBehaviour
{

    public enum EnviromentTypes
    {
        forest,
        night,
        rain
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayNewEnviromentAudio(EnviromentTypes _type)
    {
        Audio.AudioType audioType = Audio.AudioType.SoundEnviroment_01;

        switch (_type)
        {
            case EnviromentTypes.forest:
                audioType = Audio.AudioType.SoundEnviroment_01;
                break;
            case EnviromentTypes.night:
                break;
            case EnviromentTypes.rain:
                break;
            default:
                break;
        }

        AudioManager.instance.PlayAudio(audioType, true);
    }
}
