using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static ItemBlueprint;

public class ResourceSpriteLoader : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> missionResources = new List<Sprite>();
    // [Header("Event Sprite")]
    [SerializeField]
    private List<Sprite> eventResources = new List<Sprite>();

   // [Header("Specialist Sprite")]
    [SerializeField]
    private List<Sprite> specResources = new List<Sprite>();

    [SerializeField]
    private List<Sprite> buildingResources = new List<Sprite>();

    [SerializeField]
    private List<Sprite> regionResources = new List<Sprite>();

    [SerializeField]
    private List<Sprite> itemResources = new List<Sprite>();

    [Space]
    [SerializeField] private Sprite defaultResource;

    // Tmp.. For testing purposes
    public Sprite LoadItemSpriteForType(ItemType type)
    {
        int number = 0;

        if(ItemType.ArmorSpec == type)
        {
            number = 1;
        }
        else if (ItemType.BagSpec == type)
        {
            number = 2;
        }
        else if (ItemType.ItemSpec == type)
        {
            number = 3;
        }
        else if (ItemType.ResBasic == type)
        {
            number = 4;
        }
        else if (ItemType.WeapSpec == type)
        {
            number = 5;
        }

        return this.itemResources[number];
    }

    public Sprite LoadSpecialistSprite(string resourceName)
    {
        return this.specResources.Find(sprite => sprite.name == resourceName);
    }

    public Sprite LoadMissionSprite(string resourceName)
    {
        return this.missionResources.Find(sprite => sprite.name == resourceName);
    }

    public Sprite LoadBuildingSprite(string resourceName)
    {
        return this.buildingResources.Find(sprite => sprite.name == resourceName);
    }

    public Sprite LoadRegionSprite(string resourceName)
    {
        return this.regionResources.Find(sprite => sprite.name == resourceName);
    }

    public Sprite LoadEventSprite(string resourceName)
    {
        Sprite sprite = defaultResource;

        for (int i = 0; i < eventResources.Count; i++)
        {
            if(resourceName == eventResources[i].name)
            {
                sprite = eventResources[i];
            } 
        }

        return sprite;
    }

    public void ClearEventResources ()
    {
        eventResources.Clear();
    }

    public void AddEventResource(Sprite sprite)
    {
        eventResources.Add(sprite);
    }

    public Sprite SetDefault { set { defaultResource = value; } }
 

}
