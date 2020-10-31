﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public List<StatsClass> allEvents = new List<StatsClass>();


    public StatsClass ChoiseRandomEvent(int minDifficulty, int maxDifficulty, List<Terrain> occurringTerrains)
    {
        // tady vyberu random event podle parametru..

        // ted vybíram jeden porad..

        long id = 20203890660776;

        StatsClass statclass = new StatsClass();

        foreach (var item in allEvents)
        {
            if (item.Title == id.ToString())
            {
                statclass = item;
            }
        }

        return statclass;
    }

}