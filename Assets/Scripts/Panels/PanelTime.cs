using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTime : MonoBehaviour
{

    //[Header("Setup")]
    public Text textTime, textDays;
    public Text speedStatus;
    public TimeControl timeControler;

    private float timeScale;
    private int timeMultiplier;
    private bool isRunning = true;

    private float gameTimer;
    private readonly List<float> listOfTimescale = new List<float>() { 0.5f, 1, 10, 25, 50 };
    private readonly List<string> listOfTimeStatus = new List<string>() { "Very slow", "Slow", "Normal", "Fast", "Very fast" };
   
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
        if(isRunning)
        Time.timeScale = timeScale;

        if (timeScale < 1)
        {
            gameTimer = (gameTimer + Time.deltaTime * timeMultiplier);
        }
        else
        {
            gameTimer = (gameTimer + Time.deltaTime * (timeMultiplier / (2 * timeScale)));
        }
       
       
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
        timeScale = listOfTimescale[timeControler.GetTimeStep()];
    }

    public void ChangeMultiplier()
    {
        timeMultiplier = timeControler.TimePointMultiplier();
    }

    public void PauseOrUnpause ()
    {
        if(Time.timeScale == 0)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
            Time.timeScale = 0;
        } 
    }

}
