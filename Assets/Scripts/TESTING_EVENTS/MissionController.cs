using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{

    public List<Mission> missionsInProcces = new List<Mission>();

    public void StartMission(Mission missinToStart)
    {
        missionsInProcces.Add(missinToStart);
    }

    public void Update()
    {
        

    }

    public void MissionProcess()
    {
        foreach (var item in missionsInProcces)
        {
            item.missionDistance -= item.missionDistance * Time.deltaTime;

            if (item.missionDistance <= 0)
            {
                Debug.Log("mission is done");
            }
        }
    }

}
