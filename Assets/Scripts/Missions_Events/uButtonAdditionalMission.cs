using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uButtonAdditionalMission : uButton
{
    [SerializeField] private string missionId;

    private Mission currentMission;
    private Button.ButtonClickedEvent evt;

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

    public void Activate(bool setActive)
    {
        if (setActive)
            this.Anable();
        else
            this.Disable(); 

    }

    public void TemporarilyInactive(bool value)
    {

        Button additionMissionButton = this.gameObject.GetComponent<Button>();

        // nevim jestl si muzu takhle ulozit event .. ?? ,?? Zkusit
        if(this.evt == null)
            evt = additionMissionButton.onClick;

        if (value)
        {
            additionMissionButton.onClick.RemoveAllListeners();
            additionMissionButton.onClick = this.evt;
        }
        else
        {
            additionMissionButton.onClick.RemoveAllListeners();
        }


    }

    //void OnEnable()
    //{
    //    Debug.Log("Mission Button was anabled: -> name of mission:  " + this.currentMission.Name);
    //}


}
