using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObject/Region", fileName = "_NewRegion")]
public class Region : ScriptableObject
{
    private long id;
    [SerializeField] private Sprite sprite;
    public string regionName = "NoWhere";
    [SerializeField] private bool isStartingRegion = false;
    [SerializeField] private int missCompReq = -1;
    private bool isEnoughtExplored = false;
    private bool isExplored = false;  // Obeven -> na vyber jsou misse v Regionu.
    private bool isInShadow = false; //  Pokud je soused isExplored == true , Je v Shadow. OFC pokud sam neni Explored.
    private bool isInDarkness = true; // Počateční stav.  Je neaktivni cerny..

    private string mapArena;
    private string mapZoneDif;

    public string MapArena { get { return this.mapArena; } set { this.mapArena = value; } }

    public string MapZoneDif { get { return this.mapZoneDif; } set { this.mapZoneDif = value; } }

    public long Id { get { return this.id; } set { this.id = value; } }

    public List<Region> neighborhoodRegions = new List<Region>();

    public List<string> neighborhoodRegionsPointer = new List<string>();

    public string RegionName { get { return this.regionName;} set { this.regionName = value; } }

    public int MissCompReq
    {
        get { return this.missCompReq; }
        set { this.missCompReq = value; }
    }

    public Sprite Sprite
    {
        get { return this.sprite; }
        set { this.sprite = value; }
    }

    public bool IsStartingRegion
    {
        get { return isStartingRegion; }
        set { this.isStartingRegion = value; }
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
