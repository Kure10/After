using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class ResourceManager : MonoBehaviour
{
   
    private int pohonneHmoty;
    private int energie;
    private int deti;
    private int karma;

    private TileFactory tileFactory;

    private List<Resource> resources;
    public GameObject[] Prefabs;
    
    
    public  static GameObject PotravinyBigBox;
    public  static GameObject PotravinySmallBox;
    public  static GameObject VojenskyMaterialBigBox;
    public  static GameObject VojenskyMaterialSmallBox;
    public  static GameObject TechnickyMaterialBigBox;
    public  static GameObject TechnickyMaterialSmallBox;
    public  static GameObject CivilniMaterialBigBox;
    public  static GameObject CivilniMaterialSmallBox;
    public  static GameObject PohonneHmotyBigBox;
    public  static GameObject PohonneHmotySmallBox;

    public void Awake()
    {
        //ugly hack just to be able to set it from unity interface
        PotravinyBigBox = Prefabs[0];
        PotravinySmallBox = Prefabs[1];
        VojenskyMaterialBigBox = Prefabs[2];
        VojenskyMaterialSmallBox = Prefabs[3];
        TechnickyMaterialBigBox = Prefabs[4];
        TechnickyMaterialSmallBox = Prefabs[5];
        CivilniMaterialBigBox = Prefabs[6];
        CivilniMaterialSmallBox = Prefabs[7];
        PohonneHmotyBigBox = Prefabs[8];
        PohonneHmotySmallBox = Prefabs[9];
    }

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

    public static GameObject GetPrefab(int amount, Material type)
    {
        switch (type)
        {
            case Material.Potraviny:
                return amount == 10 ? PotravinyBigBox : PotravinySmallBox;
            case Material.Civilni:
                return amount == 10 ? CivilniMaterialBigBox : CivilniMaterialSmallBox;
            case Material.Vojensky:
                return amount == 10 ? VojenskyMaterialBigBox : VojenskyMaterialSmallBox;
            case Material.Technicky:
                return amount == 10 ? TechnickyMaterialBigBox : TechnickyMaterialSmallBox;
            case Material.Pohonne:
                return amount == 10 ? PohonneHmotyBigBox : PohonneHmotySmallBox;
            default:
                Debug.Log("Snazime se pridat neexistujici material -> viz Resource Manager");
                break;
        }
        return PotravinyBigBox; //shouldn't happen, TODO use some other 'red warning box' or something
    }

    public void IncPohonneHmoty(int value)
    {
        SpawnMaterial(Material.Pohonne, value);
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
        ResourceAmountChanged();
    }

    // Update is called once per frame

    public void ResourceAmountChanged()
    {
        /*  Updatuje nase suroviny v gui */
        
             text[0].text = GetResourceCount(Material.Potraviny).ToString();
             text[1].text = GetResourceCount(Material.Vojensky).ToString();
             text[2].text = GetResourceCount(Material.Technicky).ToString();
             text[3].text = GetResourceCount(Material.Civilni).ToString();
             text[4].text = GetResourceCount(Material.Pohonne).ToString();
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

    public Resource PickUp(Vector2Int position)
    {
        if (tileFactory.getTile(position) is Tile t)
        {
            return GetResource(t);
        }
//TODO mozna bude mozne i prebirat nevyuzite/zrusene surky z rozestavenych budov etc...
        return null;
    }

    public Resource GetResource(System.Object owner)
    {   
        var res = resources.Where(r => r.Owner == owner).ToList();
        if (res.Count() == 1)
        {
            return res.First();
        }

        return null;
    }

    public List<Resource> GetResourcesForOwner(Object owner)
    {
        return resources.Where(r => r.Owner == owner).ToList();
    }
    public void SpawnMaterial(Material typ, int amount)
    {

        if ((10 > GetResourceCount(typ) && amount == -10) || (GetResourceCount(typ) <= 0 && amount < 0))
        {
            Debug.Log("U cant have negative resources !!");
            return;
        }

        if (amount > 0)
        {
            
  
            if (amount > 10)
            {
                SpawnMaterial(typ, amount - 10);
            }
            var coord = tileFactory.FindFreeTile(resources);
            var newResourse = new Resource(amount, typ, tileFactory.getTile(coord));
            resources.Add(newResourse);
        }
        else
        {
            if (!resources.Any()) 
                return;

            var firstBox = resources.First(res => res.Material == typ);

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
        Resource cheapest = null;
        int smallestSteps = int.MaxValue;
        foreach (var box in res.Where(r => r.Owner is Tile))
        {
            var tile = (Tile) box.Owner;
            Vector2Int position = new Vector2Int(tile.x, tile.y);
            var steps = tileFactory.FindPath(from, position ).Count;
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
        return resources.Where(r => r.Material == type).ToList();
    }

    public int GetResourceCount(Material type)
    {
        return GetAllBoxesOfType(type).Sum(box => box.Amount);
    }
}
