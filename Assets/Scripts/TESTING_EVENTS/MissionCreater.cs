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
            missions.Add(CreateMission(i));
        }
        FillMissionList();
    }

    public Mission CreateMission(int i)
    {
        Mission mis = new Mission();

        mis.missionName = "Explore";
        mis.missionDistance = 30f;
        mis.image = image;
        mis.missionType = "Typerino : " + i.ToString(); ;

        return mis;
    }

    public void FillMissionList()
    {
        missionManager.missions = this.missions;
    }
}
