using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> eventResources = new List<Sprite>();
    [Space]
    [SerializeField] private Sprite defaultResource;


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
