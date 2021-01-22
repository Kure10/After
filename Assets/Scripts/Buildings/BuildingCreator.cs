using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class BuildingCreator : MonoBehaviour
{
    private BuildingBlueprint selectedBuildingBlueprint = null;
    private GameObject blueprint = null;
    private GameObject test = null;
    private readonly int TILE = 1 << 8;
    private float scroll;
    private int direction;
    private int rotation;
    private TileFactory tileFactory;
    public Color allowed;
    public Color forbidden;

    [SerializeField] PanelTime time;

    private List<IWorkSource> buildings;
    // Start is called before the first frame update
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();
        buildings = new List<IWorkSource>();
    }

    // Update is called once per frame
    void Update()
    {
        buildings?.ForEach(b=>b.Update());
        

        if (blueprint != null)
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
                blueprint.transform.Rotate(new Vector3(0, 90 * direction));
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, TILE))
            {
                Vector3 point = hit.point;
                var coord = Geometry.GridFromPoint(point);

                blueprint.transform.position = Geometry.PointFromGrid(coord);
                var buildingGrid = getGridForBuilding(coord);
                bool canBuild = true;
                foreach (var item in buildingGrid)
                {
                    if (tileFactory.Buildable(item) == false)
                    {
                        canBuild = false;
                        Debug.Log($"Failed on: {item.x}, {item.y}");
                        break;
                    }
                }

                blueprint.transform.Find("Build_setup").GetComponent<Renderer>()
                    .material.color = canBuild ? allowed : forbidden;
                  
                if (Input.GetMouseButtonDown(0) && canBuild)
                {
                    float upDiff = 0.04f;
                    
                    blueprint.transform.position += new Vector3(0, upDiff, 0); 
                    //TODO create proper Factory for this
                    Building newBuild;
                    if (selectedBuildingBlueprint.Name == "Skladiště")
                    {
                        newBuild = new Warehouse(selectedBuildingBlueprint, blueprint );
                    }
                    else
                    {
                        newBuild = new Building(selectedBuildingBlueprint, blueprint );
                        
                    }

                        
                    buildings.Add(newBuild);
                    tileFactory.AddBuilding(buildingGrid, newBuild);
                    EndBuildingMode();
                }
            }
        }
    }

    public void CreateBuilding(BuildingBlueprint buildingBlueprint)
    {
        CameraMovement.ZoomByScrollEnabled(false);
        selectedBuildingBlueprint = buildingBlueprint;
        scroll = 0f;
        rotation = 0;
        blueprint = Instantiate(buildingBlueprint.Prefab);
        
    }
    private void EndBuildingMode()
    {
        CameraMovement.ZoomByScrollEnabled(true);
        Destroy(blueprint);
        selectedBuildingBlueprint = null;
        time.UnpauseGame(fromPopup: true);
    }
    private List<Vector2Int> getGridForBuilding(Vector2Int coord)
    {
        int xx, yy;
        List<Vector2Int> grid = new List<Vector2Int>();
        for (int x = 0; x < selectedBuildingBlueprint.row; x++)
        {
            for (int y = 0; y < selectedBuildingBlueprint.column; y++)
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
                    case 0: xx = coord.x + x; yy = coord.y + y; break;
                    case 1: xx = coord.x + y; yy = coord.y - x; break;
                    case 2: xx = coord.x - x; yy = coord.y - y; break;
                    default: xx = coord.x - y; yy = coord.y + x; break;
                }
                grid.Add(new Vector2Int(xx, yy));
            }
        }
        return grid;
    }
}
