using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class Mission
{
    private string missionName;

    private float missionDistance; // vzdalenost je tam ale ne zpatky

    public Image image;

    public string missionMission;

    public string missionType;

    MissionTime missionTime; // kolik eventu te potka

    MissionLevel missionLevel; // jak jsou tezke eventy // i finalni event

    MissionEnviroment missionEnviroment; // filtr pro nahodny víběr eventu....

    // List Eventu .....

    List<Specialists> specialistOnMission = new List<Specialists>();

    public Mission ()
    {

    }

}

enum MissionTime { malo, akorat, stredne, hodne }; // tohle se zmeni

enum MissionLevel { jedna, dva, tri };

enum MissionEnviroment { pole, poust, dzungle, les };
