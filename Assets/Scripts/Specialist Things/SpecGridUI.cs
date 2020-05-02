using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecGridUI : MonoBehaviour
{

    [SerializeField] private GameObject panelSpecialist;


    public void AddToSpecialistsList (Specialists spec)
    {
        
        AddSpecHolder(spec);
    }

    public void AddSpecHolder(Specialists spec)
    {
        GameObject ga = Instantiate(panelSpecialist);
        ga.transform.parent = this.transform;
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        uWindowSpecialist ssg = ga.GetComponent<uWindowSpecialist>();
        ssg.SetAll(spec);
    }

}
