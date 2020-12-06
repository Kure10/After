using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class uWindowSpecController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private GameObject panelSpecialist;

    [Header("Holder")]
    [SerializeField] private Transform specHolder;
    [SerializeField] private Transform itemHolder;





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
    }

    // karma = 0
    // abecedne = 1
    // level = 2
    // health = 3
    public void AddItem(Item item)
    {
        GameObject ga = Instantiate(itemSlot);
        ga.transform.SetParent(itemHolder);
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        ItemSlot slot = ga.GetComponent<ItemSlot>();
        slot.SetAll(item);
        itemSlots.Add(slot);
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
