using System.Collections;
using System.Collections.Generic;
using ResolveMachine;
using UnityEngine;
using System.Linq;

public class MissionXmlLoader : MonoBehaviour
{

    public List<Mission> GetMissionsFromXML()
    {
        List<Mission> createdMissions = new List<Mission>();

        createdMissions = LoadMissionsFromXML();

        return createdMissions;
    }

    private List<Mission> LoadMissionsFromXML()
    {
        List<StatsClass> XMLLoadedMissions = new List<StatsClass>();
        List<StatsClass> XMLAdditionalMissionsInformation = new List<StatsClass>();
        List<Mission> allMissions = new List<Mission>();

        string path = "Assets/Data/XML";
        string fileName = "Missions";
        string fileNameCZ = "Missions-CZ";
        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName, false);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ, false);
        resolveMaster.ModifyDataNode(fileName, secondData);

        XMLLoadedMissions = resolveMaster.GetDataKeys(fileName);


        allMissions = DeSerializedMission(XMLLoadedMissions);

        // chyby jeste direct a final event...   dodelat...

        //for (int i = 0; i < AllLoadedMissions.Count; i++)
        //{
        //    slave.Add(resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[i].Title));
        //}

        //for (int i = 0; i < AllLoadedMissions.Count; i++)
        //{
        //    slave.Add(resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[i].Title));
        //}

        //

        ResolveSlave slave = resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[0].Title);
        slave.StartResolve();
        var output = slave.Resolve();

        return allMissions;
    }

    private List<Mission> DeSerializedMission(List<StatsClass> firstStatClass)
    {
        List<Mission> allMissions = new List<Mission>();
        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass item in firstStatClass)
        {
            Mission newMission = new Mission(); //Take care Construktor is not empty..

            bool success = long.TryParse(item.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some mission has Error while DeSerialized id: " + item.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            newMission.Id = idNumber;
            newMission.Repeate = item.GetBoolStat("Repeatable");

            newMission.DifficultyMax = item.GetIntStat("DifficultyMax");
            newMission.DifficultyMin = item.GetIntStat("DifficultyMin");

            string terrains = item.GetStrStat("Terrain");
            List<string> result = terrains.Split(',').ToList();
            foreach (var str in result)
            {
                Terrain terrain = newMission.ConvertTerrainStringData(str);
                newMission.AddTerrain(terrain);
            }
            newMission.Distance = item.GetIntStat("Time"); // delka mise
            newMission.InitialDistance = newMission.Distance;
            newMission.SpecMin = item.GetIntStat("SpecMin");
            newMission.SpecMax = item.GetIntStat("SpecMax");
            newMission.NeededTransport = item.GetStrStat("Transport");
            newMission.EventsMin = item.GetIntStat("EventsMin");
            newMission.EventsMax = item.GetIntStat("EventsMax");

            // FinalEvent and DirectEvent
            var statclass = item.GetData("$E");
            var keys = statclass.GetKeys();

            foreach (string key in keys)
            {
                StatsClass stat = statclass.GetData(key);
                int tValue = stat.GetIntStat("$T");
                if (tValue == 1)
                {
                    string finalEventId = stat.GetStrStat("FinalEvent");
                    success = long.TryParse(finalEventId, out long finalID);
                    if (!success)
                        finalID = 0;

                    newMission.FinalEventID = finalID;
                }
                else if (tValue == 2)
                {
                    int order = stat.GetIntStat("Order");
                    string directEventId = stat.GetStrStat("DirectEvent");
                    success = long.TryParse(directEventId, out long directID);
                    if (!success)
                        directID = 0;

                    DirectEvents directEvent = new DirectEvents();
                    directEvent.order = order;
                    directEvent.eventID = directID;
                    newMission.AddDirectEvent(directEvent);
                }
            }

            if (newMission.EventsMin > newMission.EventsMax)
                newMission.EventsMax = newMission.EventsMin;

            newMission.RepeatableIn = item.GetIntStat("RepTime");
            newMission.MissionPointer = item.GetStrStat("MissionPointer");

            // for data which can be translated
            newMission.Description = item.GetStrStat("Description");
            newMission.Name = item.GetStrStat("Name");
            var misType = item.GetStrStat("Type");
            newMission.Type = newMission.ConvertMissionTypeStringData(misType);
            
            if (spriteLoader != null)
            {
                string spriteName = item.GetStrStat("MisImage");
                newMission.Image = spriteLoader.LoadMissionSprite(spriteName);
                if (newMission.Image == null)
                    Debug.LogError("Sprite is null -> Image will not be displayed -> " + this.name + " Neffe tak me to uz dodej :D");
            }
            else
            {
                Debug.LogError("Sprite Loader is Null -> Sprite will not be loaded -> " + this.name);
            }

            allMissions.Add(newMission);
        }

        return allMissions;
    }
}
