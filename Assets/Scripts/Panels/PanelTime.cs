using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTime : MonoBehaviour
{

    //[Header("Setup")]
    public Text textTime, textDays;
    public Text speedStatus;

    private UInt32 gameTimer;
    private readonly List<string> listOfTimeStatus = new List<string>() { "Paused", "Very slow", "Slow", "Normal", "Fast", "Very fast" };

    
    // Start is called before the first frame update
    void Start()
    {
        TimeControl.OnTimeChanged += TimeChanged;
        TimeControl.OnTimeSpeedChanged += DisplayStatus;
        DisplayStatus(1);
        gameTimer = 0;
    }

    void OnDestroy()
    {
        TimeControl.OnTimeChanged -= TimeChanged;
        TimeControl.OnTimeSpeedChanged -= DisplayStatus;

    }

    private int timeRemain = 0;
    private void TimeChanged(int timePoints)
    {
        //10 timePoints = 5s
        var tpToAdd = timePoints + timeRemain;
        gameTimer += (uint) (tpToAdd / 2);
        timeRemain = tpToAdd % 2;
        DisplayTime();
    }



    public void DisplayTime()
    {
        int seconds = (int)(gameTimer % 60);
        int minutes = (int)(gameTimer / 60) % 60;
        int hours = (int)(gameTimer / 3600) % 24;
        int days = (int)(gameTimer / 86400); // ToDo asi bude potreba vice dnu .. to pak musim poresit
                                             // -- bych neresil, klidne muzeme zobrazovat den 400 a neresit roky, Ashen

        string sec = seconds.ToString("D2");
        string min = minutes.ToString("D2");
        string hour = hours.ToString("D2");
        string dayS = days.ToString();

        
        textTime.text = $"{hour}:{min}:{sec}";
        // days
        textDays.text = dayS;
    }

    private void DisplayStatus(int speed)
    {
        speedStatus.text = listOfTimeStatus[speed];
    }

}
