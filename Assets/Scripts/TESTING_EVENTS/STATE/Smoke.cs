using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Smoke : State
{

    public Smoke(Region region) : base(region)
    {
        state = STATE.SMOKE;
    }

    public override void Enter(Image regionImage, PointerEventData eventData)
    {

        base.Enter(regionImage,eventData);
    }

    public override void Update(PointerEventData eventData , GameObject exploreButton = null)
    {
        

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ShowExploreButton(exploreButton);
            // leftClick.Invoke();
            Debug.Log("Left Clicked");
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            //  middleClick.Invoke();
            Debug.Log("Middle Clicked");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // rightClick.Invoke();
            // CloseExplorePanel();
            Debug.Log("Right Clicked");
        }

        //if (!CanAttackPlayer())
        //{
        //    nextState = new Idle(npc, agent, anim, player);
        //    stage = EVENT.EXIT;
        //}
    }

    public override void Exit(PointerEventData eventData)
    {
        base.Exit(eventData);
    }

    private void ShowExploreButton(GameObject exploreButton)
    {
        exploreButton.transform.position = Input.mousePosition;
        exploreButton.SetActive(true);
        //instantExploreButton = Instantiate(exploreQuestionButton, Input.mousePosition, Quaternion.identity);
        //instantExploreButton.gameObject.transform.SetParent(exploreButtonParent);
        //uButtonExploreScript exploreButton = instantExploreButton.GetComponent<uButtonExploreScript>();
        //exploreButton.In(this.regionControler);
        //  Button button = this.instantExploreButton.GetComponent<Button>();
    }
}