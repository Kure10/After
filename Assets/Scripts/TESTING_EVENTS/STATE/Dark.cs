using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dark : State
{


    public Dark(Region region): base(region)
    {
        state = STATE.DARK;
    }

    public override void Enter(Image regionImage, PointerEventData eventData)
    {
        base.Enter(regionImage, eventData);
        if (region.IsStartingRegion)
        {
            regionImage.color = shadowColor;
            this.nextState = new Smoke(region);
        }
    }

    public override void Update(PointerEventData eventData, GameObject exploreButton)
    {
        this.nextState = new Dark(region);
        this.stage = EVENT.EXIT;
    }

    public override void Exit(PointerEventData eventData)
    {
        base.Exit(eventData);
    }
}