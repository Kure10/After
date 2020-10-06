using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TileFactory : MonoBehaviour
{
    public GameObject emptyTile;
    public GameObject wallTile;
    public GameObject outsideTile;
    public GameObject debrisTile;
    public GameObject character;
    public GameObject DebrisHealthbar;
    public TextAsset level;
    private BaseTile[,] grid;
    private readonly int TILE = 1 << 8;
    public Text coordText;
    private ResourceManager rm;
    private Ray ray;

    private RaycastHit hit;
    private Vector2Int specPosition;

    private List<Vector2Int> occupiedTiles;
    // Start is called before the first frame update
    void Start()
    {
        occupiedTiles = new List<Vector2Int>();
        rm = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        grid = CreateGrid();
        var specControler = GameObject.FindGameObjectWithTag("SpecialistController").GetComponent<SpecialistControler>();
        specControler.CreateStartingCharacters(specPosition);

        specControler.TestMove(specControler.InGameSpecialists);
        //var spec = specControler.GetStartedCharacters(specPosition);
        //List<Vector2Int> alreadyPlaced = new List<Vector2Int>();
        //foreach (var specialist in spec)
        //{
        //    var gridPoint = FindFreeTile(specPosition, alreadyPlaced).First();
        //    alreadyPlaced.Add(gridPoint);
        //    var person = Instantiate(character, Geometry.PointFromGrid(gridPoint), Quaternion.identity);
        //    person.GetComponent<Character>().SetBlueprint(specialist);
        //}


    }
    //Mark tile as occupied (for ie by specilalist) so no one else can move here
    //return false if already occupied
    public bool OccupyTile(Vector2Int coord)
    {
        if (grid[coord.x, coord.y] is IWalkable tile)
        {
            if (IsOccupied(coord)) return false;
            occupiedTiles.Add(coord);
            return true;
        }
        return false;
    }

    public bool IsOccupied(Vector2Int coord)
    {
        foreach (var candidate in occupiedTiles)
        {
            if (candidate.x == coord.x && candidate.y == coord.y)
            {
                return true;
            }
        }
        return false;
    }
    public void LeaveTile(Vector2Int coord)
    {
        foreach (var candidate in occupiedTiles.ToArray())
        {
            if (candidate.x == coord.x && candidate.y == coord.y)
            {
                occupiedTiles.Remove(candidate);
            }
        }
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, TILE))
        {
            Vector3 point = hit.point;
            var coord = Geometry.GridFromPoint(point);
            coordText.text = $"({coord.x},{coord.y})";
        }
    }

    public BaseTile getTile(Vector2Int coordinates)
    {
        return getTile(coordinates.x, coordinates.y);
    }

    public BaseTile getTile(int col, int row)
    {
        return grid[col, row];
    }

    BaseTile[,] CreateGrid()
    {
        var lines = level.text.Split(new string[] {System.Environment.NewLine},
            System.StringSplitOptions.RemoveEmptyEntries);
        var checkedLines = new List<string>();
        var tempGrid = new List<List<BaseTile>>();
        int x = 0;
        int z = 0;
        int rows = 0;
        var specialistsInGame = 0;
        var rng = new Random();
        foreach (var line in lines)
        {
            if (line[0] == '#') continue;
            checkedLines.Add(line);
        }

        checkedLines.Reverse();
        foreach (var line in checkedLines)
        {
            var col = new List<BaseTile>();
            tempGrid.Add(col);
            foreach (var type in line)
            {
                bool walkable = true;
                Vector2Int gridPoint = Geometry.GridPoint(x, z);
                switch (char.ToLower(type))
                {
                    case ' ': continue;
                    case 'w':
                        col.Add(new WallTile(generateTilePrefab(wallTile, gridPoint), x, z));
                        break;
                    case 'e':
                        col.Add(new Tile(generateTilePrefab(emptyTile, gridPoint), x, z) {inside = true});
                        break;
                    case 's':
                        //changed - now all specialists are placed around the first point with this symbol, skip if more than one
                        if (specialistsInGame == 0)
                        {
                            specialistsInGame++;
                            //save this position and generate specialist around this point (need to be generated after everything else to know where the free tiles are
                            specPosition = gridPoint;
                        }

                        goto case 'e';
                    case 'd':
                        var item = new DebrisTile(generateTilePrefab(debrisTile, gridPoint), x, z);
                        var holder = item.tile.transform.Find("Hromadka_suti/holder");
                        holder.Rotate(Vector3.up, (float)rng.NextDouble() * 180);
                        var rand = (float)rng.NextDouble() * 0.2f;
                        var newscale = new Vector3(0.9f + rand, 0.5f + (float)rng.NextDouble(), 0.9f + rand);
                        holder.localScale = newscale;
                        col.Add(item);
                        break;
                    case '0':
                        col.Add(new Tile(generateTilePrefab(outsideTile, gridPoint), x, z) {inside = false});
                        break;
                    default:
                        Debug.Log($"Unknown object: {type} at {gridPoint.x} : {gridPoint.y}");
                        continue;
                }

                x++;
            }

            x = 0;
            z++;
        }

        var ret = new BaseTile[tempGrid[0].Count, tempGrid.Count];
        foreach (var row in tempGrid)
        {
            foreach (var item in row)
            {
                ret[item.x, item.y] = item;
            }
        }

     

        return ret;
    }

    private GameObject generateTilePrefab(GameObject prefab, Vector2Int gridPoint)
    {
        return Instantiate(prefab, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform);
    }

    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        var start = grid[from.x, from.y] as IWalkable;
        var target = grid[to.x, to.y] as IWalkable;
        if (start == null || target == null)
        {
            return null;
        }

        start.gCost = 100;
        List<IWalkable> openSet = new List<IWalkable>();
        HashSet<IWalkable> closedSet = new HashSet<IWalkable>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            IWalkable node = openSet[0];
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
                return RetracePath(start, target);
            }

            foreach (var neighbour in GetNeighbours(node))
            {
                if ((!neighbour.walkthrough && !node.walkthrough))
                {
                    continue;
                }

                if (closedSet.Contains(neighbour))
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

    private List<IWalkable> GetNeighbours(IWalkable node)
    {
        List<IWalkable> neighbours = new List<IWalkable>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                //disallow diagonal movement;
                if (Mathf.Abs(x) == Mathf.Abs(y))
                    continue;

                int checkX = node.x + x;
                int checkY = node.y + y;
                int gridSizeX = grid.GetLength(0);
                int gridSizeY = grid.GetLength(1);
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if (grid[checkX, checkY] is IWalkable g) // && !(grid[checkX, checkY] is DebrisTile))
                        neighbours.Add(g);
                }
            }
        }

        return neighbours;
    }

    int GetCost(IWalkable nodeA, IWalkable nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        return 10 * dstX + 10 * dstY;
        /*
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
        */
    }

    public void ClearDebris(int col, int row)
    {
        grid[col, row] = new Tile(generateTilePrefab(emptyTile, new Vector2Int(col, row)), col, row) {inside = true};
    }
    public bool Buildable(Vector2Int coord)
    {
        bool ret = false;
        Tile highlight = null;
        if (grid[coord.x, coord.y] is Tile tile && !(grid[coord.x, coord.y] is DebrisTile))
        {
            highlight = tile;
            if (!tile.inside)
                return false;
            if (tile.building == null)
            {
                ret = true;
            }
        }

        /* //NEMAZAT pouzivam pro debug
        if (highlight != null)
        {
            highlight.tile.transform.GetComponent<Renderer>().material.color = ret ? Color.green : Color.red;
        } */


        return ret;
    }

    public void AddBuilding(List<Vector2Int> coords, Building building)
    {
        foreach (var coord in coords)
        {
            if (grid[coord.x, coord.y] is Tile t)
            {
                t.building = building;
            }
        }
    }


    private List<Vector2Int> RetracePath(IWalkable start, IWalkable end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        IWalkable currentNode = end;

        while (currentNode != start)
        {
            path.Add(new Vector2Int(currentNode.x, currentNode.y));
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }


    //Finds first emptyTile with no building and no resource box
    public Vector2Int FindFreeTile()
    {
        return FindFreeTile(new Vector2Int(10, 31)).First();
    }

    //Find first emtyTile but start searching from start point
    public List<Vector2Int> FindFreeTile(Vector2Int startPoint, [CanBeNull] List<Vector2Int> forbiddenTiles = null, bool allowResources = false)
    {
        var ret = new List<Vector2Int>();
        List<Resource> resOnTiles = new List<Resource>();
        if (!allowResources)
        {
            resOnTiles = rm.resources == null ? new List<Resource>() : rm.resources.Where(r => r.Owner is Tile).ToList();
        }

        int distance = 0;
        while (distance < 20)
        {
            for (int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                {
                    var locX = startPoint.x + x;
                    var locY = startPoint.y + y;
                    if (locX < 0 || locY < 0) continue;
                    if (locX >= grid.GetLength(0) || locY >= grid.GetLength(1)) continue;

                    if (Math.Abs(x) < distance && Math.Abs(y) < distance)
                        continue;
                    if (grid[startPoint.x + x, startPoint.y + y] is Tile t)
                    {
                        if (t.building == null && t.inside == true)
                        {
                            var empty = true;
                            var candidate = new Vector2Int(t.x, t.y);
                            if (!(forbiddenTiles is null))
                            {
                                if (forbiddenTiles.Contains(candidate))
                                {
                                    empty = false;
                                }
                            }
                                
                            foreach (var res in resOnTiles)
                            {
                                if (res.Owner == t)
                                {
                                    empty = false;
                                    break;
                                }
                            }

                            if (empty)
                            {
                               ret.Add(Geometry.GridFromPoint(t.tile.transform.position));
                            }
                        }
                    }
                }
            }
            distance++;
        }
        return ret;
    }

    public List<Vector2Int> GetOccupiedTiles()
    {
        return occupiedTiles;
    }
}