using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    [SerializeField] List<Region> allRegions = new List<Region>();

    private RegionXmlLoader xmlLoader;
    [SerializeField ]private RegionControler regionControler;

    private void Awake()
    {
        xmlLoader = GetComponent<RegionXmlLoader>();
        allRegions = xmlLoader.GetRegionsFromXml();
        regionControler.SetRegions(allRegions);
    }

}
