using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;

public class SpecialistXmlLoader : MonoBehaviour
{

    public List<Specialists> GetSpecialistFromXML ()
    {
        List<Specialists> createdSpecialists = new List<Specialists>();

        createdSpecialists = LoadMissionsFromXML();

        return createdSpecialists;
    }

    public List<Specialists> LoadMissionsFromXML()
    {
        List<StatsClass> XMLLoadedSpecialists = new List<StatsClass>();
        List<StatsClass> XMLAdditionalSpecialistsInformation = new List<StatsClass>();
        List<Specialists> allSpecialists = new List<Specialists>();

        string path = "Assets/Data/XML";
        string fileName = "Specialists";
        string fileNameCZ = "Specialists-CZ";

        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ);
        resolveMaster.AddDataNode(fileNameCZ, secondData);

        XMLLoadedSpecialists = resolveMaster.GetDataKeys(fileName);
        XMLAdditionalSpecialistsInformation = resolveMaster.GetDataKeys(fileNameCZ);

        allSpecialists = DeSerializedSpecialists(XMLLoadedSpecialists, XMLAdditionalSpecialistsInformation);

        return allSpecialists;
    }

    public List<Specialists> DeSerializedSpecialists(List<StatsClass> firstStatClass, List<StatsClass> secondStatClass)
    {
        List<Specialists> allSpecialists = new List<Specialists>();
        ResourceSpriteLoader spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        foreach (StatsClass item in firstStatClass)
        {
            Specialists spec = new Specialists(); //Take care Construktor is not empty..

            bool success = long.TryParse(item.Title, out long idNumber);
            if (!success)
                Debug.LogError("Some specialist has Error while DeSerialized id: " + item.Title);
            // mozna poptremyslet o nejakem zalozním planu když se neco posere..
            // napriklad generovani nejake generické mise.. Nebo tak neco..

            spec.Id = idNumber;


            spec.FullName = item.GetStrStat("SpecName");
            spec.Level = item.GetIntStat("SpecLvL");

            spec.Mil = item.GetIntStat("SpecMiL");
            spec.Tel = item.GetIntStat("SpecTeL");
            spec.Scl = item.GetIntStat("SpecScL");
            spec.Sol = item.GetIntStat("SpecSoL");
            spec.Kar = item.GetIntStat("SpecKar");

            int red = item.GetIntStat("SpecColorRed");
            int green = item.GetIntStat("SpecColorGreen");
            int blue = item.GetIntStat("SpecColorBlue");

            spec.SetColor(red, green, blue);

            spec.Localization = item.GetStrStat("Location");
            spec.IsDefault = spec.Localization.StartsWith("START");
            
            if (spriteLoader != null)
            {
                string spriteName = item.GetStrStat("SpecAvatar");
                spec.Sprite = spriteLoader.LoadSpecialistSprite(spriteName);
            }
            else
            {
                Debug.LogError("Sprite Loader is Null -> Sprite will not be loaded");
            }


            // for data which can be translated
            // pls optimalizovat totto :DDD
            foreach (StatsClass secondItem in secondStatClass)
            {
                if (item.Title == secondItem.Title)
                {
                    spec.Povolani = secondItem.GetStrStat("SpecClass");
                    spec.Story = secondItem.GetStrStat("SpecStory");
                }
            }


            spec.name = spec.FullName + " - " + spec.Povolani;
          //  spec.Sprite = image; // Todo dodelat .. Image se bude brat s nejakeho poolu..

            spec.ReCalcAutoStats();

            allSpecialists.Add(spec);
        }

        return allSpecialists;

    }

}
