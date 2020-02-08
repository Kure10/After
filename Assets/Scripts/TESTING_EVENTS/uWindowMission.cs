using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class uWindowMission : MonoBehaviour
{


    private string _missionName;

    private Sprite sprite;

    private string missionType;

    private float missionDistance;

    private LevelOfDangerous missionLevel;

    private MissionEnviroment missionEnviroment;

    private MissionTime missionTime;

    #region Properties

    public string MissionName
    {
        get { return _missionName; }
        set { _missionName = value; }
    }

    #endregion

}
