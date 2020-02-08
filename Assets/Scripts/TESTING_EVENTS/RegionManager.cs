using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{

    List<Region> regions = new List<Region>();

    [SerializeField]
    private GameObject missionPrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegistrRegion(Region region)
    {
        regions.Add(region);
    }

    public void OpenMissionPanel(Mission mission)
    {
        missionPrefab.SetActive(true);
    }




}
