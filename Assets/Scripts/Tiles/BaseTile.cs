using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile
{
    public GameObject tile;  
    public int x;
    public int y;
    private int rotation;
   
    public BaseTile(GameObject tile, int x, int y, int _rotation = 0)
    {
        this.tile = tile;
        this.x = x;
        this.y = y;
        this.rotation = _rotation;
        tile.name = $" {tile.name} {x} {y} ";
    }

    public int GetRotation { get { return this.rotation; } }
}
interface IWalkable
{
    IWalkable parent { get; set; }
    int gCost { get; set; }
    int hCost { get; set; }
    bool walkthrough { get; set; }
     int fCost { get; }
    int x { get; }
    int y { get; }

}