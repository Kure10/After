using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialistManager : MonoBehaviour
{
    // tady bude muset byt dalsi list ktery rozlisuje default spec. Od tech so se dostanou az v brubehu hry..
    [Header("Default Specialists")]
    [SerializeField]
    private List<Specialists> defaultSpecialists = new List<Specialists>();


    [Space]
    [Header("Utility things")]
    [SerializeField]
    private uWindowSpecController specUWindowUi;

    private SpecialistXmlLoader xmlLoader;
    
    private void Awake()
    {
        this.xmlLoader = gameObject.GetComponent<SpecialistXmlLoader>();
        defaultSpecialists = this.xmlLoader.GetSpecialistFromXML();

    }
    private void Start()
    {
        AddAllSpecialistToUI();
    }

    public void AddAllSpecialistToUI()
    {
        for (int i = 0; i < defaultSpecialists.Count; i++)
        {
            specUWindowUi.AddSpecHolder(defaultSpecialists[i]);
        }
    }

    public List<Specialists> GetSpecialists()
    {
        return defaultSpecialists;
    }

}
