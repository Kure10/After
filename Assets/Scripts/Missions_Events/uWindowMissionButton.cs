using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uWindowMissionButton : uButton
{
    [SerializeField] private string missionId;

    private Mission currentMission;

    #region Properities
    public string StringId
    {
        get { return this.missionId; }
    }
    public Mission CurrentMission
    {
        get { return this.currentMission; }
        set { this.currentMission = value; }
    }

    #endregion Properities
    //void OnEnable()
    //{
    //    Debug.Log("Mission Button was anabled: -> name of mission:  " + this.currentMission.Name);
    //}


}
