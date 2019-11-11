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

    private List<GameObject> potravinyBoxes;
    private List<GameObject> vojenskyMaterialBoxes;
    private List<GameObject> technickyMaterialBoxes;
    private List<GameObject> civilniMaterialBoxes;
    private List<GameObject> pohonneHmotyBoxes;
    private TileFactory tileFactory;
    public GameObject PotravinyBigBox;
    public GameObject PotravinySmallBox;
    public GameObject VojenskyMaterialBigBox;
    public GameObject VojenskyMaterialSmallBox;
    public GameObject TechnickyMaterialBigBox;
    public GameObject TechnickyMaterialSmallBox;
    public GameObject CivilniMaterialBigBox;
    public GameObject CivilniMaterialSmallBox;
    public GameObject PohonneHmotyBigBox;
    public GameObject PohonneHmotySmallBox;



    [Serializable]
    public enum Material
    {
        Potraviny,
        Vojensky,
        Technicky,
        Civilni,
        Pohonne
    }
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
        if (value > 0)
        {
            SpawnMaterial(Material.Potraviny, value);
        }
    }

    public void IncVojenskyMaterialy(int value)
    {
        vojenskyMaterial += value;
        if (value > 0)
        {
            SpawnMaterial(Material.Vojensky, value);
        }
    }

    public void IncTechnickyMaterial(int value)
    {
        technickyMaterial += value;
        if (value > 0)
        {
            SpawnMaterial(Material.Technicky, value);
        }
    }

    public void IncPohonneHmoty(int value)
    {
        pohonneHmoty += value;
        if (value > 0)
        {
            SpawnMaterial(Material.Pohonne, value);
        }
    }

    public void IncCivilniMaterial(int value)
    {
        civilniMaterial += value;
        if (value > 0)
        {
            SpawnMaterial(Material.Civilni, value);
        }
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
        technickyMaterialBoxes = new List<GameObject>();
        potravinyBoxes = new List<GameObject>();
        vojenskyMaterialBoxes = new List<GameObject>();
        civilniMaterialBoxes = new List<GameObject>();
        pohonneHmotyBoxes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

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
     //   pohonneHmoty += value;
        civilniMaterial += value;
        //   energie += value;
        //   deti += value;
        //   karma += value;
        SpawnMaterial(Material.Potraviny, value);
        SpawnMaterial(Material.Vojensky, value);
        SpawnMaterial(Material.Technicky, value);
     //   SpawnMaterial(Material.Pohonne, value);
        SpawnMaterial(Material.Civilni, value);
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

    
    //Unity editor neumi udel dropmenu s enumama, takze obezlicka pres int, bohuzel
    public void SpawnMaterial(int typ, int amount)
    {
        SpawnMaterial((Material)typ, amount);
    }
    public void SpawnMaterial(Material typ, int amount)
    {
        GameObject box;
        List<GameObject> pointer;
        switch (typ)
        {
            case Material.Potraviny:
                box = amount == 10 ? PotravinyBigBox : PotravinySmallBox;
                pointer = potravinyBoxes;
                break;
            case Material.Civilni:
                box = amount == 10 ? CivilniMaterialBigBox : CivilniMaterialSmallBox;
                pointer = civilniMaterialBoxes;
                break;
            case Material.Vojensky:
                box = amount == 10 ? VojenskyMaterialBigBox : VojenskyMaterialSmallBox;
                pointer = vojenskyMaterialBoxes;
                break;
            case Material.Technicky: 
                box = amount == 10 ? TechnickyMaterialBigBox : TechnickyMaterialSmallBox;
                pointer = technickyMaterialBoxes;
                break;
            default:
                box = amount == 10 ? PohonneHmotyBigBox : PohonneHmotySmallBox;
                pointer = pohonneHmotyBoxes;
                break;
        }

        if (amount > 10)
        {
            SpawnMaterial(typ, amount - 10);
        }
        var coord = tileFactory.FindFreeTile();
        var newBox = Instantiate(box, Geometry.PointFromGrid(coord), Quaternion.identity);
        pointer.Add(newBox);
        tileFactory.AddBox(coord, newBox);
    }

    //zatim jen proof of work, jen pro technicky material
    //najdi nejblizsi bednu od pole 'from'
    public GameObject Nearest(Vector2Int from)
    {
        var cheapest = technickyMaterialBoxes.First();
        int smallestSteps = int.MaxValue;
        foreach (var box in technickyMaterialBoxes)
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
