using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class SpecGridUI : MonoBehaviour
{

    List<Specialists> specList = new List<Specialists>();

    [SerializeField] private GameObject panelSpecialist;

    // for testing
    public Specialists spec;
    public Specialists spec1;

   // public SpecSetGui ssg;

    // Start is called before the first frame update
    void Start()
    {
        AddToSpecialistsList(spec);
        AddToSpecialistsList(spec1);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void AddToSpecialistsList (Specialists spec)
    {
        specList.Add(spec);
        AddSpecHolder(spec);
    }

    public void AddSpecHolder(Specialists spec)
    {
        GameObject ga = Instantiate(panelSpecialist);
        ga.transform.parent = this.transform;
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        SpecSetGui ssg = ga.GetComponent<SpecSetGui>();
        ssg.SetUp();
        ssg.SetAll(spec);
    }


}
