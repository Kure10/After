using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    [Space]
    [Header("Managers")] [SerializeField] InventoryManager inventoryManager;

    [SerializeField] public GameObject itemPrefab;

    // this will have all item . Who has actualy the player.. 

    // provide Data for inventory window.. Which is uWindowSpecialistController

    public List<ItemBlueprint> collectedItems = new List<ItemBlueprint>();

    private void Start()
    {
        // return all data for item -> ToDo finish
        collectedItems = inventoryManager.returnThemAll();

    }

}
