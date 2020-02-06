using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [SerializeField]
    GameObject missionPanel;

    public List<Mission> missions = new List<Mission>();



    public GameObject EnableMissionPanel { set { missionPanel.SetActive(value); } }

    public void ChoiseMission()
    {
        int i = Random.Range(0, missions.Count);
        Mission mission = missions[i];


        ShowMissionPanel(mission);

    }

    private void ShowMissionPanel(Mission mission)
    {
        missionPanel.SetActive(true);
    }


}
