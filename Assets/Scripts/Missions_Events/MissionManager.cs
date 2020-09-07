using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MissionManager : MonoBehaviour
{


    [SerializeField] private MissionController theMC;


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
    public void ChoiseMissionForRegionButton(uButtonAdditionalMission mission, RegionOperator regionOperator)
    {
        Mission choisedMission = FindMissionFromList(mission.MissionIdentifikator, false);

        Debug.Log(mission.MissionIdentifikator);

        Button additionMissionButton = mission.gameObject.GetComponent<Button>();
        additionMissionButton.onClick.RemoveAllListeners();
        additionMissionButton.onClick.AddListener(delegate () { ShowMissionPanel(choisedMission, regionOperator); });
    }
    private void ShowMissionPanel(Mission mission, RegionOperator regionOperator)
    {
        // tady musi byt check jestli misse už neprobíha..
        if (this.theMC.IsMissionInProgress(mission.MissionPointer))
        {
            Debug.LogError("Misse je in progress -> znovu nelze zpustit.");
            return;
        }

        theMC.windowMission.MissionName = mission.Name;
        theMC.windowMission.MissionType = mission.ConvertMissionTypeStringData(mission.Type);
        theMC.windowMission.MissionDistance = mission.Distance;
        theMC.windowMission.MissionLevel = mission.LevelOfDangerous;
        theMC.windowMission.MissionTerrainList = mission.GetEmergingTerrains;
        theMC.windowMission.MissionTime = mission.MissionTime;
        theMC.windowMission.Sprite = mission.Image;

        Button startButton = theMC.windowMission.GetStartMissionButton;

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate() { theMC.StartMission(mission, regionOperator); });

        theMC.windowMission.Init();

        currentMission = mission;

        theMC.windowMission.SetActivityMissionPanel = true;
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
