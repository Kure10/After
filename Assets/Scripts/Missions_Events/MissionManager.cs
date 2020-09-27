using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MissionManager : MonoBehaviour
{


    [SerializeField] private MissionController theMC;
    [SerializeField] private SpecialistControler theSC;


    public List<Mission> exploreMissions = new List<Mission>();
    public List<Mission> othersMissions = new List<Mission>();

    public Mission currentMission;

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

        Debug.Log(button.MissionIdentifikator);

        Button additionMissionButton = button.gameObject.GetComponent<Button>();
        additionMissionButton.onClick.RemoveAllListeners();
        additionMissionButton.onClick.AddListener(delegate () { ShowMissionPanel(choisedMission, regionOperator, button); });
    }
    private void ShowMissionPanel(Mission mission, RegionOperator regionOperator, uButtonAdditionalMission missionButton = null)
    {



        Button startButton = theMC.windowMissionController.GetWindowButton();

        startButton.onClick.RemoveAllListeners();

        if (this.theMC.IsMissionInProgress(mission.MissionPointer))
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
            Debug.Log("  ani jedno");
            theMC.windowMissionController.State = WindowMissionController.MissionPanelState.normal;
            startButton.onClick.AddListener(delegate () { theMC.StartMission(mission, regionOperator, missionButton); });
        }

        List<Specialists> specForMission = theSC.PassSpecToMissionSelection();

        theMC.windowMissionController.SetUpWindow(mission);

        theMC.windowMissionController.CreateSpecListForMission(specForMission);

        theMC.windowMissionController.Init();

        currentMission = mission;

        theMC.windowMissionController.SetActivePanel(true);
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
