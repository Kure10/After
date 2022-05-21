using UnityEngine;
using System;

namespace Enviroment
{
    [CreateAssetMenu(menuName = "ScriptableObject/Enviroment/TemperatureSettings", fileName = "_NewTemperatureSettings")]
    public class TemperatureGlobalSettings : ScriptableObject
    {
        [Header("Winter")]
        public TemperatureMonthSettings December;
        public TemperatureMonthSettings January;
        public TemperatureMonthSettings February;
        [Space]
        [Header("Spring")]
        public TemperatureMonthSettings March;
        public TemperatureMonthSettings April;
        public TemperatureMonthSettings May;
        [Space]
        [Header("Summer")]
        public TemperatureMonthSettings June;
        public TemperatureMonthSettings July;
        public TemperatureMonthSettings August;
        [Space]
        [Header("Autumn")]
        public TemperatureMonthSettings September;
        public TemperatureMonthSettings October;
        public TemperatureMonthSettings November;

        [Serializable]
        public class TemperatureMonthSettings
        {
            [SerializeField] TimeControl.GameDateTime.Months month = TimeControl.GameDateTime.Months.April;
            [SerializeField] [Range(-10, 20)] float avarageTemperature;

            [Space]
            [Header("Weather Events")]
            [SerializeField] [Range(0, 5)] int amountShower;
            [Space]
            [SerializeField] [Range(0, 3)] int amountContinuousRain;
            [SerializeField] [Range(35, 80)] int minRainyTime;
            [SerializeField] [Range(5, 20)] int maxRainyTime;

            public TimeControl.GameDateTime.Months GetMonth { get { return month; } }

            public float GetAvarageTemperature { get { return avarageTemperature; } }
            public int GetAmountShower { get { return amountShower; } }
            public int GetAmountContinuousRain { get { return amountContinuousRain; } }
            public int GetMinRainyTime { get { return minRainyTime; } }
            public int GetMaxRainyTime { get { return maxRainyTime; } }
        }
    }
}
