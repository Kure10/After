using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
public class Explore : State
{

    public Explore(Region region) : base(region)
    {
        
    }

    public override void Enter(PointerEventData eventData)
    {

        base.Enter(eventData);
    }

    public override void Update(PointerEventData eventData)
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