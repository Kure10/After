using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class State
{
    public enum STATE
    {
        DARK, SMOKE, EXPLORE,
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE state;
    protected EVENT stage;
  //  protected Animator anim;
    protected State nextState;
    protected Region region;

    [Header("Colors")]
    protected Color shadowColor = new Color(1, 1, 1, 0.25f);
    protected Color darkColor = new Color(0, 0, 0, 1);
    protected Color defaultColor = new Color(1, 1, 1, 1);


    public State(Region _region, GameObject exploreButton = null)
    {
        stage = EVENT.ENTER;
        region = _region;
    }

    public virtual void Enter(Image regionImage, PointerEventData eventData = null)
    {
        if (eventData == null)
        {
            RegionSetUp(regionImage);
            stage = EVENT.EXIT;
        }
        else
        {
            stage = EVENT.UPDATE;
        }
    }

    public virtual void Update(PointerEventData eventData = null, GameObject exploreButton = null) { stage = EVENT.UPDATE; }
    public virtual void Exit(PointerEventData eventData = null) { stage = EVENT.EXIT; }


    public State Process(Image regionImage, GameObject exploreButton = null, PointerEventData eventData = null)
    {
        if (stage == EVENT.ENTER) Enter(regionImage, eventData);
        if (stage == EVENT.UPDATE) Update(eventData, exploreButton);
        if (stage == EVENT.EXIT)
        {
            Exit(eventData);
            return nextState;
        }
        return this;
    }

    private void RegionSetUp(Image image)
    {
        image.color = darkColor;
    }



}

