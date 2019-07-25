using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{

    public static event Action<int> OnTimeChanged = delegate { };
    public static event Action<int> OnTimeSpeedChanged = delegate { };

    private readonly List<int> timePointsPerSecond = new List<int>() {0, 10, 60, 120, 1200, 7200};
    private int timeSpeed = 1; //0 = paused, 1 = very slow etc

    private int TimeSpeed
    {
        get => timeSpeed;
        set
        {
            if (timeSpeed == value)
                return;
            timeSpeed = value;
            OnTimeSpeedChanged(value);
        }
    }
    private float timePointsDelta = 0;


    void Update()
    {
        //time points since last frame
        timePointsDelta += Time.deltaTime * TimePointMultiplier();
        if (timePointsDelta > 1f)
        {
            OnTimeChanged((int) timePointsDelta);
            timePointsDelta %= 1;
        }
        CheckInput();
    }

    public void IncreaseTime()
    {
        if (TimeSpeed == 0) return;
        if (TimeSpeed != timePointsPerSecond.Count - 1)
        {
            TimeSpeed++;
        }
    }

    public void DecreaseTime()
    {
        if (TimeSpeed > 1)
        {
            TimeSpeed--;
        }
    }

    private static int unPaused = 1; //for restoring to original speed after unpausing

    public void Pause()
    {
        if (TimeSpeed != 0)
        {
            unPaused = TimeSpeed;
            TimeSpeed = 0;
        }
        else
        {
            TimeSpeed = unPaused;
        }
    }

    public int TimePointMultiplier()
    {
        return timePointsPerSecond[TimeSpeed];
    }

    private void CheckInput()
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
}
