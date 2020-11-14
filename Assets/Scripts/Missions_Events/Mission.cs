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

    private MissionType type; /* Todo Informace pro hrace -> definuje final event.
                                Vlastne je to jenom pro hrace aby vedel co ma ocekavat od misee */

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

    private int minDifficulty; // obtiznost misse -> z toho se odvozuje obtiznost eventu.

    private int maxDifficulty;

    private List<DirectEvents> directEvents = new List<DirectEvents>();

    private List<Terrain> emergingTerrains = new List<Terrain>(); // Filtr pro obevujícíse eventy na misi 

    private List<EventContent> eventsContents = new List<EventContent>();

    private List<Character> charactersOnMission = new List<Character>();

    private RegionOperator currentRegionOperator;

    #endregion

    #region Constructor
    public Mission()
    {
        
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

    public int DifficultyMin { get { return this.minDifficulty; } set { this.minDifficulty = value; } }

    public int DifficultyMax { get { return this.maxDifficulty; } set { this.maxDifficulty = value; } }

    // public List<EventBlueprint> GetEventsInMission { get { return this.eventsInMission; } }

    public List<EventContent> GetEventsContent { get { return this.eventsContents; } }
    public List<Character> GetCharactersOnMission { get { return this.charactersOnMission; } }

    public bool Repeate { get { return this.repeat; } set { this.repeat = value; } }

    public float RepeatableIn { get { return this.repeatableIn; } set { this.repeatableIn = value; } }

    public int SpecMin { get { return this.specMin; } set { this.specMin = value; } }

    public int SpecMax { get { return this.specMax; } set { this.specMax = value; } }

    public string NeededTransport { get { return this.neededTransport; } set { this.neededTransport = value; } }

    public List<DirectEvents> GetdirectEvents { get { return this.directEvents; } }

    public int FinalEventID { get { return this.finalEventId; } set { this.finalEventId = value; } }

    public List<Terrain> GetEmergingTerrains { get { return this.emergingTerrains; } }

    #endregion

    #region Methods
    //public void AddEventInMissions(EventBlueprint _event)
    //{
    //    this.eventsInMission.Add(_event);
    //}

    public void AddNewEventContent(EventContent _eventContent)
    {
        this.eventsContents.Add(_eventContent);
    }

    public void AddSpecialistToMission(List<Character> character)
    {
        this.charactersOnMission.AddRange(character);
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
            case "Hory":
                return Terrain.Hory;
            case "Lesy":
                return Terrain.Lesy;
            case "Louky":
                return Terrain.Louky;
            case "MalaMesta":
                return Terrain.MalaMesta;
            case "VelkaMEsta":
                return Terrain.VelkaMesta;
            case "Vesnice":
                return Terrain.Vesnice;
            default:
                return Terrain.Unknow;
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

public enum Terrain { Hory, Lesy, Louky, MalaMesta, VelkaMesta, Vesnice, Unknow };

/* info pro hrace -> co ma ocekavat od misse.*/
public enum MissionType { pruzkum_oblasti, pruzkum, sberLov, skavender, zachrana, zajmutiNempritele, odlakaniHordy, neznamyCil };

#endregion

#region Helpers Class 
public class DirectEvents
{
    int eventID;

    int order;
}

#endregion