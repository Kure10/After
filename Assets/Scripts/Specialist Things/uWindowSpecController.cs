using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class uWindowSpecController : MonoBehaviour
{

    [SerializeField] private GameObject panelSpecialist;

    public void AddSpecHolder(Specialists spec)
    {
        GameObject ga = Instantiate(panelSpecialist);
        ga.transform.parent = this.transform;
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        uWindowSpecialist uWindowSpec = ga.GetComponent<uWindowSpecialist>();
        uWindowSpec.SetAll(spec);
    }

}
