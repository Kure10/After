using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{

    public static event Action<int> OnTimeChanged = delegate { };
    public static event Action<int> OnTimeSpeedChanged = delegate { };

    private static readonly List<int> timePointsPerSecond = new List<int>() {0, 10, 60, 120, 1200, 7200};
    private static int timeSpeed = 1; //0 = paused, 1 = very slow etc

    private static int TimeSpeed
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
    }
    public static void SetTime(int newTime)
    {
        if (newTime >= 0 && newTime < timePointsPerSecond.Count)
        {
            TimeSpeed = newTime;
        }
    }


    public int TimePointMultiplier()
    {
        return timePointsPerSecond[TimeSpeed];
    }

    
}
