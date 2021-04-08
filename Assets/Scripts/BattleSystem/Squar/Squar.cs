using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class MyIntEvent : UnityEvent<Squar>
{
    public bool isInRange = false;

    public bool isTesting2 = false;
}

public class Squar : MonoBehaviour
{

    public int yCoordinate = 0;
    public int xCoordinate = 0;

    [SerializeField] public GameObject container;

    [SerializeField] public GameObject inRangeBackground;
    [SerializeField] public GameObject inAttackRangeBackground;

    public Unit unitInSquar;

    public bool isVisited = false;

    public MyIntEvent  m_MyEvent;


    // todo mrkni se jak to funguje..
    public Action<int> asdasd;

    public void SetCoordinates(int x, int y)
    {
        xCoordinate = x;
        yCoordinate = y;

        this.name = $"Squar  {x}:{y}";
    }

    // Events actions

    public void ShowCircle ()
    {
        inAttackRangeBackground.SetActive(true);
    }
    public void InitEvent(UnityAction<Squar> call)
    {
        if (m_MyEvent == null)
            m_MyEvent = new MyIntEvent();

        m_MyEvent.AddListener(call);
    }

    public void IneitEvent(UnityAction<Squar> call)
    {
        if (m_MyEvent == null)
            m_MyEvent = new MyIntEvent();

        m_MyEvent.AddListener(call);
    }

    public void OnPointerEnter()
    {
        m_MyEvent.Invoke(this);

        
        var tmp = m_MyEvent.isInRange;
        var tmp2 = m_MyEvent.isTesting2;
        // inAttackRangeBackground.SetActive(true);
    }

    public void OnPointerExit ()
    {
        inAttackRangeBackground.SetActive(false);
    }


}


