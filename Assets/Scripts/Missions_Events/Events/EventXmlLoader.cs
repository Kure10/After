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

        var manager = GetComponent<EventManager>();

        string path = "Assets/Data/XML";
        string fileName = "Events";
        string fileNameCZ = "Events-CZ";

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName, false);
        manager.resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ, false);
        manager.resolveMaster.ModifyDataNode(fileName, secondData);



        List<StatsClass> allMyEvents = manager.resolveMaster.GetDataKeys("Events");

        return allMyEvents;
    }



}
