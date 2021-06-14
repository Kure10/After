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
    [SerializeField] GameObject itemPrefab;

    [Header("Holder")]
    [SerializeField] private RectTransform specHolder;
    [SerializeField] private Transform slotHolder;

    public Transform GetSlotHolder { get { return slotHolder; } }

    private List<uWindowSpecialist> specInGame = new List<uWindowSpecialist>();

    private int lastSortCategory = -1;

    private bool _simpleSort = false;

    public List<uWindowSpecialist> GetSpecInGameWindows { get { return this.specInGame; } }

    private void OnEnable()
    {
        foreach (uWindowSpecialist uWindowSpec in specInGame)
        {
            uWindowSpec.RefreshCharacterInfo();
        }

        StartCoroutine("Refresh");
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
                    if (back.CurrentItem != (null, null))
                        Debug.Log(back.CurrentItem.item.Name + "   item name " + " name spec-> " + item.GetName);
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

        List<SpecInventorySlot> charSlots = new List<SpecInventorySlot>();
        List<SpecInventorySlot> backPackSlots = new List<SpecInventorySlot>();
        if (character.CharacterSlots != null)
        {
            foreach (var slot in character.CharacterSlots)
            {
                charSlots.Add(slot);
            }
        }

        if (character.CharacterBackPackSlots != null)
        {
            foreach (var slot in character.CharacterBackPackSlots)
            {
                backPackSlots.Add(slot);
            }
        }

        var slots = uWindowSpec.GetCharacterSlots();
        character.CharacterSlots = slots;

        var backpackSlots = uWindowSpec.GetCharacterBackpackSlots();
        character.CharacterBackPackSlots = backpackSlots;

        for (int i = 0; i < charSlots.Count; i++)
        {
            SpecInventorySlot firstSlot = charSlots[i];
            SpecInventorySlot charSlot = character.CharacterSlots[i];

            charSlot.CurrentItem = firstSlot.CurrentItem;
        }

        for (int i = 0; i < backPackSlots.Count; i++)
        {
            SpecInventorySlot firstSlot = backPackSlots[i];
            SpecInventorySlot charSlot = character.CharacterBackPackSlots[i];

            charSlot.CurrentItem = firstSlot.CurrentItem;
        }

        // inventory
        uWindowSpec.PopulateItemSlots(character, false);
        uWindowSpec.PopulateBackpackItemSlots(character, false);

        // todo onitem change  pro backpack

        foreach (SpecInventorySlot slot in slots)
        {
            //slot.OnItemChangeCallBack += character.OnItemChange;
            DragAndDropManager.Instantion.OnItemResponseReaction += OnItemDragResponce;

            if (slot.HasSlotThatType(ItemBlueprint.ItemType.BagSpec))
            {
                slot.OnOpenBackPack += uWindowSpec.OpenBackpackInventory;
                slot.OnOpenBackPack += RebuildLayout;
                slot.OnCloseBackPack += uWindowSpec.CloseBackpackInventory;
                slot.OnCloseBackPack += RebuildLayout;
            }
        }
    }

    public void RemoveInGameCharacterUI()
    {
        foreach (uWindowSpecialist window in specInGame)
        {
            Destroy(window.gameObject);
        }

        specInGame.Clear();
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

        if (_simpleSort)
        {
            foreach (var item in specInGame)
            {
                item.transform.SetAsFirstSibling();
                lastSortCategory = currentSortCategory;
            }
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
                    bool hasSlot = false;
                    if (slot.HasSlotThatType(dragingItem.Type))
                    {
                        hasSlot = true;
                    }

                    if (hasSlot)
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

    public void SimpleSort()
    {
        _simpleSort = true;
        Sort(lastSortCategory);
        _simpleSort = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(specHolder);
    }

    public void RefreshGrid()
    {
        int sortBy = 1;
        if (lastSortCategory > -1)
            sortBy = lastSortCategory;

        Sort(sortBy);
       
        LayoutRebuilder.ForceRebuildLayoutImmediate(specHolder);
    }

   IEnumerator Refresh ()
    {
        yield return new WaitForSeconds(0.1f);
        SimpleSort();
    }

}
