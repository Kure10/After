using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MissionManager : MonoBehaviour
{

    [SerializeField] private PanelTime time;
    [SerializeField] private MissionController theMC;
    //[SerializeField] private SpecialistControler theSC;


    public List<Mission> exploreMissions = new List<Mission>();
    public List<Mission> othersMissions = new List<Mission>();

  //  public Mission currentMission;

    public void ChoiseMission(RegionOperator regionOperator, bool isExploreMission = false)
    {
        Mission choisedMission = FindMissionWithPointer(regionOperator.Region.MapArena);

       // Mission choisedMission = FindMissionFromList(regionOperator.ExploreMissionID, isExploreMission);

        ShowMissionPanel(choisedMission, regionOperator);
    }

    // This method need to be one with ChoiseMission Method
    public void ChoiseMissionForRegionButton(uButtonAdditionalMission button, RegionOperator regionOperator)
    {
        Mission choisedMission = FindMissionFromList(button.MissionIdentifikator, false);

        Button additionMissionButton = button.gameObject.GetComponent<Button>();
        additionMissionButton.onClick.RemoveAllListeners();
        additionMissionButton.onClick.AddListener(delegate () { ShowMissionPanel(choisedMission, regionOperator, button); });
    }
    private void ShowMissionPanel(Mission mission, RegionOperator regionOperator, uButtonAdditionalMission missionButton = null)
    {

        Button startMissionButton = theMC.windowMissionController.GetWindowButton();

        startMissionButton.onClick.RemoveAllListeners();

        bool isMissionInProgress = this.theMC.IsMissionInProgress(mission.MissionPointer);

        if (isMissionInProgress)
        {
            theMC.windowMissionController.State = WindowMissionController.MissionPanelState.inProgress;
            Debug.Log("in progress");
        }
        else if (this.theMC.IsMissionInRepeatPeriod(mission.MissionPointer))
        {
            theMC.windowMissionController.State = WindowMissionController.MissionPanelState.inRepeatTime;
            Debug.Log("in repete period");
        }
        else if (mission.WasSuccessfullyExecuted)
        {
            theMC.windowMissionController.State = WindowMissionController.MissionPanelState.Complete;
            Debug.Log("in Completed");
        }
        else
        {
            theMC.windowMissionController.State = WindowMissionController.MissionPanelState.normal;
            startMissionButton.onClick.AddListener(delegate () { theMC.StartMission(mission, regionOperator, missionButton); });
        }

      //  List<Specialists> specForMission = theSC.PassSpecToMissionSelection();

        theMC.windowMissionController.SetUpWindow(mission, isMissionInProgress);

        //theMC.windowMissionController.CreateSpecAddButton(mission);

      //  theMC.windowMissionController.CreateSpecListForMission(specForMission);

        theMC.windowMissionController.Init(); // ToDo.. look inside..

        //this.currentMission = mission;

        theMC.windowMissionController.SetActivePanel(true);
        theMC.windowMissionController.ActivateBlocker();
        this.time.Pause();

    }

    private Mission FindMissionFromList(string missionIdentifikator, bool isExplorationMission)
    {
        Mission choisedMission = null;
        // Tedka mužu z listu misi odstranit.. Protože ji asi už tady nebudu potrebovat. 
        // Je to asi dobra optimalizace...  Uvidime pozdeji.


        // missionId je string a porovnavam s longem .. Cheknout jestli funguje.
        if (isExplorationMission)
            choisedMission = exploreMissions.Find(x => x.MissionPointer.ToString() == missionIdentifikator);
        else
            choisedMission = othersMissions.Find(x => x.MissionPointer.ToString() == missionIdentifikator);

        if (choisedMission == null)
            Debug.LogError("Error in MissionManager.Cs ___ Mission Was not found in list..");

        return choisedMission;
    }

    public Mission FindMissionWithPointer(string pointer)
    {

        string subString = pointer.Substring(pointer.Length -2);

        foreach (Mission item in exploreMissions)
        {
            if(item.MissionPointer.Contains(subString))
            {
                return item;
            }
        }

        return null;
    }

}
