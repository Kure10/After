using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialistControler : MonoBehaviour
{
    [Header("Default Specialists")]
    [SerializeField]
    private List<Specialists> specialists = new List<Specialists>();

    [Space]
    [Header("Utility things")]
    [SerializeField]
    private SpecGridUI specGridUI;

    private void Start()
    {
        AddAllSpecialistToUI();
    }

    public void AddAllSpecialistToUI()
    {
        for (int i = 0; i < specialists.Count; i++)
        {
            specGridUI.AddSpecHolder(specialists[i]);
        }
    }


}
