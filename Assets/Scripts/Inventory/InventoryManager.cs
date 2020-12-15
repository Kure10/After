using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    
    // Will have ALL items in Game..

    [SerializeField] Inventory inventory;

    public List<ItemBlueprint> allItems = new List<ItemBlueprint>();

    private void Awake()
    {
        ItemXmlLoader xmlLoader = new ItemXmlLoader();
        allItems = xmlLoader.GetItemsFromXML();

        // will provide only items which will be at start... For not ALL !!
        inventory.InicializedStartInventory(allItems);
    }

}
