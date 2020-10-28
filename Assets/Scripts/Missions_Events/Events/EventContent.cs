using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventContent 
{
    public int evocationTime;

    public bool isEventFinished = false;

    public bool wasTriggered;

    private List<StatsClass> curentEventInfo = new List<StatsClass>();

    public void AddStatClass(StatsClass statClass)
    {
        curentEventInfo.Add(statClass);
    }


}
