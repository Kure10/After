using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    private TileFactory tileFactory;
    private List<GameObject> selectedObjects;
    private List<GameObject> highlightedObjects;
    private int layerMask;
    private float maxDist = 100f;
    private readonly int TILE = 1 << 8;
    private readonly int SELECTABLE = 1 << 9;
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        selectedObjects = new List<GameObject>();
        highlightedObjects = new List<GameObject>();
        layerMask = SELECTABLE; //hit only layer 9 (selectables)
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClearHighlight(selectedObjects);
            ClearHighlight(highlightedObjects);
            layerMask = SELECTABLE;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDist, layerMask))
            {
                var character = hit.transform.gameObject;
                var highlight = character.transform.Find("Selection");
                if (highlight != null)
                {
                    selectedObjects.Add(character);
                    highlight.gameObject.SetActive(true);
                }

            }
        }
        if (selectedObjects.Count > 0)
        {
            layerMask = TILE;
            ClearHighlight(highlightedObjects);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDist, layerMask))
            {
                Vector3 point = hit.point;
                var coord = Geometry.GridFromPoint(point);
              //  Debug.LogWarning($"Coord[x,y]: [{coord.x},{coord.y}]");
                var tile = tileFactory.getTile(coord);
              //  Debug.LogWarning($"Tile[x,y]: [{tile.x},{tile.y}]");
                var highlightedObject = tile.tile.gameObject.transform.Find("Selection");
                if (highlightedObject != null)
                {
                    highlightedObject.gameObject.SetActive(true);
                    highlightedObjects.Add(tile.tile);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    var targetTile = tileFactory.getTile(coord);

                    foreach(var character in selectedObjects)
                    {
                        var path = tileFactory.FindPath(Geometry.GridFromPoint(character.transform.position), coord);
                        if (path != null)
                        {
                            //Move to target and if the targe tile has some default action, add it to stack of actions
                            //Debris is unwalkable, but for the purpose of cleaning, you can enter at first field
                            path.Add(coord);
                            Move(character, path);
                            //TODO actions
                        }
                    }
                    ClearHighlight(highlightedObjects);
                    ClearHighlight(selectedObjects);

                }
            }

        }
    }
    private void ClearHighlight(List<GameObject> objects)
    {
        foreach (var prevSelection in objects)
        {
            prevSelection.transform.Find("Selection").gameObject.SetActive(false);
        }
        objects.Clear();
    }
    private void Move(GameObject character, List<Vector2Int> path)
    {
        character.GetComponent<Character>().Move(path);
    }
}
