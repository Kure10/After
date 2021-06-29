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
        MissionXmlLoader xmlLoader = gameObject.GetComponent<MissionXmlLoader>();
        List<Mission> createdMissions = new List<Mission>();
        this.missionManager = this.GetComponent<MissionManager>(); 
        resourceSpriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        createdMissions = xmlLoader.GetMissionsFromXML();

        SortMissions(createdMissions);

        PassMissionToManager();
    }

    private void SortMissions(List<Mission> createdMissions)
    {
        foreach (Mission item in createdMissions)
        {
            if(item.Type == MissionType.pruzkum_oblasti)
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

    public void AddEventsToMission(Mission mis)
    {
        /* Event set trigger Time*/
        int amountEvents = Random.Range(mis.EventsMin, mis.EventsMax + 1);
        float distance = mis.Distance;
        float firstOccurrenceEvent = distance * ((100 - timeUntilFirstEvent - timeBetweenLastEvent) / 100);
        float eventOccurrenceRange = firstOccurrenceEvent / amountEvents;
        eventOccurrenceRange = eventOccurrenceRange - timeBetweenEvents;

        firstOccurrenceEvent = SetEventTimeInMission(mis, firstOccurrenceEvent, eventOccurrenceRange);
    }

    private float SetEventTimeInMission(Mission mis, float firstOccurrenceEvent, float eventOccurrenceRange)
    {
        for (int i = 0; i < mis.EventsMax; i++)
        {
            EventContent newContent = new EventContent();

            float secondOccurrenceEvent = firstOccurrenceEvent - eventOccurrenceRange;
            float currentEventOccurrenceTime = Random.Range(firstOccurrenceEvent, secondOccurrenceEvent);
            newContent.evocationTime = (int)currentEventOccurrenceTime;
            firstOccurrenceEvent = secondOccurrenceEvent - timeBetweenEvents;

            mis.AddNewEventContent(newContent);
        }

        return firstOccurrenceEvent;
    }

}
