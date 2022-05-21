using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{

    // Todo Možna by to chtelo dat Delegaty do specialni funkce aby měli pořadí volání ? ??
    // Delegaty jsou seřazene podle vykonavání
    public static event Action<GameDateTime> OnGameStart = delegate { };
    public static event Action<GameDateTime> OnTimeChangedEvery10Mins = delegate { };
    public static event Action<GameDateTime> OnTimeChangedNewDayMidnight = delegate { };
    public static event Action<GameDateTime> OnTimeChangedNoon = delegate { };
    public static event Action<GameDateTime> OnTimeChangedEveryHour = delegate { };
    public static event Action<GameDateTime> OnTimeChangedNewMonth = delegate { };
    public static event Action<int> OnTimeChanged = delegate { };
    public static event Action<GameDateTime> OnDateChanged = delegate { };
    public static event Action<int> OnTimeSpeedChanged = delegate { };

    private static readonly List<int> timePointsPerSecond = new List<int>() { 0, 10, 60, 120, 1200, 7200 };
    private static int timeSpeed = 1; //0 = paused, 1 = very slow etc

    private static bool isTimeBlocked = false;

    private static readonly Dictionary<GameDateTime.Months, int> _relevantDaysInMounths = new Dictionary<GameDateTime.Months, int>()
    {
        {GameDateTime.Months.January, 31},
        {GameDateTime.Months.February, 28},
        {GameDateTime.Months.March, 31 },
        {GameDateTime.Months.April, 30 },
        {GameDateTime.Months.May, 31 },
        {GameDateTime.Months.June, 30},
        {GameDateTime.Months.July, 31},
        {GameDateTime.Months.August, 31},
        {GameDateTime.Months.September, 30},
        {GameDateTime.Months.October, 31},
        {GameDateTime.Months.November, 30},
        {GameDateTime.Months.December, 31},
    };

    public int TimePointMultiplier { get { return timePointsPerSecond[TimeSpeed]; } }

    private GameDateTime _gameTime = null;

    #region Properities

    public static bool IsTimeBlocked
    {
        get { return isTimeBlocked; }
        set {
            if (isTimeBlocked = value)
                return;

            isTimeBlocked = value;
        }
    }

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

    #endregion

    public static int GetAmountDaysInMonth(GameDateTime.Months month)
    {
        return _relevantDaysInMounths[month];
    }

    private void Awake()
    {
        // Todo Bug .. Den 0 ale měl by byt spravně den 1 atd... Od 1 počítat dny stejně asi roky..
        _gameTime = new GameDateTime(0, 0, 0, 1, 0, (GameDateTime.Months)5);
        RecalculateStartTime();
    }

    private void Start()
    {
        OnGameStart.Invoke(_gameTime);
    }

    void Update()
    {
        //time points since last frame
        timePointsDelta += Time.deltaTime * TimePointMultiplier;
        if (timePointsDelta > 1f)
        {
            CalcuateSecDayMinHoursDays((int)timePointsDelta);
            OnTimeChanged((int)timePointsDelta);
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

    private UInt32 gameTimer = 0;
    private int timeRemain = 0;
    private void CalcuateSecDayMinHoursDays(int timePointsDelta)
    {
        // Todo proč se tu to deli dvojkou  ?  2 
        var tpToAdd = timePointsDelta + timeRemain;
        gameTimer += (uint)(tpToAdd / 2);
        timeRemain = tpToAdd % 2;

        _gameTime.sec = (int)(gameTimer % 60);
        _gameTime.Min = (int)(gameTimer / 60) % 60;
        _gameTime.Hours = (int)(gameTimer / 3600) % 24;
        _gameTime.Day = (int)(gameTimer / 86400) % 365;
        _gameTime.years = (int)(gameTimer / 31536000);

        ActionsInvoke();
    }

    private void RecalculateStartTime()
    {
        int timePoints = _gameTime.sec;
        timePoints += _gameTime.Min * 60;
        timePoints += _gameTime.Hours * 3600;
        timePoints += _gameTime.Day * 86400;
        timePoints += _gameTime.years * 31536000;
        timePointsDelta = 2 * timePoints;
    }

    // Todo Actions Are called more time when speed is high. I dont know what to do about it. Right now.
    private void ActionsInvoke()
    {
        if (_gameTime.IsNew10Minites)
            OnTimeChangedEvery10Mins.Invoke(_gameTime);

        if (_gameTime.Hours == 0 && _gameTime.Min == 0)
            OnTimeChangedNewDayMidnight.Invoke(_gameTime);

        if (_gameTime.Hours == 12 && _gameTime.Min == 0)
            OnTimeChangedNoon.Invoke(_gameTime);

        if (_gameTime.IsNewHour)
            OnTimeChangedEveryHour.Invoke(_gameTime);

        if (_gameTime.IsNewDayInMonth)
            OnTimeChangedNewMonth.Invoke(_gameTime);

        OnDateChanged.Invoke(_gameTime);
    }


    public class GameDateTime
    {
        public int sec = 0;
        private int min = 0;
        private int hours = 0;
        private int day = 0;
        public int years = 0;

        private int month = (int)Months.May;
        private int dayInMonth = 1;
        private Season season = Season.Winter;

        private bool isNewDayStarted = false;
        private bool isNewDayInMonth = false;
        private bool isNewHour = false;
        private bool isNew10Minites = false;

        public Months GetMonth { get { return (Months)month; } }
        public int GetDayInMonth { get { return dayInMonth; } }
        public Season  GetSeason  { get { return season; } }
        public bool IsNewDayStarted { get { return isNewDayStarted; } }
        public bool IsNewDayInMonth { get { return isNewDayInMonth; } }
        public bool IsNewHour { get { return isNewHour; } }
        public bool IsNew10Minites { get { return isNew10Minites; } }
        public int Min
        {
            get
            {
                return min;
            }

            set
            {
                if (value != min)
                {
                    min = value;
                    if (min % 10 == 0)
                    {
                        isNew10Minites = true;
                    }
                    else
                    {
                        isNew10Minites = false;
                    }
                }
                else
                {
                    isNew10Minites = false;
                }
            }
        }
        public int Hours
        {
            get
            {
                return hours;
            }

            set
            {
                if (value != hours)
                {
                    hours = value;
                    isNewHour = true;
                }
                else
                {
                    isNewHour = false;
                }
            }
        }

        public int Day 
        {
            get 
            {
                return day;
            }

            set 
            {
                if (value != day)
                {
                    day = value;
                    isNewDayStarted = true;
                    RecalculateCurrentDaysInMonth(day);
                }
                else
                {
                    isNewDayStarted = false;
                }
            } 
        }

        public GameDateTime(int _sec , int _min , int _hours, int _days, int _years, Months _month)
        {
            sec = _sec;
            min = _min;
            hours = _hours;
            day = _days;
            years = _years;
            month = (int)_month;

            dayInMonth = day;

            RecalculateSeason((Months)month);
        }

        private void RecalculateCurrentDaysInMonth(int day)
        {
            int totalDaysInMonth = _relevantDaysInMounths[(Months)month];

            if (day > totalDaysInMonth)
            {
                dayInMonth = 1;
                month++;
                isNewDayInMonth = true;

                if (month > Enum.GetValues(typeof(Months)).Length)
                {
                    month = 1;
                }

                RecalculateSeason((Months)month);
            }
            else
            {
                dayInMonth = dayInMonth + 1;
                isNewDayInMonth = false;
            }
        }

        private void RecalculateSeason(Months month)
        {
            if(month == Months.January || month == Months.February || month == Months.December)
                season = Season.Winter;
            else if(month == Months.March || month == Months.April || month == Months.May)
                season = Season.Spring;
            else if(month == Months.June || month == Months.July || month == Months.August)  
                season = Season.Summer;
            else
                season = Season.Autumn;

            Debug.Log($"{season}: {GetMonth}  {GetDayInMonth} -> days in game {day}");
        }

        public enum Months
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public enum Season
        {
            Winter = 1,
            Autumn,
            Summer,
            Spring
        }
    }
}
