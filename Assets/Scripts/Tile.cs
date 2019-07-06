using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject tile;
    //for pathfinding
    public int gCost;
    public int hCost;
    public int x;
    public int y;
    public bool walkable;
    public Tile parent;
    public int fCost
    {
        get { return gCost + hCost; }
    }
    public Tile(GameObject tile, int x, int y, bool walkable)
    {
        this.tile = tile;
        this.x = x;
        this.y = y;
        this.walkable = walkable;
    }
}
