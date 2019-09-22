using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCreator : MonoBehaviour
{
    private GameObject building = null;
    private readonly int TILE = 1 << 8;
    private float scroll;
    private int direction;
    private int rotation;
    private TileFactory tileFactory;
    private int column = 2; //hardcoded for now
    private int row = 3; //TODO: create new class for buildings with column and row properties
    // Start is called before the first frame update
    public GameObject testBuilding;
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (building == null)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
               CreateBuilding(testBuilding);
            }

        }
        else 
        {
 
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                EndBuildingMode();
                return;
            }
            scroll += Input.mouseScrollDelta.y;

            if (Mathf.Abs(scroll) >= 0.1f)
            {
                direction = scroll > 0 ? 1 : -1;
                rotation += direction;
                rotation = (4 + rotation) % 4;
                scroll = 0;
                building.transform.Rotate(new Vector3(0, 90 * direction));
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, TILE))
            {
                Vector3 point = hit.point;
                var coord = Geometry.GridFromPoint(point);

                building.transform.position = Geometry.PointFromGrid(coord);
                var buildingGrid = getGridForBuilding(coord);
                bool canBuild = true;
                foreach (var item in buildingGrid)
                {
                    if (tileFactory.Buildable(item) == false)
                    {
                        canBuild = false;
                        break;
                    }
                }
                  
                if (Input.GetMouseButtonDown(0) && canBuild)
                {
                    var newBuild = Instantiate(building, building.transform.position, building.transform.rotation); 
                    tileFactory.AddBuilding(buildingGrid, newBuild);
                    EndBuildingMode();
                }

            }
        }
    }

    public void CreateBuilding(GameObject prefab)
    {
        CameraMovement.ZoomByScrollEnabled(false);
        if (building != null)
        {
            Destroy(building);
        }
        scroll = 0f;
        rotation = 0;
        building = Instantiate(prefab);
        
    }
    private void EndBuildingMode()
    {
        CameraMovement.ZoomByScrollEnabled(true);
        Destroy(building);
        building = null;
    }
    private List<Vector2Int> getGridForBuilding(Vector2Int coord)
    {
        int xx, yy;
        List<Vector2Int> grid = new List<Vector2Int>();
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                // example for row = 3, column = 2
                // rotation == 0 
                // x x
                // x x
                // x X 
                // rotation == 1
                // x x x
                // X x x
                // rotation == 2
                // X x
                // x x
                // x x
                // rotation == 3
                // x x X
                // x x x

                switch (rotation)
                {
                    case 0: xx = coord.x - x; yy = coord.y + y; break;
                    case 1: xx = coord.x + y; yy = coord.y + x; break;
                    case 2: xx = coord.x + x; yy = coord.y - y; break;
                    default: xx = coord.x - y; yy = coord.y - x; break;
                }
                grid.Add(new Vector2Int(xx, yy));
            }
        }
        return grid;
    }
}
