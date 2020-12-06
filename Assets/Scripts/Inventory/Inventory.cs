using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    [Space]
    [Header("Managers")] [SerializeField] InventoryManager inventoryManager;
    // this will have all item . Who has actualy the player.. 

    // provide Data for inventory window.. Which is uWindowSpecialistController

    [SerializeField] uWindowSpecController uWindowSpecController;

    List<Item> collectedItems = new List<Item>();

    private void Start()
    {
        collectedItems = inventoryManager.returnThemAll();

        foreach (var item in collectedItems)
        {
            uWindowSpecController.AddItem(item);
        }
        
    }

}
