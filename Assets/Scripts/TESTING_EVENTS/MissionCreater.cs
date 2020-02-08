using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCreater : MonoBehaviour
{
    [SerializeField]
    public List<Mission> missions = new List<Mission>();
    private MissionManager missionManager;

    public Sprite image;


    private void Awake()
    {
        this.missionManager =  this.GetComponent<MissionManager>();
        for (int i = 0; i < 10; i++)
        {
            missions.Add(CreateMission());
        }
        FillMissionList();
    }

    public Mission CreateMission()
    {
        Mission mis = new Mission();

        mis.missionName = "Explore";
        mis.missionDistance = 30f;
        mis.image = image;
        mis.missionType = "je to v pici";

        return mis;
    }

    public void FillMissionList()
    {
        missionManager.missions = this.missions;
    }
}
