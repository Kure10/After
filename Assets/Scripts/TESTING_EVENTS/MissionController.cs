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

        if(missionsInProcces.Count > 0)
        {
            MissionProcess();
            TryOutbreakEvent();
        }


        
    }

    public void MissionProcess()
    {

        for (int i = missionsInProcces.Count -1; i >= 0; i--)
        {


            float betweenTime = CalculateTime();

            missionsInProcces[i].distance -= betweenTime;

            infoController.UpdateInfoRows(missionsInProcces);

            if (missionsInProcces[i].distance <= 0)
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

    private float CalculateTime()
    {
        float accumulatedTime = 0;
        accumulatedTime += Time.deltaTime * theTC.TimePointMultiplier();

        return accumulatedTime;
    }

    private void TryOutbreakEvent()
    {
       // Debug.Log("1 step " + missionsInProcces.Count );

        /*   Ok tadz bude muset byt komplikovanejsi system..
         1. preskakuji eventy.
         2. z nejakeho dubodu jich trigeruji vice.
         */

        for (int i = 0; i < missionsInProcces.Count; i++)
        {
            int distance = (int)missionsInProcces[i].distance;
        //    Debug.Log("2 step distance " + (int)missionsInProcces[i].missionDistance);

            for (int j = 0; j < missionsInProcces[i].posibleEvents.Count -1; j++)
            {
                EventBlueprint currentEvent = missionsInProcces[i].posibleEvents[j];
                if (distance < currentEvent.evocationTime && currentEvent.wasTriggered == false)
                {
                    currentEvent.wasTriggered = true;
                    Debug.Log("Event Triggerd");
                }
            }
        }
    }



}



