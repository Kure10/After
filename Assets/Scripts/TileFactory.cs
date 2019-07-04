﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFactory : MonoBehaviour
{
    public GameObject emptyTile;
    public GameObject wallTile;
    public GameObject outsideTile;
    public GameObject debrisTile;
    public TextAsset level;
    private GameObject[,] grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    GameObject[,] CreateGrid()
    {
        var lines = level.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
        var tempGrid = new List<List<GameObject>>();
        int x = 0;
        int z = 0;
        foreach (var line in lines)
        {
            if (line[0] == '#')
                continue;
            var col = new List<GameObject>();
            tempGrid.Add(col);
            foreach (var type in line)
            {
                GameObject go = null;
                switch (char.ToLower(type))
                {
                    case ' ': continue;
                    case 'w': go = wallTile; break;
                    case 'e': go = emptyTile; break;
                    case 's': go = emptyTile; break; //TODO add player
                    case 'd': go = debrisTile; break;
                    case '0': go = outsideTile; break;
                    default: Debug.Log($"Unknown object: {type}"); continue;
                }
                x++;
                Vector2Int gridPoint = Geometry.GridPoint(x, z);
                col.Add(Instantiate(go, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform));
            }
            x = 0;
            z++;
        }
        var ret = new GameObject[tempGrid[0].Count, tempGrid.Count];
        for(int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                ret[i, j] = tempGrid[i][j];
            }
        }
        return ret;
    }
}
