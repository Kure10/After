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

        string path = "Assets/Data/XML";
        string fileName = "Events";
        string fileNameCZ = "Events-CZ";

        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        resolveMaster.AddDataNode(fileNameCZ, secondData);

        //XMLLoadedEvents = resolveMaster.GetDataKeys(fileName);
        //XMLAdditionalEventsInformation = resolveMaster.GetDataKeys(fileNameCZ);

        //allEvents = DeSerializedEvents(XMLLoadedEvents, XMLAdditionalEventsInformation);

       // ResolveSlave slave;

        List<StatsClass> allMyEvents = resolveMaster.GetDataKeys("Events");

        //foreach (StatsClass item in allMyEvents)
        //{
        //    slave = resolveMaster.AddDataSlave("Events", item.Title);
        //    slave.StartResolve();
        //    Dictionary<string, List<StatsClass>> output = slave.Resolve();

        //    foreach (List<StatsClass> acko in output.Values)
        //    {
        //        foreach (StatsClass aa in acko)
        //        {
        //            string tecko;
                    
        //            Debug.Log("dasd");
        //            tecko = aa.GetStrStat("$T");
                  
        //        }
        //    }

        //}

        //ResolveSlave slave = resolveMaster.AddDataSlave("Events", resolveMaster.GetDataKeys("Events")[0].Title);
        ////slave = resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[1].Title);
        //slave.StartResolve();
        //Dictionary<string, List<StatsClass>> output = slave.Resolve();


        return allMyEvents;
    }

    private List<EventBlueprint> DeSerializedEvents(List<StatsClass> firstStatClass, List<StatsClass> secondStatClass)
    {
        List<EventBlueprint> allEvents = new List<EventBlueprint>();
        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass item in firstStatClass)
        {
            EventBlueprint eventBlueprint = new EventBlueprint(); //Take care Construktor is not empty..

            bool success = long.TryParse(item.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some Event has Error while DeSerialized id: " + item.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            eventBlueprint.Id = idNumber;

            eventBlueprint.difficulty = item.GetIntStat("Difficulty");
            eventBlueprint.eventTerrain = item.GetStrStat("EventTerrain"); // promyslet


            //eventBlueprint.name = item.GetStrStat("SpecName");
            //eventBlueprint.Level = item.GetIntStat("SpecLvL");

            //eventBlueprint.Mil = item.GetIntStat("SpecMiL");
            //eventBlueprint.Tel = item.GetIntStat("SpecTeL");
            //eventBlueprint.Scl = item.GetIntStat("SpecScL");
            //eventBlueprint.Sol = item.GetIntStat("SpecSoL");
            //eventBlueprint.Kar = item.GetIntStat("SpecKar");

            //int red = item.GetIntStat("SpecColorRed");
            //int green = item.GetIntStat("SpecColorGreen");
            //int blue = item.GetIntStat("SpecColorBlue");
            //eventBlueprint.SetColor(red, green, blue);

            //eventBlueprint.Localization = item.GetStrStat("Location");
            //eventBlueprint.IsDefault = eventBlueprint.Localization.StartsWith("START");

            //if (spriteLoader != null)
            //{
            //    string spriteName = item.GetStrStat("SpecAvatar");
            //    eventBlueprint.Sprite = spriteLoader.LoadSpecialistSprite(spriteName);
            //}
            //else
            //{
            //    Debug.LogError("Sprite Loader is Null -> Sprite will not be loaded -> " + this.name);
            //}

            // for data which can be translated
            foreach (StatsClass secondItem in secondStatClass)
            {
                if (item.Title == secondItem.Title)
                {
                    eventBlueprint.name = secondItem.GetStrStat("Name");
                    eventBlueprint.description = secondItem.GetStrStat("Description");
                }
            }

            // spec.na = spec.FullName + " - " + spec.Povolani; // for unity inspector.
           // eventBlueprint.ReCalcAutoStats();


            allEvents.Add(eventBlueprint);
        }

        return allEvents;

    }
}
