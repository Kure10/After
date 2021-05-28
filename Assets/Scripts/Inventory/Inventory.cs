using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    [Space]
    [SerializeField] uWindowSpecController _uWindowSpecController;

    [SerializeField] ItemCreater _itemCreator;

    [Header("Prefabs")]
    [SerializeField] public GameObject _itemPrefab;
    [SerializeField] private GameObject _itemSlot;

    [Space]
    public List<ItemSlot> inventory = new List<ItemSlot>();

    private int _baseInventorySize = 15;
    private int _additionalInventorySize = 70;

    
    public GameObject GetItemPrefab { get { return _itemPrefab; }}

    public static event Action<Mission> OnInventoryChange = delegate { };

    public ItemSlot FindEmptySlot ()
    {
        ItemSlot emptySlot = new ItemSlot();

        foreach (ItemSlot item in inventory)
        {
            if(item.IsEmpty)
            {
                emptySlot = item;
                break;
            }
        }

        return emptySlot;
    }


    public void InicializedStartInventory(List<ItemBlueprint> loot)
    {
        int size = _baseInventorySize + _additionalInventorySize;
        int count = loot.Count;

        // init Slots
        for (int i = 0; i < size; i++)
        {
            // create and set slot
            GameObject slot = Instantiate(_itemSlot);
            slot.transform.SetParent(_uWindowSpecController.GetSlotHolder);
            slot.transform.localScale = new Vector3(1f, 1f, 1f);
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();

            // create and set item
            // if Nomore items t
            if (i < count)
            {
                ItemBlueprint blueprint = loot[i];

                GameObject game = _itemCreator.CreateItemByType(loot[i], _itemPrefab);
                var item = game.GetComponent<Item>();

                if (item == null) // Todo Res and None type.. 
                {
                    Debug.LogWarning("somewhere is mistake Error in Inventory");
                    item = game.AddComponent<Item>();
                    item.SetupItem(blueprint.name, blueprint.Type, blueprint.Sprite);
                }

                item.MySlot = itemSlot;

                game.GetComponent<DragAndDropHandler>().InitDragHandler();

                itemSlot.SetSlot(i, game, item);
            }
            else
                itemSlot.SetSlot(i, null, null);
            

            inventory.Add(itemSlot);
        }
    }
}
