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
        mis.type = "Typerino : " + i.ToString(); ;

        CreateEvents(mis);

       // DeterminateEventTimesInMission(mis);

        return mis;
    }

    public void PassMissionList()
    {
        this.missionManager.allMissions = this.createdMissions;
    }

    private void CreateEvents(Mission mis)
    {
        /* Event */
        int amountEvents = mis.maxNumberOfEvents;
        float distance = mis.distance;
        float firstOccurrenceEvent = distance * ((100 - timeUntilFirstEvent - timeBetweenLastEvent) / 100);
        float eventOccurrenceRange = firstOccurrenceEvent / amountEvents;
        eventOccurrenceRange = eventOccurrenceRange - timeBetweenEvents;

        firstOccurrenceEvent = SetEventTimeInMission(mis, firstOccurrenceEvent, eventOccurrenceRange);

        /* Event Image*/

        SetEventsImage(mis);

    }

    private void SetEventsImage(Mission mis)
    {
        foreach (var item in mis.posibleEvents)
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

            mis.posibleEvents.Add(newEvent);
        }

        return firstOccurrenceEvent;
    }

    

}
