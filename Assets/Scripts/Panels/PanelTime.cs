using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enviroment;

public class PanelTime : MonoBehaviour
{

    public Text textTime, textDays, textTimeSmall , TextDaysSmall;
    public Text speedStatus;
    public Button pauseButton;
    private int _timeStatus;
    private bool paused;
    private float blinkingTime = 0f;

    [SerializeField] EventController eventController;

    [SerializeField] Text CelsiumText;

    private bool wasPauseBefore;

    public bool IsPaused { get { return this.paused; } }
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

    private readonly List<string> listOfTimeStatus = new List<string>() { "Very slow", "Slow", "Normal", "Fast", "Very fast" };

    // Start is called before the first frame update
    void Start()
    {
        TimeControl.OnDateChanged += DisplayTime;
        timeStatus = 0;
        DisplayStatus(timeStatus);
        paused = false;
        MeteorologyController.OnTemperatureChanged += DisplayTemperature;
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

    public void Pause(bool forcePause = false)
    {
        if (MenuControler.isInMenu == true) return;

        if (PanelControler.isPopupOpen == true) return;

        if(EventController.isEventRunning)
        {
            eventController.Maximaze();
            return;
        }


        if (forcePause) 
        {
            blinkingTime = 0;
            paused = true;
            TimeControl.IsTimeBlocked = true;
            TimeControl.SetTime(0);
        }

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
            wasPauseBefore = false;
        }
    }

    public void PauseGame(bool intoMenu = false, bool intoPopup = false)
    {
        if(intoMenu)
            MenuControler.isInMenu = true;

        if(!PanelControler.isPopupOpen && IsPaused)
            wasPauseBefore = true;

        if (intoPopup)
            PanelControler.isPopupOpen = true;

        if (TimeControl.IsTimeBlocked)
            return;

        if (IsPaused)
        {
            if (PanelControler.isPopupOpen)
                return;

            wasPauseBefore = true;
        }
        else
        {
            TimeControl.SetTime(0);
            paused = true;
        }
    }

    public void UnpauseGame(bool fromMenu = false, bool fromPopup = false)
    {
        if (fromMenu)
            MenuControler.isInMenu = false;

        if (fromPopup)
            PanelControler.isPopupOpen = false;

        if (MenuControler.isInMenu || PanelControler.isPopupOpen || TimeControl.IsTimeBlocked)
            return;



        if (IsPaused)
        {
            if(wasPauseBefore)
            {
                this.paused = true;
            }
            else
            {
                TimeControl.SetTime(timeStatus + 1);
                this.paused = false;
                wasPauseBefore = false;
            }
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

    public void DisplayTime(TimeControl.GameDateTime gameDate)
    {
        string sec = gameDate.sec.ToString("D2");
        string min = gameDate.Min.ToString("D2");
        string hour = gameDate.Hours.ToString("D2");
        string dayS = gameDate.Day.ToString();


        textTime.text = $"{hour}:{min}:{sec}";
        textTimeSmall.text = $"{hour}:{min}:{sec}";
        // days

        textDays.text = $"{gameDate.GetMonth}  {gameDate.GetDayInMonth}";
        TextDaysSmall.text = $"{gameDate.GetMonth}  {gameDate.GetDayInMonth}";
    }

    private void DisplayTemperature(float celsium)
    {
        CelsiumText.text = celsium.ToString("0.0");
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

    void OnDestroy()
    {
        TimeControl.OnDateChanged -= DisplayTime;
        MeteorologyController.OnTemperatureChanged -= DisplayTemperature;
    }

}
