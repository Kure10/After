using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemCreating;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] ContainerManager containerManager;
    // Will have ALL items in Game..

    [SerializeField] Inventory inventory;

    public List<ItemBlueprint> allItemsAndResource = new List<ItemBlueprint>();

    private void Awake()
    {
        //Container startContainer = containerManager.GetStartContainer();

        ItemXmlLoader xmlLoader = new ItemXmlLoader();
        allItemsAndResource = xmlLoader.GetItemsFromXML();
        //  var startItems = FindStartingItem(startContainer);

        ItemCreater itemCreater = new ItemCreater();

        if (inventory != null)
        {
            inventory.InicializedStartInventory(allItemsAndResource);
            inventory.InicializedStartInventory(allItemsAndResource);
        }
    }

    public List<ItemBlueprint> FindStartingItem (Container container)
    {
        List<ItemBlueprint> startingItems = new List<ItemBlueprint>();

        foreach (SubContainerData subData in container.containerSubData)
        {
            foreach (ItemBlueprint itemBlueprint in allItemsAndResource)
            {
                if(subData.dropItemID == itemBlueprint.itemID)
                {
                    startingItems.Add(itemBlueprint);
                }
            }
        }
        return startingItems;
    }

    public ItemBlueprint GetResourcesByID (long id)
    {
        var result = allItemsAndResource.Find(x => x.itemID == id);

        return result;
    }
}
