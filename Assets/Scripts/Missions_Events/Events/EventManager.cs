using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public List<StatsClass> allEvents = new List<StatsClass>();

    internal ResolveMaster resolveMaster = new ResolveMaster();

    void Awake()
    {
        EventXmlLoader xmlLoader = gameObject.GetComponent<EventXmlLoader>();

        allEvents = xmlLoader.GetEventsFromXML();

    }

    public StatsClass ChoiseRandomEvent(int minDifficulty, int maxDifficulty, List<Terrain> occurringTerrains)
    {
        // tady vyberu random event podle parametru..

        // ted vybíram jeden porad..

        long id = 20203890660776; // potulna yombie
       // id = 20203956336369; // kralici 

        StatsClass statclass = new StatsClass();

        foreach (var item in allEvents)
        {
            if (item.Title == id.ToString())
            {
                statclass = item;
            }
        }

        return statclass;
    }

    public StatsClass ChoiseFinalEvent(long finalEventID)
    {
        StatsClass statclass = new StatsClass();
        foreach (var item in allEvents)
        {
            if (item.Title == finalEventID.ToString())
            {
                statclass = item;
            }
        }

        return statclass;
    }


    //public bool OnResolveCondition(string dataNameFile, StatsClass element)
    //{
    //    Debug.Log("ahoj");

    //    return true;
    //}

}
