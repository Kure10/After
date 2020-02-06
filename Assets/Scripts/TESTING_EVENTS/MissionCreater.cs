using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCreater : MonoBehaviour
{
    [SerializeField]
    public List<Mission> missions = new List<Mission>();
    private MissionManager missionManager;


    private void Awake()
    {
        this.missionManager =  this.GetComponent<MissionManager>();
        for (int i = 0; i < 10; i++)
        {
            missions.Add(CreateMission());
        }
        FillMissionList();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Mission CreateMission()
    {
        Mission mis = new Mission();


        return mis;
    }

    public void FillMissionList()
    {
        missionManager.missions = this.missions;
    }
}
