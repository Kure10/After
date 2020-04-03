using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCreater : MonoBehaviour
{
    [SerializeField]
    public List<Mission> createdMissions = new List<Mission>();
    private MissionManager missionManager;

    public Sprite image;


    private void Awake()
    {
        this.missionManager =  this.GetComponent<MissionManager>();

        for (int i = 0; i < 10; i++)
        {
            createdMissions.Add(CreateMission(i));
        }
        PassMissionList();
    }

    public Mission CreateMission(int i)
    {
        Mission mis = new Mission();

        mis.missionID = i;
        mis.missionName = "Explore";
        mis.missionDistance = 100f;
        mis.image = image;
        mis.maxNumberOfEvents = 5;
        mis.missionType = "Typerino : " + i.ToString(); ;

        DeterminateEventTimesInMission(mis);

        return mis;
    }

    public void PassMissionList()
    {
        this.missionManager.allMissions = this.createdMissions;
    }

    private void DeterminateEventTimesInMission(Mission mis)
    {
        /*  in percent */
        float timeUntilFirstMission = 7f;
        float timeBetweenMissions = 5f;
        float timeBetweenLastMission = 3f;

        var amountEvents = mis.maxNumberOfEvents;
        var distance = mis.missionDistance;
        mis.eventEvocationTimes = new int[amountEvents];

        var firstOccurrenceEvent = distance * ((100 - timeUntilFirstMission - timeBetweenLastMission) / 100);
        var eventOccurrenceRange = firstOccurrenceEvent / amountEvents;
        eventOccurrenceRange = eventOccurrenceRange - timeBetweenMissions;

        for (int i = 0; i < amountEvents; i++)
        {
           
            var secondOccurrenceEvent = firstOccurrenceEvent - eventOccurrenceRange;
            var currentEventOccurrenceTime = Random.Range(firstOccurrenceEvent, secondOccurrenceEvent);
           // Debug.Log("EventPointy " + i + " range: " + (int)firstOccurrenceEvent + "// " + (int)secondOccurrenceEvent);
            mis.eventEvocationTimes[i] = (int)currentEventOccurrenceTime;
            firstOccurrenceEvent = secondOccurrenceEvent - timeBetweenMissions;
        }

    }

}
