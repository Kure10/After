using UnityEngine;
using System;

namespace Enviroment
{
    public class DayCycleController : MonoBehaviour
    {
        [SerializeField] GameObject sunLight;
        [SerializeField] GameObject moonlight;
        [SerializeField] SunCycleSettings sunCycleSettings;

        private void OnEnable()
        {
            TimeControl.OnDateChanged += EnviromentChange;
        }

        public void EnviromentChange(TimeControl.GameDateTime _gameTime)
        {
            RotateSun(_gameTime.Min, _gameTime.Hours);
        }

        private void OnDisable()
        {
            TimeControl.OnDateChanged += EnviromentChange;
        }

        private void RotateSun(int minutes, int hours)
        {
            Quaternion quaterResult = new Quaternion(0, 0, 0, 0);
            Quaternion quaterX = new Quaternion(0, 0, 0, 0);
            Quaternion quaterY = new Quaternion(0, 0.7f, 0, -0.7f); // 90 stupnu
            float sunLightRotationX = 0;
            float sunLightRotationY = 0;
            float moonLightRotation = 0;
            float finalTime = (float)((hours) + (float)(minutes / 60f));
            float percentage = (float)(finalTime / 24f);

            sunLightRotationY = Mathf.Lerp(160, 320, percentage);
            quaterY = Quaternion.AngleAxis(sunLightRotationY, Vector3.up);

            if (finalTime > sunCycleSettings.sunriseEdgeTime && finalTime < sunCycleSettings.sunsetEdgeTime)
            {
                // Day
                float actualyDayLenght = Math.Abs(sunCycleSettings.sunriseEdgeTime - sunCycleSettings.sunsetEdgeTime);
                float onePercetage = 100 / actualyDayLenght;
                percentage = ((finalTime - sunCycleSettings.sunriseEdgeTime) * onePercetage) / 100;
                sunLightRotationX = Mathf.Lerp(-30, 190, percentage);

                quaterX = Quaternion.AngleAxis(sunLightRotationX, Vector3.right);
                quaterResult = quaterY * quaterX;
                sunLight.transform.rotation = quaterResult;
            }
            else if (finalTime > sunCycleSettings.moonriseEdgeTime || finalTime < sunCycleSettings.moonEndEdgeTime)
            {
                // Night
                float actualMoonNightTime = Math.Abs(24 - sunCycleSettings.moonriseEdgeTime + sunCycleSettings.moonEndEdgeTime);
                float onePercetage = 100 / actualMoonNightTime;
                float moonLightRotationY = 0;
                moonLightRotationY = Mathf.Lerp(200, 280, onePercetage);
                quaterY = Quaternion.AngleAxis(moonLightRotationY, Vector3.up);

                if (finalTime > sunCycleSettings.moonriseEdgeTime && finalTime > sunCycleSettings.moonEndEdgeTime)
                {
                    float timePoint = finalTime - sunCycleSettings.moonriseEdgeTime;
                    percentage = onePercetage * timePoint / 100;
                }
                else
                {
                    float a = 24 - sunCycleSettings.moonriseEdgeTime;
                    float timePoint = finalTime + a;
                    percentage = onePercetage * timePoint / 100;

                }

                moonLightRotation = Mathf.Lerp(-30, 190, percentage);
                moonlight.transform.rotation = quaterY * Quaternion.AngleAxis(moonLightRotation, Vector3.right);
            }
        }
    }
}

