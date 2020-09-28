using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class uWindowSpecController : MonoBehaviour
{
    [SerializeField] private GameObject panelSpecialist;

    private List<uWindowSpecialist> activeSpecWindows = new List<uWindowSpecialist>();

    private int lastSortCategory = -1;

    public delegate void ClickAction();
    public static event ClickAction OnClicked;


    private void Awake()
    {
        var specControler = GameObject.FindGameObjectWithTag("SpecialistController").GetComponent<SpecialistControler>();

        OnClicked += specControler.AddAllSpecialistToUI;
    }

    private void OnEnable()
    {
        OnClicked();
    }

    // karma = 0
    // abecedne = 1
    // level = 2
    // health = 3

    public void AddSpecHolder(Specialists spec)
    {
        GameObject ga = Instantiate(panelSpecialist);
        ga.transform.SetParent(this.transform);
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        uWindowSpecialist uWindowSpec = ga.GetComponent<uWindowSpecialist>();
        activeSpecWindows.Add(uWindowSpec);
        uWindowSpec.SetAll(spec);
    }

    public void Sort(int currentSortCategory)
    {
        switch (currentSortCategory)
        {
            case 0:
                activeSpecWindows = activeSpecWindows.OrderBy(x => x.GetKarma).ToList();
                break;
            case 1:
                activeSpecWindows = activeSpecWindows.OrderBy(x => x.GetName).ToList();
                break;
            case 2:
                activeSpecWindows = activeSpecWindows.OrderBy(x => x.GetLevel).ToList();
                break;
            case 3:
                activeSpecWindows = activeSpecWindows.OrderBy(x => x.GetPercentHelth).ToList();
                break;
            default:
                Debug.Log("uWindowSpecControllerr Chyba. in order..");
                break;
        }

        if(lastSortCategory != currentSortCategory)
        {
            foreach (var item in activeSpecWindows)
            {
                item.transform.SetAsFirstSibling();
                lastSortCategory = currentSortCategory;
            }
        }
        else
        {
            foreach (var item in activeSpecWindows)
            {
                item.transform.SetAsLastSibling();
                lastSortCategory = -1;
            }
        }




    }
}
