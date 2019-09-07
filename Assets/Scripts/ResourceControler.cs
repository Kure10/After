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


   /*   Metody na nastaveni kazde surky zvlast */

    public void SetPotraviny (int value)
    {
        potraviny += value;
    }

    public void SetVojenskyMaterialy(int value)
    {
        vojenskyMaterial += value;
    }

    public void SetTechnickyMaterial(int value)
    {
        technickyMaterial += value;
    }

    public void SetPohonneHmoty(int value)
    {
        pohonneHmoty += value;
    }

    public void SetCivilniMaterial(int value)
    {
        civilniMaterial += value;
    }

    public void SetEnergie(int value)
    {
        energie += value;
    }

    public void SetDeti(int value)
    {
        deti += value;
    }

    public void SetKarma(int value)
    {
        karma += value;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    /*
    public void SetValueOf(int value, int resourse)
    {
        resourse += value;
    }*/

        /*  Updatuje nase suroviny v gui */
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
    }

    public void AddAll(int value)
    {
        potraviny += value;
        vojenskyMaterial += value;
        technickyMaterial += value;
    //    pohonneHmoty += value;
        civilniMaterial += value;
     //   energie += value;
     //   deti += value;
     //   karma += value;
    }

    public void SetToZero()
    {
        potraviny = 0;
        vojenskyMaterial = 0;
        technickyMaterial = 0;
     //   pohonneHmoty = 0;
        civilniMaterial = 0;
     //   energie = 0;
     //   deti = 0;
      //  karma = 0;
    }

}
