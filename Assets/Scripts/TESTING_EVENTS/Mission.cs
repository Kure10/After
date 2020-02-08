using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class Mission
{
    public string missionName;

    public float missionDistance; // vzdalenost je tam ale ne zpatky

    public Sprite image;

   // public string missionMission;

    public string missionType;

    MissionTime missionTime; // kolik eventu te potka

    LevelOfDangerous levelOfDangerous; // jak jsou tezke eventy // i finalni event

    MissionEnviroment missionEnviroment; // filtr pro nahodny víběr eventu....

    // List Eventu .....

    public List<Specialists> specialistOnMission = new List<Specialists>();

    public Mission ()
    {
        missionTime = MissionTime.akorat;
        levelOfDangerous = LevelOfDangerous.dva;
        missionEnviroment = MissionEnviroment.poust;
    }

}

enum MissionTime { malo, akorat, stredne, hodne }; // tohle se zmeni

enum LevelOfDangerous { jedna, dva, tri };

enum MissionEnviroment { pole, poust, dzungle, les };
