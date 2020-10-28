using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCreater : MonoBehaviour
{
    private EventManager eventManager;
    private ResourceSpriteLoader resourceSpriteLoader;

    void Awake()
    {
        EventXmlLoader xmlLoader = gameObject.GetComponent<EventXmlLoader>();
        List<StatsClass> createdEvents = new List<StatsClass>();
        this.eventManager = this.GetComponent<EventManager>();
        resourceSpriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        createdEvents = xmlLoader.GetEventsFromXML();



        PassMissionToManager(createdEvents);
    }

    private void PassMissionToManager(List<StatsClass> createdEvents)
    {
        this.eventManager.allEvents = createdEvents;
    }

}
