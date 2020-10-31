using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventContent 
{
    public int evocationTime;

    private bool isEventFinished = false;

    public bool wasTriggered;

    private List<StatsClass> curentEventInfo = new List<StatsClass>();

    public bool IsEventFinished { get { return this.isEventFinished; } set { this.isEventFinished = value; } }

    public void AddStatClass(StatsClass statClass)
    {
        curentEventInfo.Add(statClass);
    }


}
