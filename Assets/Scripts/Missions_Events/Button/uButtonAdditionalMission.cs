using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class uButtonAdditionalMission : uButton
{
    [SerializeField] private string missionIdentifikator;


    private Animator animator;
    private Button additionMissionButton;

    private ButtonState currentState;

    public ButtonState CurrentState { get { return this.currentState; } }


    #region Properities
    public string MissionIdentifikator
    {
        get { return this.missionIdentifikator; }
    }

    #endregion Properities

    public void Start()
    {
        animator = this.GetComponent<Animator>();
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

    //private void SetColor()
    //{
    //    Image img = this.GetComponent<Image>();
    //    Color col = new Color(155, 155, 155);
    //    img.color = col;
    //}

    public void ChangeCurrentState(ButtonState state)
    {
        this.currentState = state;

        ReserAllTrigers();

        switch (state)
        {
            case ButtonState.Available:
                animator.SetTrigger("RepatTimeOut");
                break;
            case ButtonState.InProgress:
                animator.SetTrigger("Started");
                break;
            case ButtonState.Executed:
                animator.SetTrigger("Finished");
                break;
            case ButtonState.InRepeatPeriod:
                animator.SetTrigger("FinishedButIsRepeat");
                break;
            default:
                break;
        }
    }

    private void ReserAllTrigers()
    {
        animator.ResetTrigger("RepatTimeOut");
        animator.ResetTrigger("Started");
        animator.ResetTrigger("Finished");
        animator.ResetTrigger("FinishedButIsRepeat");
    }

    public enum ButtonState
    {
        Available,
        InProgress,
        Executed,
        InRepeatPeriod
    }

}
