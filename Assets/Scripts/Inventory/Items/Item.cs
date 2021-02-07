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
    private string name;
    private ItemType type;
    private Slot mySlot;
    private BonusModificators[] modificators;

    private DragAndDropHandler _dragAndDropHandler;

    public string Name { get { return this.name; } set { name = value; } }

    public DragAndDropHandler GetDragAndDropHandler { get { return this._dragAndDropHandler; } }

    public Slot MySlot { get { return this.mySlot; } set { mySlot = value; } }

    public Sprite Sprite { get { return this.image.sprite; } set { image.sprite = value; } }

    public ItemType Type { get { return this.type; } }

    public BonusModificators[] Modificators { get { return this.modificators; } set { this.modificators = value; } }

    public void SetupItem(string _name, ItemType _type, Sprite _sprite)
    {
        name = _name;
        image.sprite = _sprite;
        type = _type;
    }

    private void Awake()
    {
        image = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        _dragAndDropHandler = this.GetComponent<DragAndDropHandler>();
    }

}
