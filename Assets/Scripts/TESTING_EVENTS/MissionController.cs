using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{

    public MissionInfoController infoController;  /* tohle pak asi bude neco jako missionViewControler  Nebo tak neco aby se staral o misse na view casti */

    public List<Mission> missionsInProcces = new List<Mission>();

    [SerializeField]
    public uWindowMission windowMission;

    private TimeControl theTC;

    private void Awake()
    {
        theTC = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
    }

    public void StartMission (Mission missinToStart)
    {
        this.windowMission.gameObject.SetActive(false);

        missionsInProcces.Add(missinToStart);
        infoController.InfoRowCreate(missinToStart);

    }

    public void Update()
    {

        MissionProcess();
        
    }

    public void MissionProcess()
    {

        for (int i = missionsInProcces.Count -1; i >= 0; i--)
        {
            // var currentMission = missionsInProcces[i];
            missionsInProcces[i].missionDistance -= Time.deltaTime;

            infoController.UpdateInfoRows(missionsInProcces);

            if (missionsInProcces[i].missionDistance <= 0)
            {
                Debug.Log("mission is done");

                MissionComplete(missionsInProcces[i]);
                continue;
            }
        }
    }


    public void MissionComplete(Mission mission)
    {

        // delete from info row // pozdeji se asi napise mission complete a bude se cekat na hrace co udela ..
        //player gets reward

        // more shits

        missionsInProcces.Remove(mission); 
        infoController.DeleteFromInfoRow(mission);
    }

}


