using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Resources;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using LogSystem;

public class TileFactory : MonoBehaviour
{
    public GameObject emptyTile;
    public GameObject outsideTile;
    public GameObject debrisTile;
    public GameObject character;
    public GameObject DebrisHealthbar;
    public TextAsset level;
    private BaseTile[,] _grid;
    private readonly int TILE = 1 << 8;
    public Text coordText;

    [Header ("WallTiles")]
    [SerializeField] GameObject wallTileDefault;
    [SerializeField] GameObject wallTileSimple;
    [SerializeField] GameObject wallTile2Paraller;
    [SerializeField] GameObject wallTile2Corner;
    [SerializeField] GameObject wallTile3Sides;
    [Space]

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
        _grid = CreateGrid();

        UpdateFog();
        var specControler = GameObject.FindGameObjectWithTag("SpecialistController").GetComponent<SpecialistControler>();
        specControler.CreateStartingCharacters(specPosition);

        //  specControler.TestMove();

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
        if (_grid[coord.x, coord.y] is IWalkable tile)
        {
            if (IsOccupied(coord)) return false;
            occupiedTiles.Add(coord);
            return true;
        }

        return false;
    }

    private static int columns = 0;
    private static int rows = 0;

    public void UpdateFog()
    {
        if (columns == 0)
        {
            columns = _grid.GetUpperBound(0);
            rows = _grid.Length / columns - 1;
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (_grid[x, y] is DebrisTile d)
                {
                    checkFog(d.tile, x, y);
                }
                else if (_grid[x, y] is WallTile w)
                {
                    checkFog(w.tile, x, y);
                }
            }
        }
    }

    private void checkFog(GameObject tile, int x, int y)
    {
        bool found = false;
        //check if any of neighbourhood's tiles is empty tile - if so, remove the fog
        for (int x2 = -1; x2 <= 1; x2++)
        {
            for (int y2 = -1; y2 <= 1; y2++)
            {
                if (x + x2 < 0) continue;
                if (y + y2 < 0) continue;
                if (x + x2 > columns) continue;
                if (y + y2 > rows) continue;
                if (_grid[x + x2, y + y2] is IWalkable t && t.walkthrough)
                {
                    tile.transform.Find("Fog").transform.gameObject.SetActive(false);
                    found = true;
                    break;
                }
            }

            if (found) break;
        }
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
        return _grid[col, row];
    }

    BaseTile[,] CreateGrid()
    {
        var lines = level.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        var checkedLines = new List<string>();
        List<List<BaseTile>> tempGrid = new List<List<BaseTile>>();
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
        foreach (string line in checkedLines)
        {
            List<BaseTile> col = new List<BaseTile>();
            tempGrid.Add(col);
            foreach (char type in line)
            {
                bool walkable = true;
                Vector2Int gridPoint = Geometry.GridPoint(x, z);
                switch (char.ToLower(type))
                {
                    case ' ': continue;
                    case 'w':

                        WallDirection wallStyle = WallStyleSelection(gridPoint, checkedLines);
                        GameObject wallTileObject = GetWallGameObject(wallStyle);
                        col.Add(new WallTile(generateTilePrefab(wallTileObject, gridPoint, wallStyle), x, z, wallStyle.rotation));

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
                        holder.Rotate(Vector3.up, (float) rng.NextDouble() * 180);
                        var rand = (float) rng.NextDouble() * 0.2f;
                        var newscale = new Vector3(0.9f + rand, 0.5f + (float) rng.NextDouble(), 0.9f + rand);
                        holder.localScale = newscale;
                        col.Add(item);
                        break;
                    case '0':
                        col.Add(new Tile(generateTilePrefab(outsideTile, gridPoint), x, z) {inside = false});
                        break;
                    default:
                        MyLogSystem.LogError($"Unknown object: {type} at {gridPoint.x} : {gridPoint.y}");
                        continue;
                }

                x++;
            }

            x = 0;
            z++;
        }

        BaseTile[,] ret = new BaseTile[tempGrid[0].Count, tempGrid.Count];
        foreach (var row in tempGrid)
        {
            foreach (var item in row)
            {
                ret[item.x, item.y] = item;
            }
        }


        return ret;
    }

    private GameObject generateTilePrefab(GameObject prefab, Vector2Int gridPoint, WallDirection wallStyle = null)
    {
        bool isWall = false;
        if(wallStyle != null)
            isWall = true;
     
        GameObject newTile = Instantiate(prefab, Geometry.PointFromGrid(gridPoint, isWall), Quaternion.identity, gameObject.transform);

        if (wallStyle != null)
            newTile.transform.eulerAngles = new Vector3(0, wallStyle.rotation, 0);

        return newTile;
    }

    private WallDirection WallStyleSelection(Vector2Int gridPoint, List<string> checkedLines)
    {
        WallDirection wallDirection = new WallDirection();
        string line = checkedLines[gridPoint.y];
        string trimmed = String.Concat(line.Where(c => !Char.IsWhiteSpace(c)));

        char right = trimmed[gridPoint.x + 1];
        char left = trimmed[gridPoint.x - 1];

        line = checkedLines[gridPoint.y - 1];
        trimmed = String.Concat(line.Where(c => !Char.IsWhiteSpace(c)));
        char up = trimmed[gridPoint.x];

        line = checkedLines[gridPoint.y + 1];
        trimmed = String.Concat(line.Where(c => !Char.IsWhiteSpace(c)));
        char down = trimmed[gridPoint.x];

        int numberOfSideWalls = 0;
        bool leftW = false;
        bool rightW = false;
        bool upW = false;
        bool downW = false;

        if (left == 'W')
        {
            numberOfSideWalls++;
            leftW = true;
        }

        if (right == 'W')
        {
            numberOfSideWalls++;
            rightW = true;
        }

        if (up == 'W')
        {
            numberOfSideWalls++;
            upW = true;
        }

        if (down == 'W')
        {
            numberOfSideWalls++;
            downW = true;
        }

        switch (numberOfSideWalls)
        {
            case 0:
                wallDirection.type = WallDirection.WallType.undefined;
                break;
            case 1:
                wallDirection.rotation = GetThreeWallRotation(leftW, rightW, downW, upW);
                wallDirection.type = WallDirection.WallType.wall3Sides;

                break;
            case 2:
                bool isParaller = false;
                wallDirection.rotation = GetTwoWallRotation(leftW, rightW, downW, upW, out isParaller);

                if (isParaller)
                    wallDirection.type = WallDirection.WallType.wall2Paraller;
                else
                    wallDirection.type = WallDirection.WallType.wall2Corner;

                break;
            case 3:
                wallDirection.rotation = GetOneWallRotation(leftW, rightW, downW, upW);
                wallDirection.type = WallDirection.WallType.wallSimple;
                break;
            case 4:
                wallDirection.type = WallDirection.WallType.undefined;
                break;
            default:
                break;
        }

        return wallDirection;

        int GetOneWallRotation(bool leftS, bool rightS, bool downS, bool upS)
        {
            int rotation = 0;
            if (!leftS)
                rotation = 270;
            else if (!rightS)
                rotation = 90;
            else if (!downS)
                rotation = 0;
            else
                rotation = 180;

            return rotation;
        }

        int GetTwoWallRotation(bool leftS, bool rightS, bool downS, bool upS, out bool isParaller)
        {
            int rotation = 0;

            if (downS && upS)
            {
                isParaller = true;
                rotation = 0;
            }
            else if (leftS && rightS)
            {
                isParaller = true;
                rotation = 90;
            }
            else if (upS && rightS)
            {
                isParaller = false;
                rotation = 0;
            }
            else if (rightS && downS)
            {
                isParaller = false;
                rotation = 270;
            }
            else if (downS && leftS)
            {
                isParaller = false;
                rotation = 180;
            }
            else
            {
                isParaller = false;
                rotation = 90;
            }

            return rotation;
        }

        int GetThreeWallRotation(bool leftS, bool rightS, bool downS, bool upS)
        {
            int rotation = 0;

            if (leftS)
                rotation = 90;
            else if (rightS)
                rotation = 270;
            else if (upS)
                rotation = 0;
            else
                rotation = 180;

            return rotation;
        }
    }

    private GameObject GetWallGameObject (WallDirection wallDirection)
    {
        GameObject wallTile = wallTileDefault;

        switch (wallDirection.type)
        {
            case WallDirection.WallType.undefined:
                wallTile = wallTileDefault;
                break;
            case WallDirection.WallType.wallSimple:
                wallTile = wallTileSimple;
                break;
            case WallDirection.WallType.wall2Paraller:
                wallTile = wallTile2Paraller;
                break;
            case WallDirection.WallType.wall2Corner:
                wallTile = wallTile2Corner;
                break;
            case WallDirection.WallType.wall3Sides:
                wallTile = wallTile3Sides;
                break;
            default:
                break;
        }

        return wallTile;
    }

    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        var start = _grid[from.x, from.y] as IWalkable;
        var target = _grid[to.x, to.y] as IWalkable;
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
                if (!neighbour.walkthrough)
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
                int gridSizeX = _grid.GetLength(0);
                int gridSizeY = _grid.GetLength(1);
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if (_grid[checkX, checkY] is IWalkable g) // && !(grid[checkX, checkY] is DebrisTile))
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
        _grid[col, row] = new Tile(generateTilePrefab(emptyTile, new Vector2Int(col, row)), col, row) {inside = true};
        UpdateFog();
    }

    [CanBeNull]
    public Building BuildingAt(Vector2Int coord)
    {
        if (coord.x > columns || coord.x < 0) return null;
        if (coord.y > rows || coord.y < 0) return null;
        if (_grid[coord.x, coord.y] is Tile t)
        {
            return t.building;
        }

        return null;
    }

    public bool Buildable(Vector2Int coord)
    {
        bool ret = false;
        Tile highlight = null;
        if (_grid[coord.x, coord.y] is Tile tile && !(_grid[coord.x, coord.y] is DebrisTile))
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
            if (_grid[coord.x, coord.y] is Tile t)
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
                    if (locX >= _grid.GetLength(0) || locY >= _grid.GetLength(1)) continue;

                    if (Math.Abs(x) < distance && Math.Abs(y) < distance)
                        continue;
                    if (_grid[startPoint.x + x, startPoint.y + y] is Tile t)
                    {
                        if (t.building == null && t.inside)
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

                            if (!allowResources)
                            {
                                if (t is IResourceHolder resHolder)
                                {
                                    if (!resHolder.Amount.Empty())
                                    {
                                        empty = false;
                                    }
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

    public class WallDirection
    {
        public WallType type = WallType.wallSimple;
        public int rotation = 0;

        public enum WallType
        {
            undefined,
            wallSimple,
            wall2Paraller,
            wall2Corner,
            wall3Sides
        }
    }

}