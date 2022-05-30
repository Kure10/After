using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : BaseTile
{
    int hp = 1500; //TODO probably move to interface and take start value from text config
    public WallTile(GameObject tile, int x, int y, int rotation) : base(tile, x, y, rotation)
    {
    }
}
