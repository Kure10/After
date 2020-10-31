using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;

public class EventCreater : MonoBehaviour
{
    private EventManager eventManager;

    internal ResolveMaster resolveMaster = new ResolveMaster();

    void Awake()
    {
        
        EventXmlLoader xmlLoader = gameObject.GetComponent<EventXmlLoader>();
        List<StatsClass> createdEvents = new List<StatsClass>();
        this.eventManager = this.GetComponent<EventManager>();
        

        createdEvents = xmlLoader.GetEventsFromXML();

        PassMissionToManager(createdEvents);
    }

    private void PassMissionToManager(List<StatsClass> createdEvents)
    {
        this.eventManager.allEvents = createdEvents;
    }

}
