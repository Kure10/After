using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    [Space]
    [SerializeField] uWindowSpecController _uWindowSpecController;

    [Header("Prefabs")]
    [SerializeField] public GameObject _itemPrefab;
    [SerializeField] private GameObject _itemSlot;

    [Space]
    //  public List<(Item item, ItemSlot slot)> inventory = new List<(Item item, ItemSlot slot)>();
    public List<ItemSlot> inventory = new List<ItemSlot>();

    private int _baseInventorySize = 15;
    private int _additionalInventorySize = 70;

    //  public Dictionary<Item, ItemSlot > _inventory = new Dictionary<Item, ItemSlot >();

    // provide Data for inventory window.. Which is uWindowSpecialistController

    public static event Action<Mission> OnInventoryChange = delegate { };

    private void Start()
    {

    }

    public void InicializedStartInventory(List<ItemBlueprint> blueprits)
    {
        int size = _baseInventorySize + _additionalInventorySize;
        int count = blueprits.Count;

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
                ItemBlueprint blueprint = blueprits[i];
                GameObject gameObject = Instantiate(_itemPrefab);
                Item item = gameObject.GetComponent<Item>();
                item.SetupItem(blueprint.Name, blueprint.Type, blueprint.Sprite);
                item.MySlot = itemSlot;
               
                itemSlot.SetSlot(i, gameObject, item);
            }
            else
                itemSlot.SetSlot(i, null, null);
            

            inventory.Add(itemSlot);
        }
    }

    //public void Swap((Item item, ItemSlot slot) first , (Item item, ItemSlot slot) secpmd)
    //{
    //    var tmp = first.item;
    //    first.item = secpmd.item;
    //    secpmd.item = tmp;
    //}
}
