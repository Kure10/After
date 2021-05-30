using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class uWindowSpecController : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private GameObject panelSpecialist;

    [Header("Holder")]
    [SerializeField] private RectTransform specHolder;
    [SerializeField] private Transform slotHolder;

    public Transform GetSlotHolder { get { return slotHolder; } }

    private List<uWindowSpecialist> specInGame = new List<uWindowSpecialist>();

    private int lastSortCategory = -1;

    private SpecialistControler specController;

    private void OnEnable()
    {
        foreach (var item in specInGame)
        {
            item.RefreshCharacterInfo();
        }
    }

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(specHolder);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            foreach (uWindowSpecialist item in specInGame)
            {
                List<SpecInventorySlot> backpack = item.GetCharacterBackpackSlots();
                foreach (SpecInventorySlot back in backpack)
                {
                    if(back.CurrentItem != (null,null))
                        Debug.Log(back.CurrentItem.item.Name +  "   item name " + " name spec-> " + item.GetName); 

                }
            }

        }
    }

    public void AddSpecHolder(Character character)
    {
        GameObject ga = Instantiate(panelSpecialist);
        ga.transform.SetParent(specHolder);
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        uWindowSpecialist uWindowSpec = ga.GetComponent<uWindowSpecialist>();
        uWindowSpec.SetAll(character);

        specInGame.Add(uWindowSpec);

        var slots = uWindowSpec.GetCharacterSlots();
        character.SetCharacterSlots = slots;

        var backpackSlots = uWindowSpec.GetCharacterBackpackSlots();
        character.SetCharacterBackPackSlots = backpackSlots;

        // todo onitem change  pro backpack

        foreach (SpecInventorySlot slot in slots)
        {
            //slot.OnItemChangeCallBack += character.OnItemChange;
            DragAndDropManager.Instantion.OnItemResponseReaction += OnItemDragResponce;

            // Todo..
            if (slot.GetFirstSlotType == ItemBlueprint.ItemType.BagSpec || slot.GetSecondSlotType == ItemBlueprint.ItemType.BagSpec)
            {
                slot.OnOpenBackPack += uWindowSpec.OpenBackpackInventory;
                slot.OnOpenBackPack += RebuildLayout;
                slot.OnCloseBackPack += uWindowSpec.CloseBackpackInventory;
                slot.OnCloseBackPack += RebuildLayout;
            }

        }
    }

    // karma = 0
    // abecedne = 1
    // level = 2
    // health = 3
    // From Editor
    public void Sort(int currentSortCategory)
    {
        switch (currentSortCategory)
        {
            case 0:
                specInGame = specInGame.OrderBy(x => x.GetKarma).ToList();
                break;
            case 1:
                specInGame = specInGame.OrderBy(x => x.GetName).ToList();
                break;
            case 2:
                specInGame = specInGame.OrderBy(x => x.GetLevel).ToList();
                break;
            case 3:
                specInGame = specInGame.OrderBy(x => x.GetPercentHelth).ToList();
                break;
            default:
                Debug.Log("uWindowSpecControllerr Chyba. in order..");
                break;
        }

        if (lastSortCategory != currentSortCategory)
        {
            foreach (var item in specInGame)
            {
                item.transform.SetAsFirstSibling();
                lastSortCategory = currentSortCategory;
            }
        }
        else
        {
            foreach (var item in specInGame)
            {
                item.transform.SetAsLastSibling();
                lastSortCategory = -1;
            }
        }
    }

    public void OnItemDragResponce(Item dragingItem)
    {
        if (DragAndDropManager.IsDraging)
        {
            foreach (var character in specInGame)
            {
                List<SpecInventorySlot> slots = character.GetCharacterSlots();
                foreach (SpecInventorySlot slot in slots)
                {
                    if (dragingItem.Type == slot.GetFirstSlotType || dragingItem.Type == slot.GetSecondSlotType)
                    {
                        slot.ShowDragPosibility();
                    }
                }
            }
        }
        else
        {
            foreach (var character in specInGame)
            {
                List<SpecInventorySlot> slots = character.GetCharacterSlots();
                foreach (SpecInventorySlot slot in slots)
                {
                    slot.HideDragPosibility();
                }
            }
        }

    }

    public void RebuildLayout(int i)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(specHolder);
    }

    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(specHolder);
    }

    public void RefreshGrid()
    {
        //specHolder  specInGame
        int sortBy = 1;
        // lastSortCategory
        if (lastSortCategory > -1)
            sortBy = lastSortCategory;

        Sort(sortBy);
       

        LayoutRebuilder.ForceRebuildLayoutImmediate(specHolder);
    }

}
