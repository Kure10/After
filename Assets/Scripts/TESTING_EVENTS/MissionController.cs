using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{

    public List<Mission> missionsInProcces = new List<Mission>();

    public void StartMission(Mission missinToStart)
    {
        missionsInProcces.Add(missinToStart);

       // Debug.Log("START !!!!!!" + missinToStart.missionDistance + " a dalsi : " + missinToStart.missionType );

       // Debug.Log(missionsInProcces.Count.ToString());

    }

    public void Update()
    {

        MissionProcess();
    }

    public void MissionProcess()
    {

        foreach (var item in missionsInProcces)
        {
            item.missionDistance -= Time.deltaTime;
            Debug.Log(item.missionDistance.ToString("F1"));

            if (item.missionDistance <= 0)
            {
                Debug.Log("mission is done");
                MissionComplete(item);
            }
        }
    }


    public void MissionComplete(Mission mission)
    {
        //player gets reward
        // more shits

        missionsInProcces.Remove(mission);
    }


}
