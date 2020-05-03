using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class PanelTime : MonoBehaviour
{

    //[Header("Setup")]
    public Text textTime, textDays, textTimeSmall , TextDaysSmall;
    public Text speedStatus;
    public Button pauseButton;
    private int _timeStatus;
    private bool paused;
    private float blinkingTime = 0f;
    private int timeStatus
    {
        get => _timeStatus;
        set
        {
            if (value == _timeStatus) return;
            _timeStatus = value;
            DisplayStatus(value);
        }
    }
    private UInt32 gameTimer;
    private readonly List<string> listOfTimeStatus = new List<string>() { "Very slow", "Slow", "Normal", "Fast", "Very fast" };


    // Start is called before the first frame update
    void Start()
    {
        TimeControl.OnTimeChanged += TimeChanged;
        timeStatus = 0;
        DisplayStatus(timeStatus);
        gameTimer = 0;
        paused = false;
    }

    void Update()
    {
        CheckInput();
        if (paused)
        {
            blinkingTime += Time.deltaTime;
            bool enabled = false;
            if (blinkingTime < 0.5f)
            {
                enabled = true;
            }
            else if (blinkingTime > 1f)
            {
                blinkingTime = 0f;
            }
            pauseButton.GetComponent<Image>().CrossFadeAlpha(enabled ? 0 : 1, 0.2f, true);
        }
        else
        {
            if (pauseButton != null) // Tohle jsem pridal jenom aby me to nehazelo error... V mojí scene.
                pauseButton.GetComponent<Image>().CrossFadeAlpha(1, 0.2f, true);
        }
    }
    void OnDestroy()
    {
        TimeControl.OnTimeChanged -= TimeChanged;

    }

    public void Pause()
    {
        if (TimeControl.IsTimeBlocked)
            return;

        blinkingTime = 0;
        paused = !paused;

        if (paused)
        {
            TimeControl.SetTime(0);
        }
        else
        {

            TimeControl.SetTime(timeStatus + 1);
        }
    }

    public void IncreaseTime()
    {
        if (timeStatus < listOfTimeStatus.Count - 1)
        {
            timeStatus++;
        }

        if (!paused)
        {
            TimeControl.SetTime(timeStatus + 1);
        }
    }

    public void DecreaseTime()
    {
        if (timeStatus > 0)
        {
            timeStatus--;
        }
        if (!paused)
        {
            TimeControl.SetTime(timeStatus + 1);
        }
    }
    private int timeRemain = 0;
    private void TimeChanged(int timePoints)
    {
        //10 timePoints = 5s
        var tpToAdd = timePoints + timeRemain;
        gameTimer += (uint)(tpToAdd / 2);
        timeRemain = tpToAdd % 2;
        DisplayTime();
    }



    public void DisplayTime()
    {
        int seconds = (int)(gameTimer % 60);
        int minutes = (int)(gameTimer / 60) % 60;
        int hours = (int)(gameTimer / 3600) % 24;
        int days = (int)(gameTimer / 86400); 
                                             

        string sec = seconds.ToString("D2");
        string min = minutes.ToString("D2");
        string hour = hours.ToString("D2");
        string dayS = days.ToString();


        textTime.text = $"{hour}:{min}:{sec}";
        textTimeSmall.text = $"{hour}:{min}:{sec}";
        // days
        textDays.text = dayS;
        TextDaysSmall.text = dayS;
    }

    private void DisplayStatus(int speed)
    {

        speedStatus.text = listOfTimeStatus[speed];
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
