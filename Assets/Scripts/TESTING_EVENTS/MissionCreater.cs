using ResolveMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCreater : MonoBehaviour
{
    [SerializeField]
    public List<Mission> createdMissions = new List<Mission>();
    private MissionManager missionManager;
    private ResourceLoader resourceLoader;

    List<StatsClass> XMLLoadedMissions = new List<StatsClass>();
    List<StatsClass> XMLAdditionalMissionsInformation = new List<StatsClass>();

    public Sprite image;

    /*  in percent */
    [SerializeField] float timeUntilFirstEvent = 7f;
    [SerializeField] float timeBetweenEvents = 5f;
    [SerializeField] float timeBetweenLastEvent = 3f;

    private void Awake()
    {
        this.missionManager =  this.GetComponent<MissionManager>();
        this.resourceLoader = this.GetComponent<ResourceLoader>();

        LoadMissionsFromXML();


        // Tady musim z mojich listu premenit data na misse...
        for (int i = 0; i < 10; i++)
        {
            createdMissions.Add(CreateMission(i));
        }

        // pokus 
        foreach (var item in XMLLoadedMissions)
        {
            Mission mis = new Mission();

        }
        // end

        PassMissionList();
    }

    public Mission CreateMission(int i)
    {
        Mission mis = new Mission();


        mis.Id = i;
        mis.Name = "Explore";
        mis.Distance = 100f;
        mis.Image = image;
        mis.MaxNumberOfEvents = 5;
        mis.Type = "Typerino : " + i.ToString();

        CreateEvents(mis);

       // DeterminateEventTimesInMission(mis);

        return mis;
    }

    public void PassMissionList()
    {
        this.missionManager.allMissions = this.createdMissions;
    }

    public void LoadMissionsFromXML()
    {
        string path = "C:/Unity Games/After/after/Assets/Data/XML/Testing Mission Data";
        string fileName = "Missions";
        string fileNameCZ = "Missions-CZ";
        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        resolveMaster.AddDataNode(fileNameCZ, secondData);

        XMLLoadedMissions = resolveMaster.GetDataKeys(fileName);
        XMLAdditionalMissionsInformation = resolveMaster.GetDataKeys(fileNameCZ);

        //List<ResolveSlave> slave = new List<ResolveSlave>();
        
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
    }

    private void CreateEvents(Mission mis)
    {
        /* Event set trigger Time*/
        int amountEvents = mis.MaxNumberOfEvents;
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
            item.sprite = resourceLoader.FindEventResource("smile.jpg"); // Todo je tu cela cesta name.jpg  - mozna by to slo udelat bet .jpg
        }
    }

    private float SetEventTimeInMission(Mission mis, float firstOccurrenceEvent, float eventOccurrenceRange)
    {
        for (int i = 0; i < mis.MaxNumberOfEvents; i++)
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


}
