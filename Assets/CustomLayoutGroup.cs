using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;


public class CustomLayoutGroup : MonoBehaviour
{

    public Specialists spec;

    [SerializeField]
    private GameObject[] arrayOfPanels;
    private List<Specialists> specialists = new List<Specialists>();

    public void AddSpecialist(Specialists spec)
    {
        specialists.Add(spec);
        RefreshGui();
    }

    public void RemoveSpecialist(int index)
    {
        specialists.RemoveAt(index);
        RefreshGui();
    }   

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in arrayOfPanels)
        {
            item.SetActive(false);
        }

        AddSpecialist(spec);
        AddSpecialist(spec);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RefreshGui()
    {
        for (int i = 0; i < specialists.Count; i++)
        {
            arrayOfPanels[i].SetActive(true);
        }
    }
}
