using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTime : MonoBehaviour
{

    public TimeControl timeControler;

    public Text textTime , textDays;
    public Text speedStatus;

    private float gameTimer;

    private readonly List<string> listOfTimeStatus = new List<string>() { "Very slow", "Slow", "Normal", "Fast", "Very fast" };
    private readonly List<int> listOfSpeed = new List<int>() { 5 , 30 , 60 , 600 , 3600 };
    private int timeMultiplier;



    // Start is called before the first frame update
    void Start()
    {

        ChangeMultiplier();
        DisplayStatus();
        gameTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        // Time.timeScale = multiplier;
        if(timeMultiplier <= 0)
        {
            timeMultiplier = 10;
        }

        gameTimer = (gameTimer + Time.deltaTime * timeMultiplier);
       
        DisplayTime();
    }


    public void DisplayTime()
    {
        int seconds = (int)(gameTimer % 60);
        int minutes = (int)(gameTimer / 60) % 60;
        int hours = (int)(gameTimer / 3600) % 24;
        int days = (int)(gameTimer / 86400) % 365; // ToDo asi bude potreba vice dnu .. to pak musim poresit

        string sec = seconds.ToString();
        string min = minutes.ToString();
        string hour = hours.ToString();
        string dayS = days.ToString();


        if (seconds <= 9)
        {
            sec = 0 + gameTimer.ToString("F0");
        }
        if (minutes <= 9)
        {
            min = 0 + minutes.ToString();
        }
        if (hours <= 9)
        {
            hour = 0 + hours.ToString();
        }

        //time
        if (seconds <= 9)
        {
            textTime.text = hour + ":" + min + ":" + 0 + seconds.ToString("F0");
        }
        else
        {
            textTime.text = hour + ":" + min + ":" + sec;
        }

        // days
        textDays.text = dayS;


    }

    public void DisplayStatus()
    {
        speedStatus.text = listOfTimeStatus[timeControler.GetTimeStep()];
        timeMultiplier = listOfSpeed[timeControler.GetTimeStep()];
    }

    public void ChangeMultiplier()
    {
        timeMultiplier = timeControler.TimePointMultiplier();
    }

}
