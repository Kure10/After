using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFactory : MonoBehaviour
{
    public GameObject emptyTile;
    public GameObject wallTile;
    public GameObject outsideTile;
    public GameObject debrisTile;
    public GameObject character;
    public TextAsset level;
    private Tile[,] grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Tile getTile(Vector2Int coordinates)
    {
        return getTile(coordinates.x, coordinates.y);
    }
    public Tile getTile(int col, int row)
    {
        return grid[col, row];
    }
    Tile[,] CreateGrid()
    {
        var lines = level.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
        var checkedLines = new List<string>();
        var tempGrid = new List<List<Tile>>();
        int x = 0;
        int z = 0;
        int rows = 0;
        foreach (var line in lines)
        {
            if (line[0] == '#') continue;
            checkedLines.Add(line);
        }
        checkedLines.Reverse();
        foreach (var line in checkedLines)
        {
            var col = new List<Tile>();
            tempGrid.Add(col);
            foreach (var type in line)
            {
                bool walkable = true;
                GameObject go = null;
                Vector2Int gridPoint = Geometry.GridPoint(x, z);
                switch (char.ToLower(type))
                {
                    case ' ': continue;
                    case 'w': go = wallTile; walkable = false; break;
                    case 'e': go = emptyTile; break;
                    case 's': go = emptyTile;
                              Instantiate(character, Geometry.PointFromGrid(gridPoint), Quaternion.identity);
                              break;
                    case 'd': go = debrisTile; walkable = false;  break;
                    case '0': go = outsideTile; break;
                    default: Debug.Log($"Unknown object: {type} at {gridPoint.x} : {gridPoint.y}"); continue;
                }
                col.Add(new Tile(Instantiate(go, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform), x, z, walkable));
                x++;
            }
            x = 0;
            z++;

        }
        var ret = new Tile[tempGrid[0].Count, tempGrid.Count];
        foreach (var row in tempGrid)
        {
            foreach(var item in row)
            {
                ret[item.x, item.y] = item;
            }
        }
        return ret;
    }
    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        var start = grid[from.x, from.y];
        var target = grid[to.x, to.y];
        if (!start.walkable && !target.walkable)
            return null;
        var path = new List<Vector2Int>();
        start.gCost = 100;
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Tile node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost <= node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                    {
                        node = openSet[i];
                    }
                }
            }
            closedSet.Add(node);
            openSet.Remove(node);

            if (node == target)
            {
                //we found our path
                return RetracePath(start,target);
            }

            foreach (var neighbour in GetNeighbours(node))
            {
                if ((!neighbour.walkable && neighbour != target) || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int costToNeighbour = node.gCost + GetCost(node, neighbour);
                if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = GetCost(neighbour, target);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }

        }
        return null;
    }
    public List<Tile> GetNeighbours(Tile node)
    {
        List<Tile> neighbours = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                int checkX = node.x + x;
                int checkY = node.y + y;
                int gridSizeX = grid.GetLength(0);
                int gridSizeY = grid.GetLength(1);
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
    int GetCost(Tile nodeA, Tile nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    private List<Vector2Int> RetracePath(Tile start, Tile end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Tile currentNode = end;

        while (currentNode != start)
        {
            path.Add(new Vector2Int(currentNode.x, currentNode.y));
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
}
