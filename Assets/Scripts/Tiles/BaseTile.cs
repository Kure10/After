using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile
{
    public GameObject tile;  
    public int x;
    public int y;
   
    public BaseTile(GameObject tile, int x, int y)
    {
        this.tile = tile;
        this.x = x;
        this.y = y;
    }
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