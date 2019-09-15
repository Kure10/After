using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : BaseTile, IWalkable
{
    public Tile(GameObject tile, int x, int y) : base(tile, x, y)
    {
        walkthrough = true;
    }

    public int gCost { get; set; }
    public int hCost { get; set; }
    public bool walkthrough { get; set; }
    public GameObject building;
    public GameObject resourceBox;
    public bool inside;
    public int fCost => gCost + hCost;
    public BaseTile parent { get; set; }
    int IWalkable.x => this.x;
    int IWalkable.y => this.y;

    IWalkable IWalkable.parent { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
