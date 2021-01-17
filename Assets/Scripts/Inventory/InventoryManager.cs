using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] ContainerManager containerManager;
    // Will have ALL items in Game..

    [SerializeField] Inventory inventory;

    public List<ItemBlueprint> allItems = new List<ItemBlueprint>();

    private void Awake()
    {
        Container startContainer = containerManager.GetStartContainer();

        ItemXmlLoader xmlLoader = new ItemXmlLoader();
        allItems = xmlLoader.GetItemsFromXML();
      //  var startItems = FindStartingItem(startContainer);

       

        inventory.InicializedStartInventory(allItems);
    }

    public List<ItemBlueprint> FindStartingItem (Container container)
    {
        List<ItemBlueprint> startingItems = new List<ItemBlueprint>();

        foreach (SubContainerData subData in container.containerSubData)
        {
            foreach (ItemBlueprint item in allItems)
            {
                if(subData.dropItemID == item.itemID)
                {
                    startingItems.Add(item);
                }
            }
        }
        return startingItems;
    }
}
