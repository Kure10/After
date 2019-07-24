﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    private readonly List<int> timePointsPerSecond = new List<int>() {10, 60, 120, 1200, 7200};  //  {10, 60, 120, 1200, 7200};
    private int TimeStep = 1;

    void Update()
    {
        //For testing only, keycode shouldn't be hardcoded
        //TODO read from keymap settings
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            IncreaseTime();
        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            DecreaseTime();
        }
    }

    public void IncreaseTime()
    {
        if (TimeStep != timePointsPerSecond.Count - 1)
        {
            TimeStep++;
        }
    }

    public void DecreaseTime()
    {
        if (TimeStep != 0)
        {
            TimeStep--;
        }
    }

    private static int unPaused = 1;
    public void Pause()
    {
        if (TimeStep != 0)
        {
            unPaused = TimeStep;
            TimeStep = 0;
        }
        else
        {
            TimeStep = unPaused;
        }
    }

    public int TimePointMultiplier()
    {
        return timePointsPerSecond[TimeStep];
    }

    // pridal jsem funkci na vraceni TimeStep
    public int GetTimeStep()
    {
        return TimeStep;
    }
}