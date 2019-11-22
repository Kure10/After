using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
   
    private int pohonneHmoty;
    private int energie;
    private int deti;
    private int karma;

    private List<GameObject> pohonneHmotyBoxes;
    private TileFactory tileFactory;

    private List<Resource> resources;
    
    public  GameObject PotravinyBigBox;
    public  GameObject PotravinySmallBox;
    public  GameObject VojenskyMaterialBigBox;
    public  GameObject VojenskyMaterialSmallBox;
    public  GameObject TechnickyMaterialBigBox;
    public  GameObject TechnickyMaterialSmallBox;
    public  GameObject CivilniMaterialBigBox;
    public  GameObject CivilniMaterialSmallBox;
    public  GameObject PohonneHmotyBigBox;
    public  GameObject PohonneHmotySmallBox;

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

    public int PohonneHmoty { get { return pohonneHmoty; } set { pohonneHmoty = value; } }

    public int Energie { get { return energie; } set { energie = value; } }

    public int Deti { get { return deti; } set { deti = value; } }

    public int Karma { get { return karma; } set { karma = value; } }

    #endregion

    /*   Metody na nastaveni kazde surky zvlast */

    public void IncPohonneHmoty(int value)
    {
        pohonneHmoty += value;
     /*   if (value > 0)
        {
            SpawnMaterial(Material.Pohonne, value);
        }*/
    }

    public void IncPotraviny(int value)
    {
        SpawnMaterial(Material.Potraviny, value);
    }
    public void IncVojenskyMaterialy(int value)
    {
        SpawnMaterial(Material.Vojensky, value);

    }
    public void IncTechnickyMaterial(int value)
    {
        SpawnMaterial(Material.Technicky, value);

    }
    public void IncCivilniMaterial(int value)
    {
        SpawnMaterial(Material.Civilni, value);
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

    public void SetToZero()
    {
        foreach (var res in resources)
        {
            res.Amount = 0;
        }
        resources.Clear();
        ResourceAmountChanged();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();
        resources = new List<Resource>();
        pohonneHmotyBoxes = new List<GameObject>();
        ResourceAmountChanged();
    }

    // Update is called once per frame

    void ResourceAmountChanged()
    {
        /*  Updatuje nase suroviny v gui */
        
             text[0].text = GetResourceCount(Material.Potraviny).ToString();
             text[1].text = GetResourceCount(Material.Vojensky).ToString();
             text[2].text = GetResourceCount(Material.Technicky).ToString();
             text[3].text = GetResourceCount(Material.Civilni).ToString();
             text[4].text = pohonneHmoty.ToString();
             text[5].text = energie.ToString();
             text[6].text = deti.ToString();
             text[7].text = karma.ToString();
         
    }

    public void AddAll(int value)
    {
        foreach (var material in Enum.GetValues(typeof(Material)))
        {
            SpawnMaterial(Convert.ToInt32(material), value);
        }
    }

    
    //Unity editor neumi udel dropmenu s enumama, takze obezlicka pres int, bohuzel
    public void SpawnMaterial(int typ, int amount)
    {
        SpawnMaterial((Material)typ, amount);
    }
    
    public void SpawnMaterial(Material typ, int amount)
    {

        if (amount > 0)
        {
            GameObject sBox, bBox;
            switch (typ)
            {
                case Material.Potraviny:
                    bBox = PotravinyBigBox;
                    sBox = PotravinySmallBox;
                    break;
                case Material.Civilni:
                    bBox = CivilniMaterialBigBox;
                    sBox = CivilniMaterialSmallBox;
                    break;
                case Material.Vojensky:
                    bBox = VojenskyMaterialBigBox;
                    sBox = VojenskyMaterialSmallBox;
                    break;
                case Material.Technicky:
                    bBox = TechnickyMaterialBigBox;
                    sBox = TechnickyMaterialSmallBox;
                    break;
                default:
                    bBox = PohonneHmotyBigBox;
                    sBox = PohonneHmotySmallBox;
                    break;
            }

            if (amount > 10)
            {
                SpawnMaterial(typ, amount - 10);
            }

            var coord = tileFactory.FindFreeTile();
            var newResourse = new Resource(amount, typ, sBox, bBox, coord);
            resources.Add(newResourse);
            tileFactory.AddBox(coord, newResourse);
        }
        else
        {
            if (!resources.Any()) 
                return;

            var firstBox = resources.First(res => res.material == typ);

            if (firstBox.Amount + amount <= 0)
            {
                amount += firstBox.Amount;
                firstBox.Amount = 0;
                resources.Remove(firstBox);
            }
            else
            {
                firstBox.Amount += amount;
                amount = 0;
            }

            if (amount != 0)
            {
                SpawnMaterial(typ, amount);
            }

        }

        ResourceAmountChanged();
    }

    //zatim jen proof of work, jen pro technicky material
    //najdi nejblizsi bednu od pole 'from'
    public Resource Nearest(Vector2Int from, Material type)
    {
        var res = GetAllBoxesOfType(type);
        var cheapest = res.First();
        int smallestSteps = int.MaxValue;
        foreach (var box in res)
        {
            var steps = tileFactory.FindPath(from, box.position).Count;
            if (steps < smallestSteps)
            {
                smallestSteps = steps;
                cheapest = box;
            }
        }
        return cheapest;
    }

    private List<Resource> GetAllBoxesOfType(Material type)
    {
        return resources.Where(r => r.material == type).ToList();
    }

    public int GetResourceCount(Material type)
    {
        return GetAllBoxesOfType(type).Sum(box => box.Amount);
    }
}
