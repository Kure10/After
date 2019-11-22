using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Resource
{
    public Resource(int amount, ResourceManager.Material material, GameObject smallBox, GameObject bigBox, Vector2Int coord)
    {
        this.smallBox = smallBox;
        this.bigBox = bigBox;
        this.amount = amount;
        this.material = material;
        position = coord;
        ChangePrefab(amount);
    }
    private int amount;
    private  GameObject prefab;
    private GameObject smallBox;
    private GameObject bigBox;
    public ResourceManager.Material material;
    public int Amount
    {
        get => amount;
        set
        {
            Object.Destroy(prefab);
            ChangePrefab(value);
            amount = value;
        }
    }
    public Vector2Int position;

    private void ChangePrefab(int newAmount)
    {
        switch (newAmount)
        {
            case 0:
                break;
            case 10:
                prefab = Object.Instantiate(bigBox, Geometry.PointFromGrid(position), Quaternion.identity );
                break;
            default:
                prefab =  Object.Instantiate(smallBox, Geometry.PointFromGrid(position), Quaternion.identity);
                break;
        }
    }
}
