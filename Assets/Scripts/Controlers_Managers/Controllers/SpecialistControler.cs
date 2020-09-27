using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialistControler : MonoBehaviour
{
    [Space]
    [Header("Managers")]
    [SerializeField] SpecialistManager specManager;

    [Space]
    [Header("Utility things")]
    [SerializeField]
    private uWindowSpecController specUWindowUi;

    private List<Specialists> InGameSpecialists = new List<Specialists>();

    private List<Specialists> InMissionSpecialist = new List<Specialists>();

    public List<Specialists> GetStartingSpecialists()
    {
        List<Specialists> startingSpec = new List<Specialists>();

        startingSpec = specManager.GetStartingSpecialists();
        InGameSpecialists.AddRange(startingSpec);

        return startingSpec;
    }


    public void AddAllSpecialistToUI()
    {
        List<Specialists> playersGainedSpecialist = new List<Specialists>();

        playersGainedSpecialist.AddRange(InGameSpecialists);
        playersGainedSpecialist.AddRange(InMissionSpecialist);

        for (int i = 0; i < playersGainedSpecialist.Count; i++)
        {
            specUWindowUi.AddSpecHolder(playersGainedSpecialist[i]);
        }
    }

    public List<Specialists> PassSpecToMissionSelection()
    {
        return InGameSpecialists;
    }

}
