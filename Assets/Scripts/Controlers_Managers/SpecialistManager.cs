using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialistManager : MonoBehaviour
{
    const string StartSpecialistText = "START"; 

    // tady bude muset byt dalsi list ktery rozlisuje default spec. Od tech so se dostanou az v brubehu hry..
    [Header("Default Specialists")]
    [SerializeField]
    private List<Specialists> allSpecialist = new List<Specialists>();

    private SpecialistXmlLoader xmlLoader;
    
    private void Awake()
    {
        this.xmlLoader = gameObject.GetComponent<SpecialistXmlLoader>();
        allSpecialist = this.xmlLoader.GetSpecialistFromXML();
    }

    public List<Specialists> GetStartingSpecialists()
    {
        List<Specialists> startingSpec = new List<Specialists>();
        startingSpec = allSpecialist.Where(s => s.Localization == StartSpecialistText).ToList();
        return startingSpec;
    }
}
