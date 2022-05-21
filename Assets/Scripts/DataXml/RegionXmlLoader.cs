using ResolveMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegionXmlLoader : MonoBehaviour
{
    public List<Region> GetRegionsFromXml()
    {
        List<Region> createdRegions = new List<Region>();

        createdRegions = LoadRegionsFromXml();

        return createdRegions;
    }

    private List<Region> LoadRegionsFromXml()
    {
        List<StatsClass> XMLLoadedRegions = new List<StatsClass>();
        //  List<StatsClass> XMLAdditionalMissionsInformation = new List<StatsClass>();
        List<Region> allRegions = new List<Region>();

        string path = "Assets/Data/XML";
        string fileName = "MapZone";
        //string fileNameCZ = "Missions-CZ";
        ResolveMaster resolveMaster = new ResolveMaster();

     

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName,false);
        resolveMaster.AddDataNode(fileName, firstData);

        // Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        // resolveMaster.ModifyDataNode(fileName, secondData);

        XMLLoadedRegions = resolveMaster.GetDataKeys(fileName);



        allRegions = DeSerializedRegions(XMLLoadedRegions);

        foreach (var reg in allRegions)
        {
            string regIdString = reg.Id.ToString();
            ResolveSlave slave = resolveMaster.AddDataSlave("MapZone", regIdString);
            slave.StartResolve();
            Dictionary<string, List<StatsClass>> output = slave.Resolve();
            List<StatsClass> result = output["Result"];

           // reg.neighborhoodRegionsPointer = new List<string>();
            for (int i = 0; i < result.Count; i++)
            {
                reg.neighborhoodRegionsPointer.Add(result[i].GetStrStat("MapNeighbor"));
            }

            foreach (var item in allRegions)
            {
                for (int i = 0; i < reg.neighborhoodRegionsPointer.Count; i++)
                {
                    string txt = reg.neighborhoodRegionsPointer[i];
                    if (txt == item.MapArena)
                    {
                        reg.neighborhoodRegions.Add(item);
                    }
                }
            }


        }

        return allRegions;
    }



    private List<Region> DeSerializedRegions(List<StatsClass> firstStatClass)
    {
        List<Region> allRegions = new List<Region>();
        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass item in firstStatClass)
        {
            Region newRegion = new Region();

            bool success = long.TryParse(item.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some region has Error while DeSerialized id: " + item.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            newRegion.Id = idNumber;
            newRegion.MapArena = item.GetStrStat("MapArea");
            newRegion.RegionName = item.GetStrStat("MapAreaName");
            string regionMapSprite = item.GetStrStat("MapCutout");
            newRegion.MissCompReq = item.GetIntStat("MissCompReq");
            newRegion.MapZoneDif = item.GetStrStat("MapZoneDif");
            newRegion.Sprite = spriteLoader.LoadRegionSprite(regionMapSprite);

            if(newRegion.MapArena == "Pole01")
            {
                newRegion.IsStartingRegion = true;
            }

            newRegion.name = newRegion.MapArena + " - " + newRegion.RegionName;

            allRegions.Add(newRegion);
        }

        return allRegions;
    }
}
