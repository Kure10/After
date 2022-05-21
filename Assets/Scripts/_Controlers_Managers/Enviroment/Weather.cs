using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enviroment
{
    public class Weather : ScriptableObject
    {
        [Header("General Weather Settings")]
        [SerializeField] float temperatureChange = 3f;
        [SerializeField] float durationInHours = 7.8f;
        [SerializeField] WeatherType type = WeatherType.standart;


        public float GetTemperatureChange { get { return temperatureChange; } }
        public float GetDurationInHours { get { return durationInHours; } }
        public WeatherType GetWeatherType { get { return type; } }
    }


    [CreateAssetMenu(menuName = "ScriptableObject/Enviroment/Weather/Rain", fileName = "NewWeather")]
    public class Rain : Weather
    {
        [Header("Rain Settings")]
        [SerializeField] [Range(0, 10)] int rainPower = 0;

    }

    [CreateAssetMenu(menuName = "ScriptableObject/Enviroment/Weather/Wind", fileName = "NewWeather")]
    public class Wind : Weather
    {
        [Header("Wind Settings")]
        [SerializeField] bool isStrongWind = true;

    }


    public enum WeatherType
    {
        standart,
        posApo,
        hell
    }
}


