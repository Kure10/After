using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Enviroment
{
    public class TemperatureController : MonoBehaviour
    {
        [SerializeField] TemperatureGlobalSettings tempGlobalSettings;

        private List<TemperatureGlobalSettings.TemperatureSeasonSettings> listTempGlobalSettings = new List<TemperatureGlobalSettings.TemperatureSeasonSettings>();

        private static float _globalTemperatureInCelsia = 0;
        public static float GetTemperatureInCelsia { get { return _globalTemperatureInCelsia; } }

        private float _maxDayTemperature = 0;
        private float _minDayTemperature = 0;

        private float _rngFactor = 3;
        private TemperatureGlobalSettings.TemperatureSeasonSettings.TemperatureMonthSettings _monthSettings = null;

        private void Awake()
        {
            listTempGlobalSettings.Add(tempGlobalSettings.Spring);
            listTempGlobalSettings.Add(tempGlobalSettings.Summer);
            listTempGlobalSettings.Add(tempGlobalSettings.Autumn);
            listTempGlobalSettings.Add(tempGlobalSettings.Winter);
        }

        private void OnEnable()
        {
            TimeControl.OnGameStart += SetupNewMonth;
            TimeControl.OnTimeChangedEvery10Mins += TemperatureChange;
        }

        private void TemperatureChange(TimeControl.GameDateTime gameDateTime)
        {
            float temperatureInCelsia = -100;

            if(gameDateTime.IsFirstDayInNewMonth)
            {
                SetupNewMonth(gameDateTime);
            }

            if (gameDateTime.Hours == 0 && gameDateTime.Min == 0)
                RecalculateMaxTemp();

            if (gameDateTime.Hours == 12 && gameDateTime.Min == 0)
                RecalculateMinTemp();

            if (_monthSettings != null)
            {
                temperatureInCelsia = GetTemerature(_maxDayTemperature, _minDayTemperature, gameDateTime.Hours);

                temperatureInCelsia = (float)Math.Round(temperatureInCelsia, 1);
            }

            if(temperatureInCelsia != -100)
            {
                _globalTemperatureInCelsia = temperatureInCelsia;
            }
        }

        private TemperatureGlobalSettings.TemperatureSeasonSettings.TemperatureMonthSettings GetRelevantTemperatureGlobalSettings(TimeControl.GameDateTime gameDateTime, TemperatureGlobalSettings.TemperatureSeasonSettings TemperatureSetting)
        {
            switch ((int)gameDateTime.GetMonth % 3)
            {
                case 0:
                    return TemperatureSetting.FirstMonth;
                case 1:
                    return TemperatureSetting.SecondMonth;
                case 2:
                    return TemperatureSetting.ThirdMonth;
                default:
                    break;
            }

            Debug.LogError("Error In TemperatureController -> Probably something is null");
            return null;
        }

        private float GetTemerature(float maxTmp, float minTmp, float time)
        {
            float prumern = (maxTmp + minTmp) / 2;
            float roztyl = maxTmp - Math.Abs(minTmp);
            float k = roztyl / 2;
            float X = 2 * (float)Math.PI * time / 24;
            float Cos = (float)(-Math.Cos(X));
            float temperature = (float)(Cos * k) + prumern;

            return temperature;
        }

        private void RecalculateMaxTemp()
        {
            float tmp = UnityEngine.Random.Range(-_rngFactor, _rngFactor) + _monthSettings.GetNormalMaxDayTemp;
            _maxDayTemperature = tmp;

            Debug.Log($"{_maxDayTemperature}  / {_minDayTemperature}   : Max Rec ");
        }

        private void RecalculateMinTemp()
        {
            float tmp = UnityEngine.Random.Range(-_rngFactor, _rngFactor) + _monthSettings.GetNormalMinDayTemp;
            _minDayTemperature = tmp;

            Debug.Log($"{_maxDayTemperature}  / {_minDayTemperature}   : Min Rec ");
        }


        private void SetupNewMonth(TimeControl.GameDateTime gameDateTime)
        {
            foreach (TemperatureGlobalSettings.TemperatureSeasonSettings TemperatureSetting in listTempGlobalSettings)
            {
                if (TemperatureSetting.GetSeason == gameDateTime.GetSeason)
                {
                    _monthSettings = GetRelevantTemperatureGlobalSettings(gameDateTime, TemperatureSetting);
                    _maxDayTemperature = _monthSettings.GetNormalMaxDayTemp;
                    _minDayTemperature = _monthSettings.GetNormalMinDayTemp;
                }
            }
        }


        private void OnDisable()
        {
            TimeControl.OnGameStart -= SetupNewMonth;
            TimeControl.OnTimeChangedEvery10Mins -= TemperatureChange;
        }
    }
}
