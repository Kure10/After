﻿using System.Collections;
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

        items = DeSerializedSpecialists(XMLLoadedItems, resolveMaster);

        return items;
    }

    private List<ItemBlueprint> DeSerializedSpecialists(List<StatsClass> firstStatClass, ResolveMaster resolveMaster)
    {
        ResolveSlave slave;
        Dictionary<string, List<StatsClass>> output = new Dictionary<string, List<StatsClass>>();
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

            ItemBlueprint item = new ItemBlueprint(id, name, type);

            item.capacity = statClass.GetIntStat("Capacity");
            item.absorbation = statClass.GetIntStat("Absorption");

            var repairableString = statClass.GetStrStat("Repairable");
            if (repairableString.Contains("Yes"))
                item.isRepairable = true;
            else
                item.isRepairable = false;

            item.repairBlock = statClass.GetIntStat("RepairBlock");
            item.repairCost.TM = statClass.GetIntStat("RepairCostTM");
            item.repairCost.MM = statClass.GetIntStat("RepairCostMM");
            item.repairCost.CM = statClass.GetIntStat("RepairCostCM");

            item.useCount = statClass.GetIntStat("UsesCount");

            item.rangeMin = statClass.GetIntStat("RangeMin");
            item.rangeMax = statClass.GetIntStat("RangeMax");



            slave = resolveMaster.AddDataSlave("Items", statClass.Title);

            if (slave != null)
            {
                slave.StartResolve();
                output = slave.Resolve();
            }


            // Sprite
            string spriteName = statClass.GetStrStat("ItemPicture");
            Sprite sprite = spriteLoader.LoadItemSpriteForType(spriteName);
            item.Sprite = sprite;
            if(sprite == null || item.Sprite == null)
            {
                // ToDo sprite is loaded randomly.  Kdzbz nahodou
                var tmp = spriteLoader.LoadItemSpriteForType(item.Type);
                item.Sprite = tmp;
            }

            items.Add(item);
        }

        return items;

    }
}
