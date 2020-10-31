using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventButton : MonoBehaviour
{
    [SerializeField] Text text;

    [SerializeField] Button buttonControler;

    public Text Text { get { return this.text; }}

    public Button ButtonControler { get { return this.buttonControler; } }

}
