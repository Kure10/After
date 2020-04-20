using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventBlueprint
{

    public int evocationTime;

    public int numberOfOptions;

    public string name;

    public Sprite sprite;

    public string description;

    public string[] answerTextField = new string[3];

    public bool wasTriggered;

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
