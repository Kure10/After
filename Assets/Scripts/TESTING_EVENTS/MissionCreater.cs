using ResolveMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MissionCreater : MonoBehaviour
{
    // Je SerializeField for testing
    [SerializeField]
    public List<Mission> exploreMissions = new List<Mission>();
    [SerializeField]
    public List<Mission> otherMissions = new List<Mission>();
    private MissionManager missionManager;
    private ResourceSpriteLoader resourceSpriteLoader;

    public Sprite image;

    /*  in percent */
    [SerializeField] float timeUntilFirstEvent = 7f;
    [SerializeField] float timeBetweenEvents = 5f;
    [SerializeField] float timeBetweenLastEvent = 3f;

    private void Awake()
    {
        List<Mission> createdMissions = new List<Mission>();
        this.missionManager = this.GetComponent<MissionManager>();
        this.resourceSpriteLoader = this.GetComponent<ResourceSpriteLoader>();

        createdMissions = LoadMissionsFromXML();

        SortMissions(createdMissions);

        AddEventsToMissions(exploreMissions);
        AddEventsToMissions(otherMissions);

        PassMissionToManager();
    }

    private void AddEventsToMissions(List<Mission> createdMissions)
    {
        foreach (Mission mission in createdMissions)
        {
            AddEventsToMission(mission);
        }
    }

    public void SortMissions(List<Mission> createdMissions)
    {
        foreach (Mission item in createdMissions)
        {
            if(item.Type == MissionType.pruzkum)
                exploreMissions.Add(item);
            else
                otherMissions.Add(item);
        }
    }

    public void PassMissionToManager()
    {
        this.missionManager.exploreMissions = this.exploreMissions;
        this.missionManager.othersMissions = this.otherMissions;
    }

    public List<Mission> LoadMissionsFromXML()
    {
        List<StatsClass> XMLLoadedMissions = new List<StatsClass>();
        List<StatsClass> XMLAdditionalMissionsInformation = new List<StatsClass>();
        List<Mission> allMissions = new List<Mission>();

        string path = "Assets/Data/XML/Testing Mission Data";
        string fileName = "Missions";
        string fileNameCZ = "Missions-CZ";
        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        resolveMaster.AddDataNode(fileNameCZ, secondData);

        XMLLoadedMissions = resolveMaster.GetDataKeys(fileName);
        XMLAdditionalMissionsInformation = resolveMaster.GetDataKeys(fileNameCZ);


        allMissions = DeSerializedMission(XMLLoadedMissions, XMLAdditionalMissionsInformation);

        // chyby jeste direct a final event...   dodelat...

        //for (int i = 0; i < AllLoadedMissions.Count; i++)
        //{
        //    slave.Add(resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[i].Title));
        //}

        //for (int i = 0; i < AllLoadedMissions.Count; i++)
        //{
        //    slave.Add(resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[i].Title));
        //}

        //
        ResolveSlave slave = resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[0].Title);
        //slave = resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[1].Title);
        slave.StartResolve();
        var output = slave.Resolve();

        return allMissions;
    }

    private void AddEventsToMission(Mission mis)
    {
        /* Event set trigger Time*/
        int amountEvents = Random.Range(mis.EventsMin, mis.EventsMax + 1);
        float distance = mis.Distance;
        float firstOccurrenceEvent = distance * ((100 - timeUntilFirstEvent - timeBetweenLastEvent) / 100);
        float eventOccurrenceRange = firstOccurrenceEvent / amountEvents;
        eventOccurrenceRange = eventOccurrenceRange - timeBetweenEvents;

        firstOccurrenceEvent = SetEventTimeInMission(mis, firstOccurrenceEvent, eventOccurrenceRange);

        /* Event Image*/

        SetEventsImage(mis);

        /* Event text answers for buttons */

        foreach (EventBlueprint item in mis.GetEventsInMission)
        {
            item.hasAvoidButton = true; // testing
            /*checknout jestli funguje..*/
            item.numberOfOptions = 1; // todo spatne..

            for (int i = 0; i < item.answerTextField.Length; i++)
            {
                item.answerTextField[i] = "Answer number: " + i;  // tady se vyplni odpovedi na kazdy button.. ToDO dodelat až budu mit zkama vyplnit..
            }
        }

    }

    private void SetEventsImage(Mission mis)
    {
        foreach (var item in mis.GetEventsInMission)
        {
            item.sprite = this.resourceSpriteLoader.FindEventResource("smile.jpg"); // Todo je tu cela cesta name.jpg  - mozna by to slo udelat bet .jpg
        }
    }

    private float SetEventTimeInMission(Mission mis, float firstOccurrenceEvent, float eventOccurrenceRange)
    {
        for (int i = 0; i < mis.EventsMax; i++)
        {
            EventBlueprint newEvent = new EventBlueprint();

            float secondOccurrenceEvent = firstOccurrenceEvent - eventOccurrenceRange;
            float currentEventOccurrenceTime = Random.Range(firstOccurrenceEvent, secondOccurrenceEvent);
            newEvent.evocationTime = (int)currentEventOccurrenceTime;
            firstOccurrenceEvent = secondOccurrenceEvent - timeBetweenEvents;

            mis.AddEventInMissions(newEvent);
        }

        return firstOccurrenceEvent;
    }

    public List<Mission> DeSerializedMission(List<StatsClass> firstStatClass , List<StatsClass> secondStatClass)
    {
        List<Mission> allMissions = new List<Mission>();

        foreach (StatsClass item in firstStatClass)
        {
            Mission newMission = new Mission(); //Take care Construktor is not empty..

            bool success = long.TryParse(item.Title, out long idNumber);
            if(!success)
                Debug.LogError("Some mission has Error while DeSerialized id: " + item.Title);
               // mozna poptremyslet o nejakem zalozním planu když se neco posere..
               // napriklad generovani nejake generické mise.. Nebo tak neco..

            newMission.Id = idNumber;
            newMission.Repeate = item.GetIntStat("Repeat");
            newMission.LevelOfDangerous = (LevelOfDangerous)item.GetIntStat("Difficulty");
            string terrains = item.GetStrStat("Terrain");
            List<string> result = terrains.Split(',').ToList();
            foreach (var str in result)
            {
                Terrain terrain = newMission.ConvertTerrainStringData(str);
                newMission.AddTerrain(terrain);
            }
            newMission.Distance = item.GetIntStat("Time"); // delka mise
            newMission.SpecMin = item.GetIntStat("SpecMin");
            newMission.SpecMax = item.GetIntStat("SpecMax");
            newMission.NeededTransport = item.GetStrStat("Transport");
            newMission.EventsMin = item.GetIntStat("EventsMin");
            newMission.EventsMax = item.GetIntStat("EventsMax");
            newMission.RepeatableIn = item.GetIntStat("IsRepeatable");

            // for data which can be translated
            foreach (StatsClass secondItem in secondStatClass)
            {
                if (item.Title == secondItem.Title)
                {
                    newMission.Description = secondItem.GetStrStat("Description");
                    newMission.Name = secondItem.GetStrStat("Name");
                    var misType = secondItem.GetStrStat("Type");
                    newMission.Type = newMission.ConvertMissionTypeStringData(misType);
                    string mapField = secondItem.GetStrStat("MapField");
                    newMission.MapField = newMission.ConvertMapFieldStringData(mapField); // mise se vyskuje na teto mape..
                }
            }

            newMission.Image = image; // Todo dodelat .. Image se bude brat s nejakeho poolu..

            allMissions.Add(newMission);
        }

        return allMissions;
    }

}
