using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
public class Dark : State
{


    public Dark(Region region): base(region)
    {
        //name = STATE.ATTACK;

    }

    public override void Enter(PointerEventData eventData)
    {
        if(region.IsStartingRegion)
        {
            region.RevealNeighbors();
        }

        base.Enter(eventData);
    }

    public override void Update(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            
   
            DoyouWannaExplore();
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
            CloseExplorePanel();
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

    public void DoyouWannaExplore()
    {
        //instantExploreButton = Instantiate(exploreQuestionButton, Input.mousePosition, Quaternion.identity);
        //instantExploreButton.gameObject.transform.SetParent(exploreButtonParent);
        //uButtonExploreScript exploreButton = instantExploreButton.GetComponent<uButtonExploreScript>();
        //exploreButton.In(this.regionControler);
        //  Button button = this.instantExploreButton.GetComponent<Button>();

    }
}