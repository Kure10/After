using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] ContainerManager containerManager;
    // Will have ALL items in Game..

    [SerializeField] Inventory inventory;

    public List<ItemBlueprint> allItemsAndResource = new List<ItemBlueprint>();

    private void Awake()
    {
      //  Container startContainer = containerManager.GetStartContainer();

        ItemXmlLoader xmlLoader = new ItemXmlLoader();
        allItemsAndResource = xmlLoader.GetItemsFromXML();
        //  var startItems = FindStartingItem(startContainer);

        inventory.InicializedStartInventory(allItemsAndResource);
    }

    public List<ItemBlueprint> FindStartingItem (Container container)
    {
        List<ItemBlueprint> startingItems = new List<ItemBlueprint>();

        foreach (SubContainerData subData in container.containerSubData)
        {
            foreach (ItemBlueprint item in allItemsAndResource)
            {
                if(subData.dropItemID == item.itemID)
                {
                    startingItems.Add(item);
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
