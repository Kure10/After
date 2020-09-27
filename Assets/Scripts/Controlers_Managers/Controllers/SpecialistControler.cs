using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialistControler : MonoBehaviour
{

    [SerializeField] SpecialistManager specManager;

    public List<Specialists> InGameSpecialists = new List<Specialists>();

    public List<Specialists> InMissionSpecialist = new List<Specialists>();

    public List<Specialists> GetStartingSpecialists()
    {
        List<Specialists> startingSpec = new List<Specialists>();

        startingSpec = specManager.GetStartingSpecialists();
        InGameSpecialists.AddRange(startingSpec);

        return startingSpec;
    }

}
