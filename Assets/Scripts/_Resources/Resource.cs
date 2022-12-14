using Buildings;
using UnityEngine;
using Object = UnityEngine.Object;

public class Resource
{
    private int amount;
    private  GameObject prefab;
    public ResourceManager.Material Material { get; }
    private System.Object owner;

    public System.Object Owner
    {
        get => owner;
        set
        {
            owner = value;
            ChangePrefab(amount);
        }
    }

    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
            ChangePrefab(value);
        }
    }
    
    public Resource(int amount, ResourceManager.Material material, System.Object owner)
    {
        this.amount = amount;
        Material = material;
        Owner = owner;
        ChangePrefab(amount);
    }

    private void ChangePrefab(int newAmount)
    {
        if (prefab != null)
        {
            Object.Destroy(prefab);
        }

        if (newAmount == 0) return;
        //Prefab looks different depending on who is the owner - laying on the floor/in the storage/carried by person...
        //for now take care only for tiles, TODO solve this for other owners
        if (Owner is Tile t)
        {
            prefab = Object.Instantiate(ResourceManager.GetPrefab(newAmount, Material), t.tile.transform.position, Quaternion.identity);
        }

        if (Owner is Building b)
        {
            var position = b.GetPosition();
            if (Owner is Warehouse w)
            {
                position = w.GetPosition();
            } //meh, don't know why this don't get overrided, temporary hack
            prefab = Object.Instantiate(ResourceManager.GetPrefab(newAmount, Material), position, Quaternion.identity);
        }
    }
}