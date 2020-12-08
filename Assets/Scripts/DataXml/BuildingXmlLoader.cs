using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ResolveMachine;
using UnityEngine;

public class BuildingXmlLoader
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

        string path = "Assets/Data/XML";
        string fileName = "Rooms";
        string fileNameCZ = "Rooms-CZ";
        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        resolveMaster.ModifyDataNode(fileName, secondData);

        XMLLoadedData = resolveMaster.GetDataKeys(fileName);

        allBuildings = DeSerializedData(XMLLoadedData);

        return allBuildings;
    }

    private List<BuildingBlueprint> DeSerializedData(List<StatsClass> firstStatClass)
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

            newBuilding.Id = idNumber;
          

            string tmp = item.GetStrStat("RoomGroup");
            newBuilding.Tag = newBuilding.ConvertTagStringData(tmp); 
            tmp = item.GetStrStat("RoomType");
            newBuilding.Type = newBuilding.ConvertTypeStringData(tmp);

            var prefabName = item.GetStrStat("RoomPrefab");
            newBuilding.Prefab = UnityEngine.Resources.Load("CompleteBuilding/" + prefabName) as GameObject;


            newBuilding.TimeToBuild = item.GetIntStat("BuildTime");

            newBuilding.column = item.GetIntStat("SizeA");
            newBuilding.row = item.GetIntStat("SizeB");

            if (newBuilding.Size == 6)
                newBuilding.ConstructionPrefab = UnityEngine.Resources.Load("InConstructionBuilding/Vystavba_2x3_mistnost") as GameObject;
            else if (newBuilding.Size == 4)
                newBuilding.ConstructionPrefab = UnityEngine.Resources.Load("InConstructionBuilding/Vystavba_2x2_mistnost") as GameObject;
            else if (newBuilding.Size == 2)
                newBuilding.ConstructionPrefab = UnityEngine.Resources.Load("InConstructionBuilding/Vystavba_2x1_mistnost") as GameObject;
            else if (newBuilding.Size == 1)
                newBuilding.ConstructionPrefab = UnityEngine.Resources.Load("InConstructionBuilding/Vystavba_1x1_mistnost") as GameObject;



            newBuilding.Civil = item.GetIntStat("PriceCM");
            newBuilding.Tech = item.GetIntStat("PriceTM");
            newBuilding.Military = item.GetIntStat("PriceMM");
            newBuilding.ElectricConsumption = item.GetIntStat("Consumption");

            // color
            int red = item.GetIntStat("RoomColorRed");
            int green = item.GetIntStat("RoomColorGreen");
            int blue = item.GetIntStat("RoomColorBlue");
            newBuilding.SetColor(red, green, blue);

            //// for data which can be translated
            newBuilding.Name = item.GetStrStat("RoomName");


            if (spriteLoader != null)
            {
                string spriteName = item.GetStrStat("RoomPicture");
                newBuilding.Sprite = spriteLoader.LoadBuildingSprite(spriteName);
            }
            else
            {
                Debug.LogError("Sprite Loader is Null -> Sprite will not be loaded ->  XML loader building ");
            }

            allMissions.Add(newBuilding);
        }

        return allMissions;
    }
}
