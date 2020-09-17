using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEventNotification : MonoBehaviour
{

    [SerializeField] Text messageText;

    private RegionOperator regOperator;

    public RegionOperator RegOperator { get { return this.regOperator; } set { this.regOperator = value; } }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }

    // Animation
    public void DestroyOnTime()
    {
        Destroy(this.gameObject);
    }
}
