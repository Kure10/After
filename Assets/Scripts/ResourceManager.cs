using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    
    private int potraviny;
    private int vojenskyMaterial;
    private int technickyMaterial;
    private int civilniMaterial;
    private int pohonneHmoty;
    private int energie;
    private int deti;
    private int karma;
    private List<GameObject> TechnickyMaterialBoxes;
    private TileFactory tileFactory;
    public GameObject TechnickyMaterialBox;

    /*
    private int specialniMaterial;
    private int specialiste;
    */

    public Text[] text;

    #region Properity

     public int Potraviny { get { return potraviny; } set{ potraviny = value;} }

    public int VojenskyMaterial { get { return vojenskyMaterial; } set { vojenskyMaterial = value; } }

    public int TechnickyMaterial { get { return technickyMaterial; } set { technickyMaterial = value; } }

    public int CivilniMaterial { get { return civilniMaterial; } set { civilniMaterial = value; } }

    public int PohonneHmoty { get { return pohonneHmoty; } set { pohonneHmoty = value; } }

    public int Energie { get { return energie; } set { energie = value; } }

    public int Deti { get { return deti; } set { deti = value; } }

    public int Karma { get { return karma; } set { karma = value; } }

    #endregion




    /*   Metody na nastaveni kazde surky zvlast */
    public void IncPotraviny (int value)
    {
        potraviny += value;
    }

    public void IncVojenskyMaterialy(int value)
    {
        vojenskyMaterial += value;
    }

    public void IncTechnickyMaterial(int value)
    {
        technickyMaterial += value;
        SpawnTechnickyMaterial(value);
    }

    public void IncPohonneHmoty(int value)
    {
        pohonneHmoty += value;
    }

    public void IncCivilniMaterial(int value)
    {
        civilniMaterial += value;
    }

    public void IncEnergie(int value)
    {
        energie += value;
    }

    public void IncDeti(int value)
    {
        deti += value;
    }

    public void IncKarma(int value)
    {
        karma += value;
    }


    // Start is called before the first frame update
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();
        TechnickyMaterialBoxes = new List<GameObject>();
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






    //jakmile ziskas novou surku, mela by se objevit i na mape ve forme bedny
    //aby se nerozhodil pocet surek a pocet beden, melo by se spawnovat jen z jednoho misto, ktere to bude hlidat
    //tady test, zatim jen pro TechnickyMaterial, casem nejak univerzalneji
    public void SpawnTechnickyMaterial(int amount, Vector2Int coord)
    {
        var box = Instantiate(TechnickyMaterialBox, Geometry.PointFromGrid(coord), Quaternion.identity);
        TechnickyMaterialBoxes.Add(box);
        tileFactory.AddBox(coord, box);
    }

    //nemame-li konkretni koordinaty, najdi nahodne volne misto a spawni to tam
    public void SpawnTechnickyMaterial(int amount)
    {
        //Finds first emptyTile with no building and no resource box
        var coord = tileFactory.FindFreeTile();
        SpawnTechnickyMaterial(amount, coord);
    }

    //zatim jen proof of work, jen pro technicky material
    //najdi nejblizsi bednu od pole 'from'
    public GameObject Nearest(Vector2Int from)
    {
        var cheapest = TechnickyMaterialBoxes.First();
        int smallestSteps = int.MaxValue;
        foreach (var box in TechnickyMaterialBoxes)
        {
            var steps = tileFactory.FindPath(from, Geometry.GridFromPoint(box.transform.position)).Count;
            if (steps < smallestSteps)
            {
                smallestSteps = steps;
                cheapest = box;
            }
        }
        return cheapest;
    }
}
