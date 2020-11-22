using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    [SerializeField] private RegionControler regionControler;

    [SerializeField] List<Region> allRegions = new List<Region>();

    private RegionXmlLoader xmlLoader;


    public List<Region> AllRegions { get { return allRegions; } }


    private void Awake()
    {
        xmlLoader = GetComponent<RegionXmlLoader>();
        allRegions = xmlLoader.GetRegionsFromXml();
        regionControler.SetRegions(allRegions);
    }
}
