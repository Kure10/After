using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;
using System;

public class ItemXmlLoader 
{
    public List<ItemBlueprint> GetItemsFromXML()
    {
        List<ItemBlueprint> createdItems = new List<ItemBlueprint>();

        createdItems = LoadItemsFromXML();

        return createdItems;
    }

    private List<ItemBlueprint> LoadItemsFromXML()
    {
        List<StatsClass> XMLLoadedItems= new List<StatsClass>();

        List<ItemBlueprint> items = new List<ItemBlueprint>();

        string path = "Assets/Data/XML";
        string fileName = "Items";
        string fileNameCZ = "Items-CZ";

        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        resolveMaster.ModifyDataNode(fileName, secondData);

        XMLLoadedItems = resolveMaster.GetDataKeys(fileName);

        items = DeSerializedSpecialists(XMLLoadedItems);

        return items;
    }

    private List<ItemBlueprint> DeSerializedSpecialists(List<StatsClass> firstStatClass)
    {
        List<ItemBlueprint> items = new List<ItemBlueprint>();

        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass statClass in firstStatClass)
        {
           // Item item = new Item(); //Take care Construktor is not empty..

            bool success = long.TryParse(statClass.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some specialist has Error while DeSerialized id: " + statClass.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            long id = idNumber;
            string typeString = statClass.GetStrStat("ItemType");
            string name = statClass.GetStrStat("X");

            ItemBlueprint.ItemType type;
            bool checkParse = Enum.TryParse(typeString, out type);

            ItemBlueprint item = new ItemBlueprint(id, name,type);

            items.Add(item);
        }

        return items;

    }
}
