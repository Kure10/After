using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventBlueprint
{
    public int difficulty;

    public string eventTerrain;

    public string name;

    public long Id;
    // pro missi  OLD

    public bool isEventFinished = false;

    public int evocationTime;

    public int numberOfOptions;

    public Sprite sprite;

    public string description;

    public string[] answerTextField = new string[3];

    public bool wasTriggered;

    public bool hasAvoidButton; // jestli ma avoid tlacitko..

    // dwada

    public List<string> reaction = new List<string>(); // tohle by mohlo byt pro akce.. nejdriv string a pak podle toho udelam akci na button.

    // mnohem vic to tu bude... musim si promyslet jakou strukturu budou mit entity...
    //  (počet zombiku ,  stamina , info pro hrace.. , stamina cost. ..  Nevim ??)
    // možny reward za event .. a tak dale..
    

    /*
     pak tu musí byt uloženy jeste EVent neboli akce.. co se stane když se klikne určita odpoved.. Třeba kliknu utect nebo bojuj..
     */

    public EventBlueprint()
    {

    }     


}
