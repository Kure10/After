using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Mission
{
    #region Fields

    private long id;

    private string _name;

    private string description;

    private float distance;

    private Sprite image;

    private MissionType type;

    private int eventsMin;

    private int eventsMax;

    private bool repeat;

    private float repeatableIn;

    private int specMin;

    private int specMax;

    private string missionPointer;

    private string neededTransport; // možna to neni transport co potřebuji ale jaky se na ten event da použit jako bonus.. nevim

    private int finalEventId;

    private float initialDistance;

    private List<DirectEvents> directEvents = new List<DirectEvents>();

    private MissionTime misTime; // kolik eventu te potka

    private LevelOfDangerous levelOfDangerous; // jak jsou tezke eventy // i finalni event

    private List<Terrain> emergingTerrains = new List<Terrain>(); // filtr pro nahodny víběr eventu....

    private List<EventBlueprint> eventsInMission = new List<EventBlueprint>();

    private List<Specialists> specialistOnMission = new List<Specialists>();

    private RegionOperator currentRegionOperator;

    private MapField mapField;

    #endregion

    #region Constructor
    public Mission()
    {
        misTime = MissionTime.akorat;
        levelOfDangerous = LevelOfDangerous.dva;
    }

    #endregion

    #region PublicFields

    public float RepeatableTime = 0;

    public bool WasSuccessfullyExecuted = false;

    #endregion

    #region Properitiers

    public RegionOperator RegionOperator { get { return this.currentRegionOperator; } set { this.currentRegionOperator = value; } }

    public long Id { get { return this.id; } set { this.id = value; } }

    public string Name { get { return this._name; } set { this._name = value; } }

    public string MissionPointer { get { return this.missionPointer; } set { this.missionPointer = value; } }

    public string Description { get { return this.description; } set { this.description = value; } }

    public float Distance { get { return this.distance; } set { this.distance = value; } }

    public float InitialDistance { get { return this.initialDistance; } set { this.initialDistance = value; } }

    public Sprite Image { get { return this.image; } set { this.image = value; } }

    public MissionType Type { get { return this.type; } set { this.type = value; } }

    public int EventsMin { get { return this.eventsMin; } set { this.eventsMin = value; } }
    public int EventsMax { get { return this.eventsMax; } set { this.eventsMax = value; } }

    public List<EventBlueprint> GetEventsInMission { get { return this.eventsInMission; } }

    public List<Specialists> SpecialistOnMission { get { return this.specialistOnMission; } }

    public bool Repeate { get { return this.repeat; } set { this.repeat = value; } }

    public float RepeatableIn { get { return this.repeatableIn; } set { this.repeatableIn = value; } }

    public int SpecMin { get { return this.specMin; } set { this.specMin = value; } }

    public int SpecMax { get { return this.specMax; } set { this.specMax = value; } }

    public string NeededTransport { get { return this.neededTransport; } set { this.neededTransport = value; } }

    public List<DirectEvents> GetdirectEvents { get { return this.directEvents; } }

    public int FinalEventID { get { return this.finalEventId; } set { this.finalEventId = value; } }

    public MissionTime MissionTime { get { return this.misTime; } set { this.misTime = value; } }
    public LevelOfDangerous LevelOfDangerous { get { return this.levelOfDangerous; } set { this.levelOfDangerous = value; } }
    public List<Terrain> GetEmergingTerrains { get { return this.emergingTerrains; } }
    public MapField MapField { get { return this.mapField; } set { this.mapField = value; } }
    #endregion

    #region Methods
    public void AddEventInMissions(EventBlueprint _event)
    {
        this.eventsInMission.Add(_event);
    }

    public void AddSpecialistToMission(List<Specialists> spec)
    {
        this.specialistOnMission.AddRange(spec);
    }

    public void AddDirectEvent(DirectEvents dirEvent)
    {
        directEvents.Add(dirEvent);
    }

    public void AddTerrain(Terrain terrain)
    {
        emergingTerrains.Add(terrain);
    }


    #endregion

    #region Helpers

    public Terrain ConvertTerrainStringData(string data)
    {
        switch (data)
        {
            case "Pole":
                return Terrain.pole;
            case "Poust":
                return Terrain.poust;
            case "Dzungle":
                return Terrain.dzungle;
            case "Lesy":
                return Terrain.les;
            case "Louky":
                return Terrain.louky;
            default:
                return Terrain.unknow;
        }
    }
    public MapField ConvertMapFieldStringData(string data)
    {
        switch (data)
        {
            case "Udoly":
                return MapField.udoly;
            case "Peklo":
                return MapField.peklo;
            case "Ring":
                return MapField.ring;
            case "Ctverec":
                return MapField.ctverec;
            case "Garaz":
                return MapField.garaz;
            default:
                return MapField.none;
        }
    }

    public MissionType ConvertMissionTypeStringData(string data)
    {
        switch (data)
        {
            case "PruzObl":
                return MissionType.pruzkum_oblasti;
            case "Pruzkum":
                return MissionType.pruzkum;
            case "SberLov":
                return MissionType.sberLov;
            case "skavender":
                return MissionType.skavender;
            case "zachrana":
                return MissionType.zachrana;
            case "zajmutiNempritele":
                return MissionType.zajmutiNempritele;
            case "odlakaniHordy":
                return MissionType.odlakaniHordy;
            default:
                return MissionType.neznamyCil;
        }
    }

    public string ConvertMissionTypeStringData(MissionType type)
        {
            switch (type)
            {
                case MissionType.pruzkum_oblasti:
                    return "pruzkum_oblasti";
                case MissionType.pruzkum:
                    return "pruzkum";
                case MissionType.sberLov:
                    return "sberLov";
                case MissionType.skavender:
                    return "skavender";
                case MissionType.zachrana:
                    return "zachrana";
                case MissionType.zajmutiNempritele:
                    return "zajmutiNempritele";
                case MissionType.odlakaniHordy:
                    return "odlakaniHordy";
                default:
                    return "Unknow";
            }
        }

    #endregion

}


#region Enum 
public enum MissionTime { malo, akorat, stredne, hodne }; // tohle se zmeni

public enum LevelOfDangerous { jedna = 1, dva = 2 , tri = 3 };

public enum Terrain { pole , poust , dzungle, les , louky, unknow };

public enum MapField { ring, ctverec, garaz, udoly, peklo , none };

public enum MissionType { pruzkum_oblasti, pruzkum, sberLov, skavender, zachrana, zajmutiNempritele, odlakaniHordy, neznamyCil };

#endregion

#region Helpers Class 
public class DirectEvents
{
    int eventID;

    int order;
}

#endregion