using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;

public class EventXmlLoader : MonoBehaviour
{
    public List<StatsClass> GetEventsFromXML()
    {
        List<StatsClass> loadedEvents = new List<StatsClass>();

        loadedEvents = LoadEventsFromXML();

        return loadedEvents;
    }
     
    private List<StatsClass> LoadEventsFromXML()
    {
        List<StatsClass> XMLLoadedEvents = new List<StatsClass>();
        List<StatsClass> XMLAdditionalEventsInformation = new List<StatsClass>();
        // List<EventBlueprint> allEvents = new List<EventBlueprint>();

        var creator = GetComponent<EventCreater>();

        string path = "Assets/Data/XML";
        string fileName = "Events";
        string fileNameCZ = "Events-CZ";

        // ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        creator.resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        creator.resolveMaster.ModifyDataNode(fileName, secondData);


        List<StatsClass> allMyEvents = creator.resolveMaster.GetDataKeys("Events");

        return allMyEvents;
    }

}
