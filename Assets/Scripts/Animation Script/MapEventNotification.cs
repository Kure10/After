using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventNotification : MonoBehaviour
{
    // Animation
    public void DestroyOnTime()
    {
        Destroy(this.gameObject);
    }
}
