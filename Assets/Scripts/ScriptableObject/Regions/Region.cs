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
    [SerializeField] private int amountCompletedMissionsToNextRegion = -1;
    private bool isExplored = false;  // Obeven -> na viber jsou misse v Regionu.
    private bool isInShadow = false; //  Pokud je soused isExplored == true , Je v Shadow. OFC pokud sam neni Explored.
    private bool isInDarkness = true; // Počateční stav.  Je neaktivni cerny..

    public List<Region> neighborhoodRegions = new List<Region>();

    public string RegionName
    {
        get { return this.regionName;}
    }

    public int AmountToNext
    {
        get { return this.amountCompletedMissionsToNextRegion; }
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

    //public bool AreNeighborsRegionEnoughtExplored()
    //{
    //    foreach (Region item in neighborhoodRegions)
    //    {
    //        if(item.isInDarkness || item.isInShadow)
    //            continue;
            
    //        if(item.)


    //    }


    //    return false;
    //}


}
