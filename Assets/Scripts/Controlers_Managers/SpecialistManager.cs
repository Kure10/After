using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialistManager : MonoBehaviour
{
    [Header("Default Specialists")]
    [SerializeField]
    private List<Specialists> defaultSpecialists = new List<Specialists>();

    [Space]
    [Header("Utility things")]
    [SerializeField]
    private uWindowSpecController specUWindowUi;


   // private static List<Specialists> inGameSpecialists = new List<Specialists>();

  //  public static List<Specialists> GetInGameSpecialists { get { return SpecialistManager.inGameSpecialists; }  }

    private void Start()
    {
        AddAllSpecialistToUI();
    }

    public void AddAllSpecialistToUI()
    {
        for (int i = 0; i < defaultSpecialists.Count; i++)
        {
            specUWindowUi.AddSpecHolder(defaultSpecialists[i]);
            AddInGameSpecialist(defaultSpecialists[i]);
        }
    }

    public List<Specialists> GetSpecialists()
    {
        return defaultSpecialists;
    }

    private void AddInGameSpecialist(Specialists spec)
    {
   //     inGameSpecialists.Add(spec);
    }

    //public void SortList()
    //{
    //    SpecialistManager.inGameSpecialist = inGameSpecialist.OrderByDescending(x => x.Kar).ToList();

    //    foreach (var item in SpecialistManager.inGameSpecialist)
    //    {
    //        Debug.Log(item.Kar);
    //    }
    //}


}
