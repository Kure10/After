using ResolveMachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.XR;
using Notification;

public class MissionController : MonoBehaviour
{

    public MissionInfoController infoController;  /* tohle pak asi bude neco jako missionViewControler  Nebo tak neco aby se staral o misse na view casti */
    
    private List<Mission> missionsInProcces = new List<Mission>(); /*vsechny probihající misse*/
    private List<Mission> missionsInRepate = new List<Mission>(); /* vsechny Opakovatelne misse. Obnovise po nejakem case */
    private List<Mission> missionsInReturnWay = new List<Mission>(); /* vsechny mise které probíhají ale specialiste se na nich vraci */

    [SerializeField] public WindowMissionController windowMissionController;

    [SerializeField] private SpecialistControler specialistControler;

    [SerializeField] private EventController eventController;

    [SerializeField] MissionCreater missionCreater;

    private TimeControl theTC;
    private PanelTime thePT;

    private ResourceManager resourceManager;

    private void Awake()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        this.theTC = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
        this.thePT = GameObject.FindObjectOfType<PanelTime>();


        EventController.OnEventEnd += RemoveEventNotification;
    }



    public void StartMission (Mission missinToStart,RegionOperator regionOperator, uButtonAdditionalMission missionButton = null)
    {
        if (windowMissionController.uWindowSpecSelection.IsWindowActive) return;

        if (windowMissionController.AmountReadyCharactersToMission < missinToStart.SpecMin)
        {
            Debug.Log("Počet charakteru nesedí: " + windowMissionController.AmountReadyCharactersToMission +
                ".  Mise vyžaduje min: " + missinToStart.SpecMin);
            return;
        }
        

        missionCreater.AddEventsToMission(missinToStart);

        missinToStart.AddSpecialistToMission(this.windowMissionController.StartMission());
        this.windowMissionController.SetActivePanel(false);

        infoController.InfoRowCreate(missinToStart);
        missinToStart.RegionOperator = regionOperator;
        missionsInProcces.Add(missinToStart);
        if (missionButton != null)
        {
            missionButton.ChangeCurrentState(uButtonAdditionalMission.ButtonState.InProgress);
        }
    }

    public void Update()
    {
        if(!this.thePT.IsPaused)
        {
            if (this.missionsInProcces.Count > 0)
            {
                MissionProcess();
                TryOutbreakEvent();
            }

            if (this.missionsInRepate.Count > 0)
            {
                CalculateRemainingTimeForRepeatMissions();
            }

            if (this.missionsInReturnWay.Count > 0)
            {
                ReturnMissionProccess();
            }
        }
    }

    private void MissionProcess()
    {
        for (int i = missionsInProcces.Count -1; i >= 0; i--)
        {
            Mission procesingMission = missionsInProcces[i];
            float betweenTime = CalculateTime();
            missionsInProcces[i].Distance -= betweenTime;
            infoController.UpdateInfoRows(missionsInProcces);

            if (missionsInProcces[i].Distance <= 0)
            {
                Debug.Log("Final Event Triger!!!");

                if (procesingMission.FinalEventID != 0)
                {
                    eventController.EventTrigered(procesingMission, true);

                    this.thePT.Pause(true);
                    EventController.isEventRunning = true;
                    NotificationManager.Instantion.AddNotification(procesingMission);
                }

                PrepareMissionToBackProcess(procesingMission);

                continue;
            }
        }
    }

    private void TryOutbreakEvent()
    {
        for (int i = 0; i < missionsInProcces.Count; i++)
        {
            Mission currentMission = missionsInProcces[i];
            int distance = (int)missionsInProcces[i].Distance;

            for (int j = 0; j < missionsInProcces[i].GetEventsContent.Count; j++)
            {

                EventContent currentEvent = currentMission.GetEventsContent[j];

                if (distance < currentEvent.evocationTime && currentEvent.wasTriggered == false)
                {
                    currentEvent.wasTriggered = true;

                    eventController.EventTrigered(currentMission);

                    this.thePT.Pause(true);
                    EventController.isEventRunning = true;

                    NotificationManager.Instantion.AddNotification(currentMission);

                    /* Time blocked*/

                    /*-------*/
                    Debug.Log("Event Triggerd");
                }
            }
        }
    }

    private void CalculateRemainingTimeForRepeatMissions()
    {
        for (int i = this.missionsInRepate.Count - 1; i >= 0; i--)
        {
            missionsInRepate[i].RepeatableTime -= CalculateTime();

            if (missionsInRepate[i].RepeatableTime <= 0)
            {
                MissionRefresh(missionsInRepate[i]);
            }
        }
    }

    private void ReturnMissionProccess()
    {
        for (int i = missionsInReturnWay.Count - 1; i >= 0; i--)
        {
            Mission procesingMission = missionsInReturnWay[i];
            float betweenTime = CalculateTime();
            missionsInReturnWay[i].Distance -= betweenTime;
            infoController.UpdateInfoRows(missionsInReturnWay);

            if (missionsInReturnWay[i].Distance <= 0)
            {
                if (procesingMission.Type == MissionType.pruzkum_oblasti)
                {
                    procesingMission.RegionOperator.ExploreRegion();
                }
                else
                {
                    if (procesingMission.Repeate)
                    {
                        procesingMission.RepeatableTime = procesingMission.RepeatableIn;
                        missionsInRepate.Add(procesingMission);

                        if (!procesingMission.WasSuccessfullyExecuted)
                        {
                            procesingMission.WasSuccessfullyExecuted = true;
                            procesingMission.RegionOperator.CompleteMission(true, procesingMission.MissionPointer, true);
                        }
                        else
                        {
                            procesingMission.RegionOperator.CompleteMission(true, procesingMission.MissionPointer, true);
                        }
                    }
                    else
                    {
                        procesingMission.WasSuccessfullyExecuted = true;
                        procesingMission.RegionOperator.CompleteMission(false, procesingMission.MissionPointer, true);
                    }
                }

                Debug.Log("Spec are going to return!!!");

                UnloadImportedGoodsFromMission(missionsInReturnWay[i].GetCharactersOnMission);

                specialistControler.CharacterOnMissionReturn(procesingMission.GetCharactersOnMission);

                MissionReward(missionsInReturnWay[i]);

                continue;
            }
        }
    }

    private void PrepareMissionToBackProcess(Mission procesingMission)
    {
        procesingMission.Distance = procesingMission.InitialDistance;
        foreach (var item in procesingMission.GetEventsContent)
        {
            item.wasTriggered = false;
        }
        missionsInProcces.Remove(procesingMission);
        missionsInReturnWay.Add(procesingMission);
    }

    private void MissionReward(Mission mission)
    {
        // delete from info row // pozdeji se asi napise mission complete a bude se cekat na hrace co udela ..
        //player gets reward

        mission.Distance = mission.InitialDistance;
        foreach (var item in mission.GetEventsContent)
        {
            item.wasTriggered = false;
        }
        missionsInReturnWay.Remove(mission); 
        infoController.DeleteFromInfoRow(mission);
    }

    private void UnloadImportedGoodsFromMission (List<Character> characterOnMission)
    {
        foreach (Character character in characterOnMission)
        {
            foreach (SpecInventorySlot slot in character.CharacterSlots)
            {
                Item item = slot.CurrentItem.item;
                if (item != null)
                {
                    if (item.Type == ItemBlueprint.ItemType.ResBasic)
                    {
                        resourceManager.AddResource(item.ResourceType, item.GetStackAmount, true);
                        slot.CleanSlot();
                    }
                }
            }

            foreach (SpecInventorySlot slot in character.CharacterBackPackSlots)
            {
                Item item = slot.CurrentItem.item;
                if (item != null)
                {
                    if (item.Type == ItemBlueprint.ItemType.ResBasic)
                    {
                        resourceManager.AddResource(item.ResourceType, item.GetStackAmount, true);
                        slot.CleanSlot();
                    }
                }
            }
        }
    }

    private void MissionRefresh(Mission mission)
    {
       // mission.RepeatableTime = 0;
        mission.RegionOperator.RefreshMissionButton(mission);

        this.missionsInRepate.Remove(mission);
    }

    private float CalculateTime()
    {
        float accumulatedTime = 0;
        accumulatedTime += Time.deltaTime * theTC.TimePointMultiplier;

        return accumulatedTime;
    }

    // event 
    private void RemoveEventNotification(Mission mission)
    {
        NotificationManager.Instantion.DestroyNotification(mission.Id);
    }

    public bool IsMissionInProgress(string missionPointer)
    {
        foreach (Mission item in this.missionsInProcces)
        {
            if (missionPointer == item.MissionPointer)
                return true;
        }

        return false;
    }

    public bool IsMissionInRepeatPeriod(string missionPointer)
    {
        foreach (Mission item in this.missionsInRepate)
        {
            if (missionPointer == item.MissionPointer)
                return true;
        }

        return false;
    }
}



