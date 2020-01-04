using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Region", fileName = "_NewRegion")]
public class Region : ScriptableObject
{
    public string regionName = "NoWhere";
    [SerializeField] private bool isStartingRegion = false;
    public bool isExplored = false;
    public bool isInShadow = false;
    public bool isInDarkness = true;

    public List<Region> neighborhoodRegions = new List<Region>();

    public List<Events> events = new List<Events>(); // zatim nema vyuziti jeste uvidim jak se to vyvyne

    public string RegionName
    {
        get { return this.regionName;}
    }

    public bool IsStartingRegion
    {
        get { return isStartingRegion; }
    }

    public bool IsInShadow
    {
        get { return isInShadow; }
        set
        {
            isInShadow = value;
            if (this.isInShadow == true)
            {
                isInDarkness = false;
                isExplored = false;
            }
        }
    }

    public bool IsExplored 
    {
        get { return isExplored; }
        set
        {
            isExplored = value;
            if (this.isExplored == true)
            {
                isInDarkness = false;
                isInShadow = false;
            }
        }
    }

    public bool IsOutOfReach
    {
        get { return isInDarkness; }
        set
        {
            isInDarkness = value;
            this.isExplored = false;
            isInShadow = false;
        }
    }

    public void RevealNeighbors ()
    {
        foreach (Region item in neighborhoodRegions)
        {
            item.IsInShadow = true;
        }
    }
   

}

public class Events
{

}