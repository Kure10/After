using System.Collections;
using System.Collections.Generic;
using Resources;
using UnityEngine;
using static ResourceManager;

public class Tile : BaseTile, IWalkable, IResourceHolder
{
    public Tile(GameObject tile, int x, int y) : base(tile, x, y)
    {
        walkthrough = true;
        Amount = new ResourceAmount();
    }
    public ResourceAmount Amount { get; set; }
    public int gCost { get; set; }
    public int hCost { get; set; }
    public bool walkthrough { get; set; }
    public Building building;
    public bool inside;
    public int fCost => gCost + hCost;
    public BaseTile parent { get; set; }
    int IWalkable.x => this.x;
    int IWalkable.y => this.y;
    private Resource resource;
    IWalkable IWalkable.parent { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(ResourceAmount amount)
    {
        Amount = amount;
        RefreshMaterials();
    }

    public ResourceAmount Add(ResourceAmount amount)
    {
        GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>().Register(this);
        Amount += amount;
        var realAmount = FixAmount();
        var toReturn = Amount - realAmount;
        Amount = realAmount;
        RefreshMaterials();
        return toReturn;
    }

    ResourceAmount FixAmount()
    {
        var realResourceAmount = new ResourceAmount();
        if (Amount.Food > 0)
        {
            if (Amount.Food > 10)
            {
                realResourceAmount.Food = 10;
            }

            realResourceAmount.Food = Amount.Food;
            return realResourceAmount;
        }
        
        if (Amount.Civilian> 0)
        {
            if (Amount.Civilian> 10)
            {
                realResourceAmount.Civilian= 10;
            }

            realResourceAmount.Civilian= Amount.Civilian;
            return realResourceAmount;
        }
        
        if (Amount.Technical> 0)
        {
            if (Amount.Technical> 10)
            {
                realResourceAmount.Technical= 10;
            }

            realResourceAmount.Technical= Amount.Technical;
            return realResourceAmount;
        }
        if (Amount.Fuel> 0)
        {
            if (Amount.Fuel> 10)
            {
                realResourceAmount.Fuel= 10;
            }

            realResourceAmount.Fuel = Amount.Fuel;
            return realResourceAmount;
        }
        if (Amount.Military> 0)
        {
            if (Amount.Military> 10)
            {
                realResourceAmount.Military= 10;
            }

            realResourceAmount.Military= Amount.Military;
            return realResourceAmount;
        }
        
        return realResourceAmount;
    }

    private void RefreshMaterials()
    {
        if (resource != null)
        {
            resource.Amount = 0; //destroy old one
        }
        if (Amount.Food > 0)
        {
            resource = new Resource(Amount.Food, ResourceManager.Material.Potraviny, this);
        }
        if (Amount.Civilian> 0)
        {
            resource = new Resource(Amount.Civilian, ResourceManager.Material.Civilni, this);
        }
        if (Amount.Technical > 0)
        {
            resource = new Resource(Amount.Technical, ResourceManager.Material.Technicky, this);
        }
        if (Amount.Fuel> 0)
        {
            resource = new Resource(Amount.Fuel, ResourceManager.Material.Pohonne, this);
        }
        if (Amount.Military > 0)
        {
            resource = new Resource(Amount.Military, ResourceManager.Material.Vojensky, this);
        }
    }

    public ResourceAmount Remove(ResourceAmount amount)
    {
        Amount -= amount;
        if (Amount.Empty())
        {
            GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>().Unregister(this);
        }
        RefreshMaterials();
        return amount; //TODO vrat skutecny pocet ziskanych resources
    }
}
