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

    public Sprite image;

    /*  in percent */
    [SerializeField] float timeUntilFirstEvent = 7f;
    [SerializeField] float timeBetweenEvents = 5f;
    [SerializeField] float timeBetweenLastEvent = 3f;

    private void Awake()
    {
        this.missionManager =  this.GetComponent<MissionManager>();
        this.resourceLoader = this.GetComponent<ResourceLoader>();

        LoadMissions();

        for (int i = 0; i < 10; i++)
        {
            createdMissions.Add(CreateMission(i));
        }
        PassMissionList();
    }

    public Mission CreateMission(int i)
    {
        Mission mis = new Mission();


        mis.id = i;
        mis._name = "Explore";
        mis.distance = 100f;
        mis.image = image;
        mis.maxNumberOfEvents = 5;
        mis.type = "Typerino : " + i.ToString();

        CreateEvents(mis);

       // DeterminateEventTimesInMission(mis);

        return mis;
    }

    public void PassMissionList()
    {
        this.missionManager.allMissions = this.createdMissions;
    }

    public void LoadMissions()
    {
       // string path = "Assets/Data/XML/Testing Mission Data/Missions.xml";
        string path = "C:/Unity Games/After/after/Assets/Data/XML/Testing Mission Data";
        string fileName = "Missions";
        string fileNameCZ = "Missions-CZ";
        ResolveMaster rm = new ResolveMaster();


        var data2 = StatsClass.LoadXmlFile(path, fileName);
        rm.AddDataNode(fileName, data2);
        

        var dataloc2 = StatsClass.LoadXmlFile(path, fileNameCZ);
        rm.ModifyDataNode(fileName, dataloc2);
        foreach (var key in rm.GetDataKeys(fileName)) Debug.LogWarning(key.ToLog());

        var tmp = rm.GetDataKeys("Missions");

        ResolveSlave slave = rm.AddDataSlave("Missions", rm.GetDataKeys("Missions")[0].Title);

        slave.StartResolve();
        var output = slave.Resolve();
    }

    private void CreateEvents(Mission mis)
    {
        /* Event set trigger Time*/
        int amountEvents = mis.maxNumberOfEvents;
        float distance = mis.distance;
        float firstOccurrenceEvent = distance * ((100 - timeUntilFirstEvent - timeBetweenLastEvent) / 100);
        float eventOccurrenceRange = firstOccurrenceEvent / amountEvents;
        eventOccurrenceRange = eventOccurrenceRange - timeBetweenEvents;

        firstOccurrenceEvent = SetEventTimeInMission(mis, firstOccurrenceEvent, eventOccurrenceRange);

        /* Event Image*/

        SetEventsImage(mis);

        /* Event text answers for buttons */

        foreach (EventBlueprint item in mis.eventsInMission)
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
        foreach (var item in mis.eventsInMission)
        {
            item.sprite = resourceLoader.FindEventResource("smile.jpg"); // Todo je tu cela cesta name.jpg  - mozna by to slo udelat bet .jpg
        }
    }

    private float SetEventTimeInMission(Mission mis, float firstOccurrenceEvent, float eventOccurrenceRange)
    {
        for (int i = 0; i < mis.maxNumberOfEvents; i++)
        {
            EventBlueprint newEvent = new EventBlueprint();

            float secondOccurrenceEvent = firstOccurrenceEvent - eventOccurrenceRange;
            float currentEventOccurrenceTime = Random.Range(firstOccurrenceEvent, secondOccurrenceEvent);
            newEvent.evocationTime = (int)currentEventOccurrenceTime;
            firstOccurrenceEvent = secondOccurrenceEvent - timeBetweenEvents;

            mis.eventsInMission.Add(newEvent);
        }

        return firstOccurrenceEvent;
    }


}
