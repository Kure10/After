using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class MissionController : MonoBehaviour
{

    public MissionInfoController infoController;  /* tohle pak asi bude neco jako missionViewControler  Nebo tak neco aby se staral o misse na view casti */
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private Transform eventHolder;

    public List<Mission> missionsInProcces = new List<Mission>(); /*vsechny probihající misse*/

    [SerializeField]
    public uWindowMission windowMission;

    private TimeControl theTC;
    private PanelTime thePT;
    private MissionNotificationManager notificationMissionManager;

    private void Awake()
    {
        this.theTC = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
        this.thePT = GameObject.FindObjectOfType<PanelTime>();
        this.notificationMissionManager = GameObject.FindObjectOfType<MissionNotificationManager>();
    }



    public void StartMission (Mission missinToStart,RegionOperator regionOperator)
    {
        this.windowMission.gameObject.SetActive(false);
        missinToStart.RegionOperator = regionOperator;
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
            missionsInProcces[i].Distance -= betweenTime;
            infoController.UpdateInfoRows(missionsInProcces);

            if (missionsInProcces[i].Distance <= 0)
            {
                Debug.Log("mission is done");
                missionsInProcces[i].RegionOperator.ExploreRegion();
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
        for (int i = 0; i < missionsInProcces.Count; i++)
        {
            int distance = (int)missionsInProcces[i].Distance;

            for (int j = 0; j < missionsInProcces[i].GetEventsInMission.Count; j++)
            {
                Mission currentMission = missionsInProcces[i];
                EventBlueprint currentEvent = currentMission.GetEventsInMission[j];
                if (distance < currentEvent.evocationTime && currentEvent.wasTriggered == false)
                {
                    currentEvent.wasTriggered = true;
                    this.thePT.Pause(); // toto je mozna spatne :D musim se nad tím zamyslet co vsechno dela pause.

                    GameObject eventGameObject = Instantiate(this.eventPanel, this.eventHolder.transform.position, Quaternion.identity);
                    EventPanel eventPanel = eventGameObject.GetComponent<EventPanel>();

                    


                    eventGameObject.transform.SetParent(eventHolder);

                    /*tohle bude v interni metode.. .. TODO Dodelat pico..*/

                    eventPanel.TitleField.text = "ahoj";
                    eventPanel.DescriptionTextField.text = "LOL";
                    eventPanel.SetSprite = currentEvent.sprite;

                    // tohle jeste otestovat .... // ToDo nastavit button akci..
                    eventPanel.CreateOptions(currentEvent.numberOfOptions, currentEvent.answerTextField, currentEvent);

                    /* reset transform .. */
                    RectTransform rect = eventGameObject.GetComponent<RectTransform>();
                    rect.offsetMin = new Vector2(0, 0);
                    rect.offsetMax = new Vector2(0, 0);
                    eventGameObject.transform.localScale = new Vector3(1, 1, 1);


                    /* Create Notifiction*/
                    this.notificationMissionManager.CreateNewNotification(currentMission); // neni dokonceno

                    /* Time blocked*/
                    TimeControl.IsTimeBlocked = true;

                    /*-------*/
                    Debug.Log("Event Triggerd");
                }
            }
        }
    }
}



