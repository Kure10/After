using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;
using System;

public class MonsterXmlLoader
{
    public List<Monster> GetMonstersFromXML()
    {
        return LoadSpecialistFromXML();
    }

    private List<Monster> LoadSpecialistFromXML()
    {
        List<StatsClass> XMLLoadedMonsters = new List<StatsClass>();
        List<StatsClass> XMLAdditionalMonstersInformation = new List<StatsClass>();
        List<Monster> allMonsters = new List<Monster>();

        string path = "Assets/Data/XML";
        string fileName = "Bestiary";
        string fileNameCZ = "Bestiary-CZ";

        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName, false);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ, false);
        resolveMaster.ModifyDataNode(fileName, secondData);

        XMLLoadedMonsters = resolveMaster.GetDataKeys(fileName);

        allMonsters = DeSerializedMonsters(XMLLoadedMonsters);

        return allMonsters;
    }

    private List<Monster> DeSerializedMonsters(List<StatsClass> firstStatClass)
    {
        List<Monster> allMonsters = new List<Monster>();
        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass item in firstStatClass)
        {
            bool success = long.TryParse(item.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some monster has Error while DeSerialized id: " + item.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            Monster monster = new Monster(idNumber, item.GetStrStat("Name"), item.GetStrStat("Description")
                , item.GetIntStat("Treath"), item.GetIntStat("MiL"), item.GetIntStat("Lives")
                , item.GetDecStat("Speed"), item.GetIntStat("Danger"), item.GetIntStat("Range"), item.GetStrStat("BeastPicture"));

            Monster.MonsterType type = Monster.MonsterType.Demon;
            string kind = item.GetStrStat("EnemyKind");
            bool checkParse = Enum.TryParse(kind, out type);
            monster._monsterType = type;

            StatsClass otherData = item.GetData("$E");
            List<string> keys = otherData.GetKeys();

            foreach (string otherItems in keys)
            {
                StatsClass advanceData = otherData.GetData(otherItems);

                int tStat = advanceData.GetIntStat("$T");

                if(tStat == 1)
                {
                    Monster.Perk perk = new Monster.Perk();

                    Monster.PerkType perkType = new Monster.PerkType();
                    string perkString = advanceData.GetStrStat("Perks");
                    checkParse = Enum.TryParse(perkString, out perkType);

                    perk.value = advanceData.GetIntStat("Value");
                    perk.perks = perkType;

                    monster._perk.Add(perk);
                }
                else if(tStat == 2)
                {
                    Monster.Loot loot = new Monster.Loot();

                    string dropItem = advanceData.GetStrStat("DropItem");
                    loot.lootID = long.Parse(dropItem);

                    loot.dropChange = advanceData.GetIntStat("DropChance");
                    loot.itemCountMin = advanceData.GetIntStat("ItemsCountMin");

                    loot.itemCountMax = advanceData.GetIntStat("ItemsCountMax");
                    if (loot.itemCountMax < loot.itemCountMin)
                    {
                        loot.itemCountMax = loot.itemCountMin;
                    }

                    monster._loot.Add(loot);
                }
            }

            allMonsters.Add(monster);
        }

        return allMonsters;
    }
}
