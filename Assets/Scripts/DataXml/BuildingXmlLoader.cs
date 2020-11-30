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
            newBuilding.Group = newBuilding.ConvertSectorStringData(tmp); 
            tmp = item.GetStrStat("RoomType");
            newBuilding.Type = newBuilding.ConvertTypeStringData(tmp);

            newBuilding.TimeToBuild = item.GetIntStat("BuildTime");

            newBuilding.column = item.GetIntStat("SizeA");
            newBuilding.row = item.GetIntStat("SizeB");

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
              //  Debug.LogError("Sprite Loader is Null -> Sprite will not be loaded -> " + this.name);
            }

            


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
