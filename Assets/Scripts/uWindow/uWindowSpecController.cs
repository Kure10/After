using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;


public class uWindowSpecController : MonoBehaviour
{
  
    [Header("Prefabs")]
    [SerializeField] private GameObject panelSpecialist;

    [Header("Holder")]
    [SerializeField] private Transform specHolder;
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

    public void AddSpecHolder(Character character)
    {
        GameObject ga = Instantiate(panelSpecialist);
        ga.transform.SetParent(specHolder);
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        uWindowSpecialist uWindowSpec = ga.GetComponent<uWindowSpecialist>();
        uWindowSpec.CharacterInWindow = character;
        specInGame.Add(uWindowSpec);
        uWindowSpec.SetAll(character);

        var slots = uWindowSpec.GetCharacterSlots();
        character.CharacterSlots = slots;
        foreach (SpecInventorySlot slot in slots)
        {
            slot.OnItemChangeCallBack += character.OnItemChange;
            DragAndDropManager.Instantion.OnItemResponceAnimation += OnItemDragResponce;
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

        if(lastSortCategory != currentSortCategory)
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
        if(DragAndDropManager.IsDraging)
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

}
