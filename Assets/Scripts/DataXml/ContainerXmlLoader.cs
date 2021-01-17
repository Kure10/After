using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;
using System;


public class ContainerXmlLoader
{

    long lokingContainerNumber = 0;

    public Container GetContainerByNumber(long containerNumber)
    {
        lokingContainerNumber = containerNumber;

        Container createdItems = new Container();

        createdItems = LoadItemsFromXML();

        return createdItems;
    }

    private Container LoadItemsFromXML()
    {
        List<StatsClass> XMLLoadedItems = new List<StatsClass>();

        Container cont = new Container();

        string path = "Assets/Data/XML";
        string fileName = "Containers";
        //string fileNameCZ = "Items-CZ";

        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

       // Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
       // resolveMaster.ModifyDataNode(fileName, secondData);

        XMLLoadedItems = resolveMaster.GetDataKeys(fileName);

        cont = DeSerializedSpecialists(XMLLoadedItems, resolveMaster);

        return cont;
    }

    private Container DeSerializedSpecialists(List<StatsClass> firstStatClass, ResolveMaster resolveMaster)
    {
        ResolveSlave slave;
        Dictionary<string, List<StatsClass>> output = new Dictionary<string, List<StatsClass>>();
        Container container = new Container();

        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass statClass in firstStatClass)
        {
            bool success = long.TryParse(statClass.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some specialist has Error while DeSerialized id: " + statClass.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            if (idNumber != lokingContainerNumber)
                continue;

            container.id = idNumber;

            container.name = statClass.GetStrStat("ContainerName");
            container.rarity = statClass.GetStrStat("ContainerRarite");
            container.level = statClass.GetIntStat("ConainerLevel");

            slave = resolveMaster.AddDataSlave("Containers", statClass.Title);

            // tady to musím upravit..
            if (slave != null)
            {
                slave.StartResolve();
                output = slave.Resolve();
            }

            var count = output["Result"].Count;

            int i = 0;
            foreach (StatsClass seconData in output["Result"])
            {
                SubContainerData subData = new SubContainerData();
                subData.dropChange = seconData.GetIntStat("DropChance");
                string stringID = seconData.GetStrStat("DropItem");
                subData.dropItemID = Convert.ToInt64(stringID);
                subData.itemCountMin = seconData.GetIntStat("ItemsCountMin");
                subData.itemCountMax = seconData.GetIntStat("ItemsCountMax");

                container.containerSubData.Add(subData);

            }
        }

        return container;

    }
}

public class Container
{
    public long id;
    public string name;
    public int level;
    public string rarity;

    public List<SubContainerData> containerSubData = new List<SubContainerData>();
}

public class SubContainerData
{
    public int dropChange;
    public long dropItemID;
    public int itemCountMin;
    public int itemCountMax;
}

