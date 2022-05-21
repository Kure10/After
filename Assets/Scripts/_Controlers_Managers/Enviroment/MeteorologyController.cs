using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Enviroment
{
    // TODO  pokud cas beží max rychlostí tak se z nejakých duvodu vola zmena teploty  36 a ne 6..   Možna performace issue do budoucna..
    //  To je jedina rychlost kde je problem ... Proc nevím
    public class MeteorologyController : MonoBehaviour
    {
        [Header("Temperature settings")]
        [SerializeField] TemperatureGlobalSettings tempGlobalSettings;
        [SerializeField] float rngOscilationMinRange = -1f;
        [SerializeField] float rngOscilationMaxRange = 1f;
        [Space]
        [Header("Weather Settings")]
        [SerializeField] private List<Weather> weatherPool = new List<Weather>();
        [SerializeField] private WeatherSettings weatherSettings;

        public static event Action<float> OnTemperatureChanged = delegate { };

        private readonly Dictionary<int, float> _temperatureGraf = new Dictionary<int, float>()
        {
            {0, 3.5f},{1, 3.0f},{2, 2.5f},{3, 2f}, {4, 1.5f}, {5, 1f},
            {6, 0f}, {7, 0.5f}, {8, 1.5f},{9, 3f}, {10, 4.5f}, {11, 6.5f},
            {12, 8.5f},{13, 9.5f}, {14, 11f},{15, 12f},{16, 12.1f}, {17, 11.6f},
            {18, 10f}, {19, 9f}, {20, 8f}, {21, 7f}, {22, 6f}, {23, 5f},
        };

        private List<TemperatureGlobalSettings.TemperatureMonthSettings> _temperatureMonthSettings = new List<TemperatureGlobalSettings.TemperatureMonthSettings>();
        private TemperatureGlobalSettings.TemperatureMonthSettings _currentMonthSettings = null;
        private TemperatureGlobalSettings.TemperatureMonthSettings _previousMonthSettings = null;
        private TemperatureGlobalSettings.TemperatureMonthSettings _nextMonthSettings = null;

        private float _oscillationDayTemperature = 0;
        private static float _currentTemperature = 0;

        private WeatherMonthSettings _weatherMonthSettings = null;
        private WeatherMonthSettings.WeatherReadyToStart _currentWeather = null;

        public static float GetTemperatureInCelsia { get { return _currentTemperature; } }

        private void Awake()
        {
            _temperatureMonthSettings.Add(tempGlobalSettings.January);
            _temperatureMonthSettings.Add(tempGlobalSettings.February);
            _temperatureMonthSettings.Add(tempGlobalSettings.March);
            _temperatureMonthSettings.Add(tempGlobalSettings.April);
            _temperatureMonthSettings.Add(tempGlobalSettings.May);
            _temperatureMonthSettings.Add(tempGlobalSettings.June);
            _temperatureMonthSettings.Add(tempGlobalSettings.July);
            _temperatureMonthSettings.Add(tempGlobalSettings.August);
            _temperatureMonthSettings.Add(tempGlobalSettings.September);
            _temperatureMonthSettings.Add(tempGlobalSettings.October);
            _temperatureMonthSettings.Add(tempGlobalSettings.November);
            _temperatureMonthSettings.Add(tempGlobalSettings.December);
        }
        private void OnEnable()
        {
            TimeControl.OnGameStart += MeteorologyEventsOnStart;
            TimeControl.OnTimeChangedEvery10Mins += MeteorologyEventsEvery10Mins;
            TimeControl.OnTimeChangedNewMonth += CalculateWeatherForNextMonth;
            TimeControl.OnTimeChangedEveryHour += MeteorologyEventsEveryHour;
        }

        private void Start()
        {
            _oscillationDayTemperature = UnityEngine.Random.Range(rngOscilationMinRange, rngOscilationMaxRange);
        }

        #region

        private void MeteorologyEventsOnStart(TimeControl.GameDateTime gameDateTime)
        {
            RecalculateMonthSettings(gameDateTime);
            TemperatureRecalculation(gameDateTime);
            CalcNewWeatherForYear(gameDateTime);
            CalculateWeatherForNextMonth(gameDateTime);
        }

        private void MeteorologyEventsEvery10Mins(TimeControl.GameDateTime gameDateTime)
        {
            TemperatureRecalculation(gameDateTime);

            CheckCurrentWeatherDuration(gameDateTime);

            CheckToStartNewWeather(gameDateTime);
        }

        private void MeteorologyEventsEveryHour(TimeControl.GameDateTime gameDateTime)
        {
            
        }
        #endregion

        #region TemperatureCalculation

        private void TemperatureRecalculation (TimeControl.GameDateTime gameDateTime)
        {
            if (gameDateTime.IsNewDayStarted)
            {
                RecalculateMonthSettings(gameDateTime);
            }

            if(gameDateTime.Hours == 0 && gameDateTime.Min == 0)
            {
                _oscillationDayTemperature = UnityEngine.Random.Range(rngOscilationMinRange, rngOscilationMaxRange);
            }

            float grafTemperature = _temperatureGraf[gameDateTime.Hours];
            int nextHour = gameDateTime.Hours + 1;

            if(nextHour > 23)
                nextHour = 0;

            float graphNextTemperature = _temperatureGraf[nextHour];

            float graphTemperature = InterpolatedTemperatureFromGraph(grafTemperature, graphNextTemperature, gameDateTime.Min);
            float avrgTemperature = CalculateAvrgMonthTemperature(gameDateTime.GetDayInMonth, gameDateTime.GetMonth);

            _currentTemperature = graphTemperature + avrgTemperature + _oscillationDayTemperature;

            // apply weather condition
            if (_currentWeather != null && _currentWeather.Weather != null)
            {
                float weatherTemperature = InterpolateWeatherTemperature();
                _currentTemperature = _currentTemperature + weatherTemperature;
            }

            OnTemperatureChanged.Invoke(_currentTemperature);

            // First 30 % and last 30% effect of weather temperature is raising to max or min. 
            float InterpolateWeatherTemperature()
            {
                float interpolatedTemp = 0;
                float first20Percantage = _currentWeather.Weather.GetDurationInHours * 0.3f;
                float first70Percantage = _currentWeather.Weather.GetDurationInHours * 0.7f;

                int passedDays =  gameDateTime.GetDayInMonth - _currentWeather.StartDay;
                int passedHours = 0;
                int passedMins = 0;
                if (passedDays > 0)
                {
                    passedHours = (passedDays * 24) - _currentWeather.StartHour;
                    passedHours += gameDateTime.Hours;
                    passedMins = passedHours * 60;
                    passedMins = passedMins + gameDateTime.Min;
                }
                else
                {
                    passedHours =  gameDateTime.Hours - _currentWeather.StartHour;
                    passedMins = passedHours * 60;
                    passedMins = passedMins + gameDateTime.Min;
                }

                float percantage = 0;
                if (passedMins < first20Percantage * 60)
                {
                    float first20PercantageInMinutes = first20Percantage * 60;
                    percantage = passedMins / first20PercantageInMinutes;
                    interpolatedTemp = Mathf.Lerp(0, _currentWeather.Weather.GetTemperatureChange, percantage);
                }
                else if (passedMins > first70Percantage * 60)
                {
                    float first70PercantageInMinutes = first70Percantage * 60;
                    percantage = passedMins / first70PercantageInMinutes;
                    percantage = percantage - (float)Math.Truncate(percantage);
                    interpolatedTemp = Mathf.Lerp(_currentWeather.Weather.GetTemperatureChange, 0, percantage);
                }
                else
                {
                    percantage = 1;
                    interpolatedTemp = Mathf.Lerp(0, _currentWeather.Weather.GetTemperatureChange, percantage);
                }
    
                return interpolatedTemp;
            }
        }

        private float InterpolatedTemperatureFromGraph(float curentTemp , float nextTemp, int minutes)
        {
            float percentage = minutes / 60;

            float finalTemperature = Mathf.Lerp(curentTemp, nextTemp, percentage);

            return finalTemperature;
        }

        private float CalculateAvrgMonthTemperature(int dayInMonth, TimeControl.GameDateTime.Months currentMonth)
        {
            float temperature = _currentMonthSettings.GetAvarageTemperature;
            var days = TimeControl.GetAmountDaysInMonth(currentMonth);
   
            if(dayInMonth < 6)
            {
                // previous month + current / 2
                temperature = (_previousMonthSettings.GetAvarageTemperature + _currentMonthSettings.GetAvarageTemperature) / 2;
            }

            if(dayInMonth > days - 6)
            {
                // next month + current / 2
                temperature = (_nextMonthSettings.GetAvarageTemperature + _currentMonthSettings.GetAvarageTemperature) / 2;
            }

            return temperature;
        }

        private TemperatureGlobalSettings.TemperatureMonthSettings GetMonthSettings(int month)
        {
            foreach (TemperatureGlobalSettings.TemperatureMonthSettings monthSettings in _temperatureMonthSettings)
            {
                if ((int)monthSettings.GetMonth == month)
                    return monthSettings;
            }

            return null;
        }

        private void RecalculateMonthSettings (TimeControl.GameDateTime gameDateTime)
        {
            var previous = (int)gameDateTime.GetMonth - 1;
            if (previous < 0)
                previous = 12;

            var next = (int)gameDateTime.GetMonth + 1;
            if (next > 12)
                next = 0;

            _nextMonthSettings = GetMonthSettings(next);
            _previousMonthSettings = GetMonthSettings(previous);
            _currentMonthSettings = GetMonthSettings((int)gameDateTime.GetMonth);
        }

        #endregion

        #region WeatherCalculation

        private void CheckToStartNewWeather(TimeControl.GameDateTime gameDateTime)
        {
            if(_currentWeather == null)
            {
                foreach (WeatherMonthSettings.WeatherReadyToStart weatherToStart in _weatherMonthSettings.listWeatherReadyToStart)
                {
                    if (CheckWeatherStartTime(weatherToStart))
                        StartWeatherEffect(weatherToStart);
                }

                if(_currentWeather != null)
                {
                    _weatherMonthSettings.listWeatherReadyToStart.Remove(_currentWeather);
                }
            }

            bool CheckWeatherStartTime(WeatherMonthSettings.WeatherReadyToStart weatherToStart)
            {
                if (gameDateTime.Day == weatherToStart.StartDay && gameDateTime.Hours == weatherToStart.StartHour && gameDateTime.Min == weatherToStart.StartMin)
                    return true;

                return false;
            }
        }

        // TODO visualization not implemented yet
        private void StartWeatherEffect(WeatherMonthSettings.WeatherReadyToStart weather)
        {
            _currentWeather = weather;
            Debug.Log("weather started :   " + _currentWeather.ToString());
        }

        private void StopWeatherEffect()
        {
            Debug.Log("weather ended :   " + _currentWeather.ToString());
            _currentWeather = null;
        }

        private void CheckCurrentWeatherDuration(TimeControl.GameDateTime gameDateTime)
        {
            if(_currentWeather != null)
            {
                int passedDays = _currentWeather.StartDay - gameDateTime.GetDayInMonth;
                int passedHours = 0;
                if (passedDays > 0)
                {
                    passedHours = (passedDays * 24) - _currentWeather.StartHour;
                    passedHours += gameDateTime.Hours;
                }
                else
                {
                    passedHours = _currentWeather.StartHour - gameDateTime.Hours;
                }

                if (_currentWeather.Weather.GetDurationInHours <= passedHours)
                {
                    StopWeatherEffect();
                }
            }
        }

        //TODO Future maybe we can have more weather when game is harder - difficulty is harder.
        //TODO think about evenly insertion of elements into list. By WeatherType  "standar" posApo, Hell
        // Goal is posApo , hell weathers was evenly put into one Year so in one month there is no more than 2 HARD weather to survive.
        private void CalculateWeatherForNextMonth(TimeControl.GameDateTime gameDateTime)
        {
            float weatherPerMonth = weatherSettings.GetNumberWeatherPerMonth;
            double decNumber = weatherPerMonth - Math.Truncate(weatherPerMonth);

            int amountDaysInMonth = TimeControl.GetAmountDaysInMonth(_currentMonthSettings.GetMonth);

            float rng = UnityEngine.Random.Range(0,100);

            if (rng <= decNumber * 100)
                weatherPerMonth = (float)Math.Ceiling(weatherPerMonth);
            else
            {
                weatherPerMonth = (float)Math.Round(weatherPerMonth);
            }

            int weatherEndDay = 0;
            int weatherStartDay = 0;

            _weatherMonthSettings = new WeatherMonthSettings();

            for (int i = 0; i < weatherPerMonth; i++)
            {
                int daysForOneWeather = (int)(amountDaysInMonth / weatherPerMonth);
                //Todo random not implemented yeat.
                Weather weather = TakeRandomWeatherFromWeatherPool();

                weather = weatherPool[0];

                int weatherDurationInDays = (int)weather.GetDurationInHours / 24 + 1;

                daysForOneWeather -= weatherDurationInDays;

                int weatherStartMin = UnityEngine.Random.Range(0, 6);
                int weatherStartHour = UnityEngine.Random.Range(0, 24);
                weatherStartDay = (int)UnityEngine.Random.Range( 2 + weatherEndDay, weatherStartDay + daysForOneWeather);

                weatherEndDay = weatherStartDay;

                WeatherMonthSettings.WeatherReadyToStart ws = new WeatherMonthSettings.WeatherReadyToStart(weather, weatherStartDay, weatherStartHour, weatherStartMin * 10);
                _weatherMonthSettings.listWeatherReadyToStart.Add(ws);
            }
        }

        private Weather TakeRandomWeatherFromWeatherPool()
        {

            return null;
        }

        // TODO lets think about future.. We should take weather randomly but according difficulty of the game..
        // lets say Easy:  80% standart weather 20% posApo
        // lets say Normal:  60% standart weather 30% posApo , hell 10%
        // lets say Apocalyptic:  40% standart weather 35% posApo , hell 25%
        private void CalcNewWeatherForYear(TimeControl.GameDateTime gameDateTime)
        {
            List<Weather> listStandarWeather = weatherSettings.GetAvailableWeather.Where(w => w.GetWeatherType == WeatherType.standart).ToList();
            List<Weather> listPosApoWeather = weatherSettings.GetAvailableWeather.Where(w => w.GetWeatherType == WeatherType.posApo).ToList();
            List<Weather> listHellWeather = weatherSettings.GetAvailableWeather.Where(w => w.GetWeatherType == WeatherType.hell).ToList();

            int count = listStandarWeather.Count;

            for (int i = 0; i < weatherSettings.GetNumberOfStandartWeather; i++)
            {
                int rng = UnityEngine.Random.Range(0,count);
                weatherPool.Add(listStandarWeather[rng]);
            }
        }

        #endregion

        private void OnDisable()
        {
            TimeControl.OnGameStart -= MeteorologyEventsOnStart;
            TimeControl.OnTimeChangedEvery10Mins -= MeteorologyEventsEvery10Mins;
            TimeControl.OnTimeChangedNewMonth -= CalculateWeatherForNextMonth;
            TimeControl.OnTimeChangedEveryHour -= MeteorologyEventsEveryHour;
        }
    }

    [Serializable]
    public class WeatherSettings
    {

        [SerializeField] [Range(12, 45)] int numberOfWeatherEventsInYear = 16;

        [Space]
        [SerializeField] [Range(0, 12)] float numbeWeatherPerMonth = 2.5f;
        [SerializeField] private List<Weather> availableWeather = new List<Weather>();

        public List<Weather> GetAvailableWeather { get { return availableWeather; } }
        public int GetNumberOfStandartWeather { get { return numberOfWeatherEventsInYear; } }
        public float GetNumberWeatherPerMonth { get { return numbeWeatherPerMonth; } }

    }

    public class WeatherMonthSettings
    {
        public List<WeatherReadyToStart> listWeatherReadyToStart = new List<WeatherReadyToStart>();

        public class WeatherReadyToStart
        {
            private Weather weather;
            private int startDay;
            private int startHour;
            private int startMin;

            public WeatherReadyToStart(Weather _weather, int _dayInMonth, int _hourInDay, int _min)
            {
                weather = _weather;
                startDay = _dayInMonth;
                startHour = _hourInDay;
                startMin = _min;
            }

            public Weather Weather { get { return weather; } }
            public int StartDay { get { return startDay; } }
            public int StartHour { get { return startHour; } }
            public int StartMin { get { return startMin; } }
        }
    }
}