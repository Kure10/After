using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    private TileFactory tileFactory;
    private List<GameObject> SelectedObjects;
    private int layerMask;
    private float maxDist = 100f;
    // Start is called before the first frame update
    void Start()
    {
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        SelectedObjects = new List<GameObject>();
        layerMask = 1 << 8; //hit only layer 8 (tiles)
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        foreach (var prevSelection in SelectedObjects)
        {
            prevSelection.SetActive(false);
        }
        SelectedObjects.Clear();
        if (Physics.Raycast(ray, out hit, maxDist, layerMask))
        {
            Vector3 point = hit.point;
            var coord = Geometry.GridFromPoint(point);
            var tile = tileFactory.getTile(coord);
            var selectedObject = tile.gameObject.transform.Find("Selection").gameObject;
            if (selectedObject != null)
            {
                selectedObject.SetActive(true);
                SelectedObjects.Add(selectedObject);
            }
        }
    }
}
