using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class ResourceManager : MonoBehaviour
{
   
    private TileFactory tileFactory;
    private List<IResourceHolder> resourceHolders;
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
        resourceHolders = new List<IResourceHolder>();
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

    public struct ResourceAmount
    {
        public int Food;
        public int Military;
        public int Technical;
        public int Civilian;
        public int Fuel;
        public int Karma;
        public int Kids;
        public int Energy;

        public static ResourceAmount operator +(ResourceAmount src, ResourceAmount dst)
        {
            src.Food += dst.Food;
            src.Military += dst.Military;
            src.Technical += dst.Technical;
            src.Civilian += dst.Civilian;
            src.Fuel += dst.Fuel;
            src.Karma += dst.Karma;
            src.Kids += dst.Kids;
            src.Energy += dst.Energy;
            return src;
        }
        public static ResourceAmount operator -(ResourceAmount src, ResourceAmount dst)
        {
            src.Food -= dst.Food;
            src.Military -= dst.Military;
            src.Technical -= dst.Technical;
            src.Civilian -= dst.Civilian;
            src.Fuel -= dst.Fuel;
            src.Karma -= dst.Karma;
            src.Kids -= dst.Kids;
            src.Energy -= dst.Energy;
            return src;
        }

        internal bool HasEnough(ResourceAmount amount)
        {
            if (Food < amount.Food) return false;
            if (Military < amount.Military) return false;
            if (Technical < amount.Technical) return false;
            if (Civilian < amount.Civilian) return false;
            if (Fuel < amount.Fuel) return false;
            if (Karma < amount.Karma) return false;
            if (Kids < amount.Kids) return false;
            if (Energy < amount.Energy) return false;
            return true;
        }

        internal bool Empty()
        {
            if (Food > 0) return false;
            if (Military > 0) return false;
            if (Technical > 0) return false;
            if (Civilian > 0) return false;
            if (Fuel > 0) return false;
            if (Karma > 0) return false;
            if (Kids > 0) return false;
            if (Energy > 0) return false;
            return true;
        }

        public bool HasAny(ResourceAmount amount)
        {
            if (amount.Food > 0)
            {
                if (Food > 0) return true;
            }
            if (amount.Military> 0)
            {
                if (Military> 0) return true;
            }
            if (amount.Technical> 0)
            {
                if (Technical > 0) return true;
            }
            if (amount.Civilian > 0)
            {
                if (Civilian > 0) return true;
            }
            if (amount.Fuel > 0)
            {
                if (Fuel > 0) return true;
            }
            if (amount.Energy > 0)
            {
                if (Energy > 0) return true;
            }

            return false;
        }
    }
    public Text[] text;

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

    Vector2Int defaultSpawnPoint = new Vector2Int(20, 20);
    public void IncPohonneHmoty(int value)
    {
        var val = new ResourceAmount();
        val.Fuel = value;

        SpawnResource(val, defaultSpawnPoint);
    }

    public void IncPotraviny(int value)
    {
        var val = new ResourceAmount();
        val.Food = value;
        if (value < 0)
        {
            var allFoods = resourceHolders.Where(r => r.Amount.Food > 0);
            foreach (var f in allFoods.ToList())
            {
                val.Food = -value;
                if (value == 0) break;
                if (f.Amount.Food < val.Food)
                {
                    val.Food = f.Amount.Food;
                }
                value += val.Food;
                f.Remove(val);
            }
            ResourceAmountChanged();
        }
        else
        {
            SpawnResource(val, defaultSpawnPoint);
        }
    }
    public void IncVojenskyMaterialy(int value)
    {

        var val = new ResourceAmount();
        val.Military = value;

        SpawnResource(val, defaultSpawnPoint);
    }
    public void IncTechnickyMaterial(int value)
    {

        var val = new ResourceAmount();
        val.Technical = value;

        SpawnResource(val, defaultSpawnPoint);
    }
    public void IncCivilniMaterial(int value)
    {
        var val = new ResourceAmount();
        val.Civilian = value;

        SpawnResource(val, defaultSpawnPoint);
    }
    public void IncEnergie(int value)
    {
        var val = new ResourceAmount();
        val.Energy = value;

        SpawnResource(val, defaultSpawnPoint);
    }

    public void IncDeti(int value)
    {
        var val = new ResourceAmount();
        val.Kids = value;

        SpawnResource(val, defaultSpawnPoint);
    }

    public void IncKarma(int value)
    {
        var val = new ResourceAmount();
        val.Karma = value;

        SpawnResource(val, defaultSpawnPoint);
    }

    public void SetToZero()
    {
        foreach (var holder in resourceHolders)
        {
            holder.Set(new ResourceAmount());
        }
        ResourceAmountChanged();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();
        ResourceAmountChanged();
    }

    public void ResourceAmountChanged()
    {
        /*  Updatuje nase suroviny v gui */
        var resources = new ResourceAmount();
        foreach (var holder in resourceHolders)
        {
            resources += holder.Amount;
        }
        text[0].text = resources.Food.ToString();
        text[1].text = resources.Military.ToString();
        text[2].text = resources.Technical.ToString();
        text[3].text = resources.Civilian.ToString();
        text[4].text = resources.Fuel.ToString();
        text[5].text = resources.Energy.ToString();
        text[6].text = resources.Kids.ToString();
        text[7].text = resources.Karma.ToString();
    }

    public void AddAll(int value)
    {
        var val = new ResourceAmount();
        val.Food = value;
        val.Civilian = value;
        //val.Energy = value;
        val.Fuel = value;
        //val.Karma = value;
        // val.Kids = value;
        val.Military = value;
        val.Technical = value;
        SpawnResource(val, defaultSpawnPoint);
    }

    //for backward compatibility
    public void SpawnResource(ResourceAmount resourceAmount, Vector2Int where = default)
    {
        while (!resourceAmount.Empty())
        {
            var coord = tileFactory.FindFreeTile(where).First();
            if (tileFactory.getTile(coord) is IResourceHolder resourceHolder)
            {
                resourceAmount = resourceHolder.Add(resourceAmount);
            }
        }

        ResourceAmountChanged();
    }

    public void Register(IResourceHolder holder)
    {
        if (!resourceHolders.Contains(holder))
        {
            resourceHolders.Add(holder);
        }
    }

    public void Unregister(IResourceHolder holder)
    {
        if (resourceHolders.Contains(holder))
        {
            resourceHolders.Remove(holder);
        }
    }
    public Vector2Int Nearest(Vector2Int from, ResourceAmount amount)
    {
        var res = GetAllResourceHolders(amount);
        Vector2Int cheapest = Vector2Int.Max(default, default);
        int smallestSteps = int.MaxValue;
        foreach (var box in res.Where(r => r is Tile))
        {
            var tile = (Tile) box; //TODO muze byt i sklad
            Vector2Int position = new Vector2Int(tile.x, tile.y);
            var steps = tileFactory.FindPath(from, position ).Count;
            if (steps < smallestSteps)
            {
                smallestSteps = steps;
                cheapest = position;
            }
        }
        return cheapest;
    }

    private List<IResourceHolder> GetAllResourceHolders(ResourceAmount amount)
    {
        return resourceHolders.Where(r => r.Amount.HasAny(amount)).ToList();
    }

}
