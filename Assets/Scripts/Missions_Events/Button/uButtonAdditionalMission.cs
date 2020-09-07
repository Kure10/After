using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class uButtonAdditionalMission : uButton
{
    [SerializeField] private string missionIdentifikator;

    private Button additionMissionButton;

    #region Properities
    public string MissionIdentifikator
    {
        get { return this.missionIdentifikator; }
    }
    //public Mission CurrentMission
    //{
    //    get { return this.currentMission; }
    //    set { this.currentMission = value; }
    //}

    #endregion Properities

    public void Start()
    {
        additionMissionButton = this.GetComponent<Button>();
    }


    public void Activate(bool setActive)
    {
        if (setActive)
            this.Anable();
        else
            this.Disable(); 

    }

    public void ChangeMissionOnClickEvent(UnityAction evt)
    {
        additionMissionButton.onClick.RemoveAllListeners();
        additionMissionButton.onClick.AddListener(evt);
    }

    private void SetColor()
    {
        Image img = this.GetComponent<Image>();
        Color col = new Color(155, 155, 155);
        img.color = col;
    }

}
