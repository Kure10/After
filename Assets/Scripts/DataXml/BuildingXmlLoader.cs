using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ResolveMachine;
using UnityEngine;

public class BuildingXmlLoader : MonoBehaviour
{
    public List<BuildingBlueprint> GetBuildingsFromXML()
    {
        List<BuildingBlueprint> createdBuildings = new List<BuildingBlueprint>();

        createdBuildings = LoadBuildingFromXML();

        return createdBuildings;
    }

    private List<BuildingBlueprint> LoadBuildingFromXML()
    {
        List<StatsClass> XMLLoadedData = new List<StatsClass>();
        List<StatsClass> XMLAdditionalData = new List<StatsClass>();
        List<BuildingBlueprint> allBuildings = new List<BuildingBlueprint>();

        string path = "Assets/Data/XML/Testing Mission Data";
        string fileName = "Rooms";
        string fileNameCZ = "Rooms-CZ";
        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        resolveMaster.AddDataNode(fileNameCZ, secondData);

        XMLLoadedData = resolveMaster.GetDataKeys(fileName);
        XMLAdditionalData = resolveMaster.GetDataKeys(fileNameCZ);


        allBuildings = DeSerializedData(XMLLoadedData, XMLAdditionalData);

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
        //slave = resolveMaster.AddDataSlave("Missions", resolveMaster.GetDataKeys("Missions")[1].Title);
        slave.StartResolve();
        var output = slave.Resolve();

        return allBuildings;
    }

    private List<BuildingBlueprint> DeSerializedData(List<StatsClass> firstStatClass, List<StatsClass> secondStatClass)
    {
        List<BuildingBlueprint> allMissions = new List<BuildingBlueprint>();
        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass item in firstStatClass)
        {
            BuildingBlueprint newBuilding = new BuildingBlueprint(); //Take care Construktor is not empty..

            bool success = long.TryParse(item.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some building has Error while DeSerialized id: " + item.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            //newBuilding.Id = idNumber;
            //newBuilding.Repeate = item.GetIntStat("Repeat");
            //newBuilding.LevelOfDangerous = (LevelOfDangerous)item.GetIntStat("Difficulty");
            //string terrains = item.GetStrStat("Terrain");
            //List<string> result = terrains.Split(',').ToList();
            //foreach (var str in result)
            //{
            //    Terrain terrain = newMission.ConvertTerrainStringData(str);
            //    newMission.AddTerrain(terrain);
            //}
            //newBuilding.Distance = item.GetIntStat("Time"); // delka mise
            //newBuilding.SpecMin = item.GetIntStat("SpecMin");
            //newBuilding.SpecMax = item.GetIntStat("SpecMax");
            //newBuilding.NeededTransport = item.GetStrStat("Transport");
            //newBuilding.EventsMin = item.GetIntStat("EventsMin");
            //newBuilding.EventsMax = item.GetIntStat("EventsMax");
            //newBuilding.RepeatableIn = item.GetIntStat("IsRepeatable");

            //// for data which can be translated
            //foreach (StatsClass secondItem in secondStatClass)
            //{
            //    if (item.Title == secondItem.Title)
            //    {
            //        newBuilding.Description = secondItem.GetStrStat("Description");
            //        newBuilding.Name = secondItem.GetStrStat("Name");
            //        var misType = secondItem.GetStrStat("Type");
            //        newBuilding.Type = newMission.ConvertMissionTypeStringData(misType);
            //        string mapField = secondItem.GetStrStat("MapField");
            //        newBuilding.MapField = newMission.ConvertMapFieldStringData(mapField); // mise se vyskuje na teto mape..
            //    }
            //}

            //// ToDo doesnt work item.getstrstat(Wrong key) - spatny klic je unvnit neni dodelane od designu
            //if (spriteLoader != null)
            //{
            //    string spriteName = item.GetStrStat("MisImage");
            //    newMission.Image = spriteLoader.LoadMissionSprite(spriteName);
            //    if (newMission.Image == null)
            //        Debug.LogError("Sprite is null -> Image will not be displayed -> " + this.name + " Neffe tak me to uz dodej :D");
            //}
            //else
            //{
            //    Debug.LogError("Sprite Loader is Null -> Sprite will not be loaded -> " + this.name);
            //}

            allMissions.Add(newBuilding);
        }

        return allMissions;
    }
}
