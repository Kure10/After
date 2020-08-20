using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    [Space]
    [SerializeField] private Sprite defaultResource;


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

    public Sprite FindEventResource (string resourceName)
    {
        Sprite tex = defaultResource;

        for (int i = 0; i < eventResources.Count; i++)
        {
            if(resourceName == eventResources[i].name)
            {
                tex = eventResources[i];
            } 
        }

        return tex;
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
