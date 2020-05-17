using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialistManager : MonoBehaviour
{
    [Header("Default Specialists")]
    [SerializeField]
    private List<Specialists> specialists = new List<Specialists>();

    [Space]
    [Header("Utility things")]
    [SerializeField]
    private uWindowSpecController specUWindowUi;

    private void Start()
    {
        AddAllSpecialistToUI();
    }

    public void AddAllSpecialistToUI()
    {
        for (int i = 0; i < specialists.Count; i++)
        {
            specUWindowUi.AddSpecHolder(specialists[i]);
        }
    }

    public List<Specialists> GetSpecialists()
    {
        return specialists;
    }
    
    


}
