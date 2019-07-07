using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisTile : Tile
{
    public int hp;
    public DebrisTile(GameObject tile, int x, int y) : base(tile, x, y)
    {
        walkthrough = false;
        hp = 100;
    }
}
