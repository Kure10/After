using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class uButtonAdditionalMission : uButton , IPointerEnterHandler , IPointerExitHandler
{
    [SerializeField] private string missionIdentifikator;


    private Animator animator;
    private Button additionMissionButton;

    private ButtonState lastState;

    private ButtonState currentState;

    public ButtonState CurrentState { get { return this.currentState; } }


    #region Properities
    public string MissionIdentifikator
    {
        get { return this.missionIdentifikator; }
    }

    #endregion Properities

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        additionMissionButton = this.GetComponent<Button>();
    }

    private void OnEnable()
    {
        ChangeCurrentState(this.currentState);
    }

    public void Start()
    {
        
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


    public void ChangeHighlightAnim()
    {

        switch (this.currentState)
        {
            case ButtonState.Available:
                animator.SetTrigger("Highlighted");
                Debug.Log("jsem tady");
                break;
            case ButtonState.InProgress:
                animator.SetTrigger("Highlighted");
                break;
            case ButtonState.Executed:
                animator.SetTrigger("HighlightedNotPulsing");
                break;
            case ButtonState.InRepeatPeriod:
                animator.SetTrigger("HighlightedNotPulsing");
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeHighlightAnim();
        Debug.Log("someanim");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetTrigger("Normal");
        Debug.Log("normal");
    }

    
    public enum ButtonState
    {
        Available,
        InProgress,
        Executed,
        InRepeatPeriod
    }

    

}
