using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTime : MonoBehaviour
{

    
    private int multiplayer = 1;

    public TimeControl timeControler;

    public Text textTime , textDays;
    public Text speedStatus;

    private float secs;
    private int mins, hours, days;

    private readonly List<string> listOfTimeStatus = new List<string>() { "Very slow", "Slow", "Normal", "Fast", "Very fast" };
    public int multiplier; 

    // Start is called before the first frame update
    void Start()
    {
        
        multiplier = timeControler.TimePointMultiplier();
        DisplayStatus();
        secs = 0;
        days = 0;
    }

    // Update is called once per frame
    void Update()
    {

        Time.timeScale = multiplier;

        secs = (secs + Time.deltaTime);

        DisplayTime();

    }


    public void DisplayTime()
    {
        TimeCalculation();

        string sec = secs.ToString("F0");
        string min = mins.ToString();
        string hour = hours.ToString();

        if (secs < 9)
        {
            sec = 0 + secs.ToString("F0");
        }
        if (mins < 9)
        {
            min = 0 + mins.ToString();
        }
        if (hours < 9)
        {
            hour = 0 + hours.ToString();
        }

        // time
        textTime.text = hour + ":" + min + ":" + sec;
        // days
        textDays.text = days.ToString();
    }

    private void TimeCalculation()
    {
        if (secs > 59)
        {
            secs = 0;
            mins++;
        }

        if (mins > 59)
        {
            mins = 0;
            hours++;
        }

        if (hours > 59)
        {
            days++;
        }
    }

    public void DisplayStatus()
    {
        speedStatus.text = listOfTimeStatus[timeControler.GetTimeStep()];
    }

    public void ChangeMultiplier()
    {
        multiplier = timeControler.TimePointMultiplier();
    }
}
