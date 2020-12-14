﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // will be responsible. for load Item from XML

    // Will have ALL items in Game..

    // will provide curent item if will be asked..

    public List<ItemBlueprint> allItems = new List<ItemBlueprint>();

    private void Awake()
    {
        ItemXmlLoader xmlLoader = new ItemXmlLoader();
        allItems = xmlLoader.GetItemsFromXML();
    }

    // ToDo tmp method for testing..
    public List<ItemBlueprint> returnThemAll ()
    {
        return allItems;
    }
}