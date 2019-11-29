using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Region", fileName = "_NewRegion")]
public class Region : ScriptableObject
{
    public string regionName = "NoWhere";
    [SerializeField] private bool isStartingRegion = false;
    private bool isExplored = false;
    private bool isOutOfReach = true;

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

    public bool IsExplored 
    {
        get { return isExplored; }
        set
        {
            isExplored = value;
            if (this.isExplored == true)
            {
                isOutOfReach = false;
            }
        }
    }

    public bool IsOutOfReach
    {
        get { return isOutOfReach; }
        set
        {
            isOutOfReach = value;
            this.isExplored = false;
        }
    }

    // jeste se musím doptat nefa jak to bude.

    // každy region by měl mít specifické mise .. což jsou eventy a to jeste nevím jak budu reprezentovat..

}

public class Events
{

}