using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Explore : State
{

    public Explore(Region region) : base(region)
    {
        state = STATE.EXPLORE;
    }

    public override void Enter(Image regionImage, PointerEventData eventData)
    {

        base.Enter(regionImage,eventData);
    }

    public override void Update(PointerEventData eventData, GameObject exploreButton)
    {

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
}