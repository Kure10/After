using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class BuildingCreator : MonoBehaviour
{
    private BuildingBlueprint selectedBuildingBlueprint = null;
    private GameObject blueprint = null;
    private Renderer blueprintOverlayColor;
    private GameObject test = null;
    private readonly int TILE = 1 << 8;
    private float scroll;
    private int direction;
    private int rotation;
    private TileFactory tileFactory;
    private Color allowed;
    public Color forbidden;
    public float overlayAlpha = 0.4f;

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
        buildings?.ForEach(b => b.Update());


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

                var selectedTag = selectedBuildingBlueprint.Tag;
                //Extensions can be build only next to existing one or base one
                if (selectedBuildingBlueprint.Type == TypeOfBuilding.Extension && canBuild)
                {
                    canBuild = false;
                    var vectors = new List<Vector2Int>()
                        {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
                    foreach (var item in buildingGrid)
                    {
                        foreach (var position in vectors)
                        {
                            var building = tileFactory.BuildingAt(item + position);
                            if (building == null) continue;
                            if (building.blueprint.Tag == selectedTag)
                            {
                                if (building.State == Building.BuildingState.Build)
                                {
                                    canBuild = true;
                                    break;
                                }
                            }
                        }

                        if (canBuild) break;
                    }
                }
                if (selectedBuildingBlueprint.Type == TypeOfBuilding.Upgrade)
                {
                    canBuild = true;
                    foreach (var item in buildingGrid)
                    {
                        var building = tileFactory.BuildingAt(item);
                        if (building == null)
                        {
                            canBuild = false;
                            break;
                        }

                        if (building.blueprint.Tag != selectedTag ||
                            building.blueprint.Type != TypeOfBuilding.Extension)
                        {
                            canBuild = false;
                            break;
                        }

                        if (building.State != Building.BuildingState.Build)
                        {
                            canBuild = false;
                            break;
                        }
                    }
                }

                blueprintOverlayColor.material.color = canBuild ? allowed : forbidden;

                if (Input.GetMouseButtonDown(0) && canBuild)
                {
                    float upDiff = 0.04f;

                    blueprint.transform.position += new Vector3(0, upDiff, 0);
                    //TODO create proper Factory for this
                    Building newBuild;
                    if (selectedBuildingBlueprint.Name == "Skladiště")
                    {
                        newBuild = new Warehouse(selectedBuildingBlueprint, blueprint);
                    }
                    else
                    {
                        newBuild = new Building(selectedBuildingBlueprint, blueprint);
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
        if (blueprint != null)
        {
            Object.Destroy(blueprint);
        }

        selectedBuildingBlueprint = buildingBlueprint;
        scroll = 0f;
        rotation = 0;
        if (buildingBlueprint.Type == TypeOfBuilding.Upgrade)
        {
            Debug.Log($"Upgrade");
        }

        blueprint = Instantiate(buildingBlueprint.Prefab);
        blueprintOverlayColor = blueprint.transform.Find("Build_setup").GetComponent<Renderer>();
        var c = selectedBuildingBlueprint.BackgroundColor;
        allowed = new Color(c.r, c.g, c.b, overlayAlpha);
    }

    private void EndBuildingMode()
    {
        CameraMovement.ZoomByScrollEnabled(true);
        blueprintOverlayColor = null;
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
                // x x x
                // X x x
                // rotation == 1
                // X x 
                // x x
                // x x
                // rotation == 2
                // x x X
                // x x x
                // rotation == 3
                // x x 
                // x x
                // x X

                switch (rotation)
                {
                    case 0:
                        xx = coord.x + x;
                        yy = coord.y + y;
                        break;
                    case 1:
                        xx = coord.x + y;
                        yy = coord.y - x;
                        break;
                    case 2:
                        xx = coord.x - x;
                        yy = coord.y - y;
                        break;
                    default:
                        xx = coord.x - y;
                        yy = coord.y + x;
                        break;
                }

                grid.Add(new Vector2Int(xx, yy));
            }
        }

        return grid;
    }
}