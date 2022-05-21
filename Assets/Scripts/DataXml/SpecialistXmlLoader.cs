using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;

public class SpecialistXmlLoader : MonoBehaviour
{

    public List<Specialists> GetSpecialistFromXML ()
    {
        List<Specialists> createdSpecialists = new List<Specialists>();

        createdSpecialists = LoadSpecialistFromXML();

        return createdSpecialists;
    }

    private List<Specialists> LoadSpecialistFromXML()
    {
        List<StatsClass> XMLLoadedSpecialists = new List<StatsClass>();
        List<StatsClass> XMLAdditionalSpecialistsInformation = new List<StatsClass>();
        List<Specialists> allSpecialists = new List<Specialists>();

        string path = "Assets/Data/XML";
        string fileName = "Specialists";
        string fileNameCZ = "Specialists-CZ";

        ResolveMaster resolveMaster = new ResolveMaster();

        Dictionary<string, StatsClass> firstData = StatsClass.LoadXmlFile(path, fileName, false);
        resolveMaster.AddDataNode(fileName, firstData);

        Dictionary<string, StatsClass> secondData = StatsClass.LoadXmlFile(path, fileNameCZ, false);
        resolveMaster.ModifyDataNode(fileName, secondData);

        XMLLoadedSpecialists = resolveMaster.GetDataKeys(fileName);

        allSpecialists = DeSerializedSpecialists(XMLLoadedSpecialists);

        return allSpecialists;
    }

    private List<Specialists> DeSerializedSpecialists(List<StatsClass> firstStatClass)
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
                spec.SpriteString = spriteName;
                spec.Sprite = spriteLoader.LoadSpecialistSprite(spriteName);
            }
            else
            {
                Debug.LogError("Sprite Loader is Null -> Sprite will not be loaded -> " + this.name);
            }

            // for data which can be translated
            spec.Povolani = item.GetStrStat("SpecClass");
            spec.Story = item.GetStrStat("SpecStory");
            // spec.na = spec.FullName + " - " + spec.Povolani; // for unity inspector.
            


            allSpecialists.Add(spec);
        }

        return allSpecialists;

    }

}
