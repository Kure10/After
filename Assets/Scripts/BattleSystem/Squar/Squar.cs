using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

[System.Serializable]
public class CursorColorEvent : UnityEvent<Squar>
{
    public bool isInMoveRange = false;
    public bool canAttack = false;
}

public class Squar : MonoBehaviour
{

    public int yCoordinate = 0;
    public int xCoordinate = 0;

    [SerializeField] public GameObject container;

    [SerializeField] public GameObject inRangeBackground;
    [SerializeField] public GameObject canAttackMark;

    [Header("Attack Range Borders")]
    [SerializeField] public GameObject leftBorder;
    [SerializeField] public GameObject rightBorder;
    [SerializeField] public GameObject upBorder;
    [SerializeField] public GameObject downBorder;

    [Space]

    public Unit unitInSquar;

    public bool isVisited = false;
    public bool isInReach = false;

    public CursorColorEvent CursorEvent;

    [Header("Color for Curzor")]
    public Color isInMoveRangeColor;
    public Color isOutOfMoveRangeColor;
    public Color canAttackColor;

    public Action action;

    public void SetCoordinates(int x, int y)
    {
        xCoordinate = x;
        yCoordinate = y;

        this.name = $"Squar  {x}:{y}";
    }

    // Events actions

    public void InitEvent(UnityAction<Squar> call)
    {
        if (CursorEvent == null)
            CursorEvent = new CursorColorEvent();

        CursorEvent.AddListener(call);
    }

    public void DisableAttackBorders()
    {
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);
        upBorder.SetActive(false);
        downBorder.SetActive(false);
    }

    // Event Trigger from Unity
    public void OnPointerEnter()
    {
        CursorEvent.Invoke(this);
        var cursorImg = canAttackMark.GetComponent<Image>();

        if (cursorImg == null)
            return;

        if (CursorEvent.canAttack)
        {
            cursorImg.color = canAttackColor;
        }
        else
        {
            if (CursorEvent.isInMoveRange)
            {
                cursorImg.color = isInMoveRangeColor;
            }
            else
            {
                cursorImg.color = isOutOfMoveRangeColor;
            }
        }

        canAttackMark.SetActive(true);
    }

    // Event Trigger from Unity
    public void OnPointerExit ()
    {
        canAttackMark.SetActive(false);
    }


}


