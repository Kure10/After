using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour, IWorkSource
{
    private TileFactory tileFactory;
    private List<GameObject> selectedObjects;
    private List<GameObject> highlightedObjects;
    private ResourceManager resourceManager;
    private int layerMask;
    private float maxDist = 100f;
    private readonly int TILE = 1 << 8;
    private readonly int SELECTABLE = 1 << 9;
    private List<Character> characters;
    private GameObject panelUI;
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        selectedObjects = new List<GameObject>();
        highlightedObjects = new List<GameObject>();
        layerMask = SELECTABLE; //hit only layer 9 (selectables)
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        characters = new List<Character>();
        panelUI = GameObject.FindGameObjectWithTag("SpecialistUI");
        panelUI.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
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
                    var blueprint = character.GetComponent<Character>();
                    highlight.gameObject.GetComponent<Renderer>().material.SetColor("_Color", blueprint.GetColor());
                    highlight.gameObject.SetActive(true);
                    panelUI.GetComponent<uWindowSpecialist>().SetAll(blueprint.GetBlueprint());
                    panelUI.SetActive(true);

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
                Debug.LogWarning($"Coord[x,y]: [{coord.x},{coord.y}]");
                var tile = tileFactory.getTile(coord);
                Debug.LogWarning($"Tile[x,y]: [{tile.x},{tile.y}]");
                var highlightedObject = tile.tile.gameObject.transform.Find("Selection");
                if (highlightedObject != null)
                {
                    highlightedObject.gameObject.SetActive(true);
                    highlightedObjects.Add(tile.tile);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    var targetTile = tileFactory.getTile(coord);
                    if (targetTile is Tile t)
                    {
                        if (t.building != null)
                        {
                            //TODO map to building class to get the state of building (if it's under construction,
                            //what materials are missing...)
                            if (selectedObjects[0].TryGetComponent(out Character character))
                            {
                                character.Register(t.building);
                                
                            }
                            ClearHighlight(highlightedObjects);
                            ClearHighlight(selectedObjects);
                            return;
                        }

                        if (t is DebrisTile debris)
                        {
                            if (selectedObjects[0].TryGetComponent(out Character character))
                            {
                                character.Register(debris);
                            }
                            ClearHighlight(highlightedObjects);
                            ClearHighlight(selectedObjects);
                            return;
                        }
                    }
                    foreach(var character in selectedObjects)
                    {
                        var path = tileFactory.FindPath(Geometry.GridFromPoint(character.transform.position), coord);
                        if (path != null)
                        {
                            //Move to target and if the target tile has some default action, add it to stack of actions
                            //Debris is unwalkable, but for the purpose of cleaning, you can enter at first field
                            if (character.TryGetComponent(out Character person))
                            {
                                person.Register(this);
                                person.AddCommand(new Move(character, path));
                            }
                        }
                    }
                    ClearHighlight(highlightedObjects);
                    ClearHighlight(selectedObjects);

                }
            }
            
        }

        foreach (var character in characters.ToList())
        {
            if (character.Execute() == Result.Success)
            {
                Unregister(character);
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

    public bool Register(Character who)
    {
        characters.Add(who);
        return true;
    }

    public void Unregister(Character who)
    {
        characters.Remove(who);
    }
}
