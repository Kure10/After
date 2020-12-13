using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class uWindowSpecController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [Header("Prefabs")]
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private GameObject panelSpecialist;

    [Header("Holder")]
    [SerializeField] private Transform specHolder;
    [SerializeField] private Transform itemHolder;



    private int _inventorzSize = 70;


    private List<uWindowSpecialist> specInGame = new List<uWindowSpecialist>();

    private List<ItemSlot> itemSlots = new List<ItemSlot>();

    private int lastSortCategory = -1;

    public delegate void ClickAction();
    public static event ClickAction OnClicked;


    private void Awake()
    {
        SpecialistControler specControler = GameObject.FindGameObjectWithTag("SpecialistController").GetComponent<SpecialistControler>();

        OnClicked += this.ClearPreviousCharacters;
        OnClicked += specControler.AddAllSpecialistToUI;

        CreateSlots();
    }

    private void CreateSlots ()
    {
        // vytvořím sloty podle velikosti inventare pro itemy.. nastavím jím holder a pridam je do listu vsech slotu..
        for (int i = 0; i < _inventorzSize; i++)
        {
            GameObject slot = Instantiate(this.itemSlot);
            slot.transform.SetParent(itemHolder);
            slot.transform.localScale = new Vector3(1f, 1f, 1f);
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.SetEmpty();
            itemSlots.Add(itemSlot);
        }
    }

    private void ClearPreviousCharacters()
    {
        foreach (var item in specInGame)
        {
            Destroy(item.transform.gameObject);
        }

        specInGame.Clear();
    }

    private void OnEnable()
    {
        OnClicked();
        UpdateInventory();
    }

    // karma = 0
    // abecedne = 1
    // level = 2
    // health = 3
    public void UpdateInventory()
    {
        var listOfItems = inventory.collectedItems;

        // vytvořím item a naplním ho daty , a nastavím jednotlivym slotum ITEM.
        for (int i = 0; i < listOfItems.Count; i++)
        {
            ItemBlueprint itemBlueprint = listOfItems[i];
            GameObject goItem = Instantiate(inventory.itemPrefab);
            Item item = goItem.GetComponent<Item>();
            item.Blueprint = itemBlueprint;
            item.Sprite = itemBlueprint.Sprite;
            item.MySlot = itemSlots[i];
            itemSlots[i].AddItem(goItem,item);
        }
    }


    public void AddSpecHolder(Character character)
    {
        GameObject ga = Instantiate(panelSpecialist);
        ga.transform.SetParent(specHolder);
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        uWindowSpecialist uWindowSpec = ga.GetComponent<uWindowSpecialist>();
        specInGame.Add(uWindowSpec);
        uWindowSpec.SetAll(character);
    }

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
}
