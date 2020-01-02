using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

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

    public STATE name;
    protected EVENT stage;
  //  protected Animator anim;
    protected State nextState;
    protected Region region;

    public State(Region _region)
    {
        stage = EVENT.ENTER;
        region = _region;
    }

    public virtual void Enter(PointerEventData eventData) { stage = EVENT.UPDATE; }
    public virtual void Update(PointerEventData eventData) { stage = EVENT.UPDATE; }
    public virtual void Exit(PointerEventData eventData) { stage = EVENT.EXIT; }

    public State Process(PointerEventData eventData)
    {
        if (stage == EVENT.ENTER) Enter(eventData);
        if (stage == EVENT.UPDATE) Update(eventData);
        if (stage == EVENT.EXIT)
        {
            Exit(eventData);
            return nextState;
        }
        return this;
    }
}

