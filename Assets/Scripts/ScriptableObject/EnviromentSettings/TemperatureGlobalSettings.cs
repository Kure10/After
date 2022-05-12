using UnityEngine;
using System;

namespace Enviroment
{
    [CreateAssetMenu(menuName = "ScriptableObject/TemperatureSettings", fileName = "_NewTemperatureSettings")]
    public class TemperatureGlobalSettings : ScriptableObject
    {
        [Header("Seasons")]
        public TemperatureSeasonSettings Spring;

        public TemperatureSeasonSettings Summer;

        public TemperatureSeasonSettings Autumn;

        public TemperatureSeasonSettings Winter;

        [Serializable]
        public class TemperatureSeasonSettings
        {
            [SerializeField] TimeControl.GameDateTime.Season season = TimeControl.GameDateTime.Season.Autumn;
            public TemperatureMonthSettings FirstMonth;
            public TemperatureMonthSettings SecondMonth;
            public TemperatureMonthSettings ThirdMonth;

            public TimeControl.GameDateTime.Season GetSeason { get { return season; } }

            [Serializable]
            public class TemperatureMonthSettings
            {
                [SerializeField] [Range(-20, 20)] int normalMinDayTemp;
                [SerializeField] [Range(0, 45)] int normalMaxDayTemp;
                [Space]
                [SerializeField] [Range(0, 5)] int amountAnomalyColdDays;
                [SerializeField] [Range(0, 45)] int anomalyColdDayTemperature;
                [Space]
                [SerializeField] [Range(0, 5)] int amountAnomalyHotDays;
                [SerializeField] [Range(-20, 45)] int anomalyHotDayTemperature;

                public int GetNormalMinDayTemp { get { return normalMinDayTemp; } }
                public int GetNormalMaxDayTemp { get { return normalMaxDayTemp; } }
                public int GetAmountAnomalyColdDays { get { return amountAnomalyColdDays; } }
                public int GetAnomalyColdDayTemperature { get { return anomalyColdDayTemperature; } }
                public int GetAmountAnomalyHotDays { get { return amountAnomalyHotDays; } }
                public int GetAnomalyHotDayTemperature { get { return anomalyHotDayTemperature; } }
            }
        }
    }
}
