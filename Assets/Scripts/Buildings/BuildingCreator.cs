using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCreator : MonoBehaviour
{
    private GameObject building = null;
    private readonly int TILE = 1 << 8;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (building != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                Destroy(building);
                building = null;
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, TILE))
            {
                Vector3 point = hit.point;
                var coord = Geometry.GridFromPoint(point);
                building.transform.position = point;
            }
        }
    }

    public void CreateBuilding(GameObject prefab)
    {
        if (building != null)
        {
            Destroy(building);
        }

        building = Instantiate(prefab);
    }
}
