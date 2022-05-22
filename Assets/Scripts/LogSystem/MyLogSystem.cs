using UnityEngine;

namespace LogSystem
{
    // Todo i can log messages into list
    // Time where messages was log
    // and check if some are redundant or repetetive.
    // what ever
    public static class MyLogSystem
    {
        public static void Log(string message, Object obj = null)
        {
            if(obj)
            {
                Debug.Log($"This Object: {obj.name} {message}");
            }
            else
            {
                Debug.Log($"{message}");
            }
        }

        public static void LogWarning(string message, Object obj = null)
        {
            if (obj)
            {
                Debug.LogWarning($"This Object: {obj.name} {message}");
            }
            else
            {
                Debug.LogWarning($"{message}");
            }
        }

        public static void LogError(string message, Object obj = null)
        {
            if (obj)
            {
                Debug.LogError($"This Object: {obj.name} {message}");
            }
            else
            {
                Debug.LogError($"{message}");
            }
        }
    }
}

