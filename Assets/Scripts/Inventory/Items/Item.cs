using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using static ItemBlueprint;
using System;

[Serializable]
public class Item : MonoBehaviour
{
    [SerializeField] private Image image;
    [Space]
    [SerializeField] private bool isStackAble;

    private string name;
    private Text amountInStackText;
    private ItemType type;
    private ItemResourceType resourceType;
    private Slot mySlot;
    private BonusModificators[] modificators;
    private int sizeStock = 1;

    private DragAndDropHandler _dragAndDropHandler;

    public string Name { get { return this.name; } set { name = value; } }

    public DragAndDropHandler GetDragAndDropHandler { get { return this._dragAndDropHandler; } }

    public Slot MySlot { get { return this.mySlot; } set { mySlot = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

    public ItemType Type { get { return this.type; } }

    public ItemResourceType ResourceType { get { return this.resourceType; } }

    public BonusModificators[] Modificators { get { return this.modificators; } set { this.modificators = value; } }

    public bool IsStackAble { get{ return isStackAble; } }

    public int GetStackAmount { get { return this.sizeStock; } }

    public void SetupItem(string _name, ItemType _type, Sprite _sprite, ItemResourceType _resourceType = ItemResourceType.None )
    {
        name = _name;
        image.sprite = _sprite;
        type = _type;
        resourceType = _resourceType;

        if (type == ItemType.ResBasic)
        {
            isStackAble = true;
            amountInStackText.text = sizeStock.ToString();
        }
        else
        {
            if (amountInStackText != null)
            {
                amountInStackText.gameObject.SetActive(false);
            }
        }
    }

    public void SetupItem(ItemBlueprint bluePrint)
    {
        name = bluePrint.name;
        image.sprite = bluePrint.Sprite;
        type = bluePrint.Type;
        resourceType = bluePrint.ResourceType;
        sizeStock = bluePrint.sizeStock;

        if(type == ItemType.ResBasic)
        {
            isStackAble = true;
            amountInStackText.text = sizeStock.ToString();
        }
        else
        {
            if (amountInStackText != null)
            {
                amountInStackText.gameObject.SetActive(false);
            }
        }
    }

    public void SetupItem(Item item)
    {
        name = item.Name;
        image.sprite = item.Sprite;
        type = item.Type;
        resourceType = item.ResourceType;
        sizeStock = item.sizeStock;

        if (type == ItemType.ResBasic)
        {
            isStackAble = true;
            amountInStackText.text = sizeStock.ToString();
        }
        else
        {
            if (amountInStackText != null)
            {
                amountInStackText.gameObject.SetActive(false);
            }
        }
    }

    public void AddAmountToStack(int amount)
    {
        sizeStock += amount;
        amountInStackText.text = $"{sizeStock}";
    }

    private void Awake()
    {
        image = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        amountInStackText = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        _dragAndDropHandler = this.GetComponent<DragAndDropHandler>();
    }

}
