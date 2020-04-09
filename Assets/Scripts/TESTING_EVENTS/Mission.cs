using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class Mission
{
    public int id;

    public string _name;

    public float distance; 

    public Sprite image;

    //public int[] eventEvocationTimes;

    public int maxNumberOfEvents;

    public string type;

    public MissionTime missionTime; // kolik eventu te potka

    public LevelOfDangerous levelOfDangerous; // jak jsou tezke eventy // i finalni event

    public MissionEnviroment missionEnviroment; // filtr pro nahodny víběr eventu....

    public List<EventBlueprint> posibleEvents = new List<EventBlueprint>();

    public List<Specialists> specialistOnMission = new List<Specialists>();

    public Mission ()
    {
        missionTime = MissionTime.akorat;
        levelOfDangerous = LevelOfDangerous.dva;
        missionEnviroment = MissionEnviroment.poust;
    }

}

public enum MissionTime { malo, akorat, stredne, hodne }; // tohle se zmeni

public enum LevelOfDangerous { jedna, dva, tri };

public enum MissionEnviroment { pole, poust, dzungle, les };
