using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class Mission
{
    #region Fields

    private int id;

    private string _name;

    private float distance;

    private Sprite image;

    private int maxNumberOfEvents;

    private string type;

    private int eventsMin;

    private int eventsMax;

    private int repeat;

    private int repeatableIn;

    private int time;

    private int specMin;

    private int specMax;

    private string neededTransport; // možna to neni transport co potřebuji ale jaky se na ten event da použit jako bonus.. nevim

    private int finalEventId;

    private List<DirectEvents> directEvents = new List<DirectEvents>();

    private MissionTime missionTime; // kolik eventu te potka

    private LevelOfDangerous levelOfDangerous; // jak jsou tezke eventy // i finalni event

    private MissionEnviroment missionEnviroment; // filtr pro nahodny víběr eventu....

    private List<EventBlueprint> eventsInMission = new List<EventBlueprint>();

    private List<Specialists> specialistOnMission = new List<Specialists>();

    private RegionOperator currentRegionOperator;

    #endregion

    #region Constructor
    public Mission()
    {
        missionTime = MissionTime.akorat;
        levelOfDangerous = LevelOfDangerous.dva;
        missionEnviroment = MissionEnviroment.poust;
    }

    #endregion

    #region Properitiers

    public RegionOperator RegionOperator { get { return this.currentRegionOperator; } set { this.currentRegionOperator = value; } }

    public int Id { get { return this.id; } set { this.id = value; } }

    public string Name { get { return this._name; } set { this._name = value; } }

    public float Distance { get { return this.distance; } set { this.distance = value; } }

    public Sprite Image { get { return this.image; } set { this.image = value; } }

    public int MaxNumberOfEvents { get { return this.maxNumberOfEvents; } set { this.maxNumberOfEvents = value; } }

    public string Type { get { return this.type; } set { this.type = value; } }

    public int EventsMin { get { return this.eventsMin; } set { this.eventsMin = value; } }
    public int EventsMax { get { return this.eventsMax; } set { this.eventsMax = value; } }

    public List<EventBlueprint> GetEventsInMission { get { return this.eventsInMission; } }

    public List<Specialists> GetspecialistOnMission { get { return this.specialistOnMission; } }

    public int Repeate { get { return this.repeat; } set { this.repeat = value; } }

    public int RepeatableIn { get { return this.repeatableIn; } set { this.repeatableIn = value; } }

    public int Time { get { return this.time; } set { this.time = value; } }

    public int SpecMin { get { return this.specMin; } set { this.specMin = value; } }

    public int SpecMax { get { return this.specMax; } set { this.specMax = value; } }

    public string NeededTransport { get { return this.neededTransport; } set { this.neededTransport = value; } }

    public List<DirectEvents> GetdirectEvents { get { return this.directEvents; } }

    public int FinalEventID { get { return this.finalEventId; } set { this.finalEventId = value; } }

    public MissionTime GetMissionTime { get { return this.missionTime; } }
    public LevelOfDangerous GetLevelOfDangerous { get { return this.levelOfDangerous; } }
    public MissionEnviroment GetMissionEnviroment { get { return this.missionEnviroment; } }

    #endregion

    #region Methods
    public void AddEventInMissions(EventBlueprint _event)
    {
        this.eventsInMission.Add(_event);
    }

    public void AddEventInMissions(Specialists spec)
    {
        this.specialistOnMission.Add(spec);
    }

    public void AddDirectEvent(DirectEvents dirEvent)
    {
        directEvents.Add(dirEvent);
    }

    #endregion

}


#region Enum 
public enum MissionTime { malo, akorat, stredne, hodne }; // tohle se zmeni

public enum LevelOfDangerous { jedna, dva, tri };

public enum MissionEnviroment { pole, poust, dzungle, les };

#endregion

#region Helpers Class 
public class DirectEvents
{
    int eventID;

    int order;
}

#endregion