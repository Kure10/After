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

}
