using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceControler : MonoBehaviour
{

    private int potraviny;
    private int vojenskyMaterial;
    private int technickyMaterial;
    private int civilniMaterial;
    private int pohonneHmoty;
    private int energie;
    private int deti;
    private int karma;

    /*
    private int specialniMaterial;
    private int specialiste;
    */

    public Text[] text;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    public void SetValueOf(int value, int resourse)
    {
        resourse += value;
    }

    private void UpdateText ()
    {
        text[0].text = potraviny.ToString();
        text[1].text = vojenskyMaterial.ToString();
        text[2].text = technickyMaterial.ToString();
        text[3].text = civilniMaterial.ToString();
        text[4].text = pohonneHmoty.ToString();
        text[5].text = energie.ToString();
        text[6].text = deti.ToString();
        text[7].text = karma.ToString();
       // text[8].text = karma.ToString();
    }

}
