using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventButton : MonoBehaviour
{
    [SerializeField] Text buttonText;

    [SerializeField] Text buttonDescription;

    [SerializeField] Button buttonControler;

    

    public Text ButtonDescription { get { return this.buttonDescription; } }

    public Text Text { get { return this.buttonText; }}

    public Button ButtonControler { get { return this.buttonControler; } }

}
