using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Region", fileName = "_NewRegion")]
public class Region : ScriptableObject
{

    public bool isStartingRegion = false;
    public bool isExplored = false;
    public bool isFarAway = true;

    public List<Region> neighborhoodRegions = new List<Region>();

    public List<Events> events = new List<Events>();



    // jeste se musím doptat nefa jak to bude.

    // každy region by měl mít specifické mise .. což jsou eventy a to jeste nevím jak budu reprezentovat..

}



public class Events
{

}