using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTime : MonoBehaviour
{

    public int multiplayer = 1;

    public TimeControl timeControler;

    public Text textTime;
    public Text speedStatus;

    private float secs;
    private int mins, hours;

    private readonly List<string> listOfTimeStatus = new List<string>() { "Very slow", "Slow", "Normal", "Fast", "Very fast" };
    private int multiplier; 

    // Start is called before the first frame update
    void Start()
    {
        multiplier = timeControler.TimePointMultiplier();
        DisplayStatus();
        secs = 0;
    }

    // Update is called once per frame
    void Update()
    {

        secs = (secs + Time.deltaTime) * multiplier;

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

        textTime.text = hour + ":" + min + ":" + sec;
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
            //TODO  pokud prekroci 24 hodin. Novy den a pak tyden ci mesic.. ?
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
