using UnityEngine;

namespace Enviroment
{
    [CreateAssetMenu(menuName = "ScriptableObject/Enviroment/SunCycleSettings", fileName = "_NewSunCycleSettings")]
    public class SunCycleSettings : ScriptableObject
    {
        public float sunriseEdgeTime = 8;
        public float sunsetEdgeTime = 20;

        public float moonriseEdgeTime = 22;
        public float moonEndEdgeTime = 4;

    }
}
