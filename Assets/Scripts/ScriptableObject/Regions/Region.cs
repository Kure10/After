using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/Region", fileName = "_NewRegion")]
public class Region : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    public string regionName = "NoWhere";
    [SerializeField] private bool isStartingRegion = false;
    private bool isExplored = false;
    private bool isInShadow = false;
    private bool isInDarkness = true;

    public List<Region> neighborhoodRegions = new List<Region>();

    public string RegionName
    {
        get { return this.regionName;}
    }

    public Sprite GetSprite
    {
        get { return sprite; }
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

    public bool IsInDarkness
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
            if (item.IsInShadow || item.IsExplored)
                continue;

            item.IsInShadow = true;
        }
    }

}
