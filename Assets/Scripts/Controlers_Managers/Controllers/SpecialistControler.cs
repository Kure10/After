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

    public void MoveSpecialistToMission(List<Specialists> list )
    {
        foreach (var item in list)
        {
            var spec = InGameSpecialists.Find(x => x == item);

            if (spec != null)
            {
                if(InMissionSpecialist.Contains(spec))
                {
                    Debug.LogError("Specialista se snazi jit na misi i kdyz uz na nejake je...");
                    continue;
                }

                InMissionSpecialist.Add(spec);
                InGameSpecialists.Remove(spec);

                //ToDo move spec from map TO Mission where ever it is .. :D

            }
            else
            {
                Debug.LogError("Specialista nebyl prirazen do listu inMissionSpec. Neco se stalo spatne..");
            }
        }


        /*For DeBuging ... */
        foreach (var item in InMissionSpecialist)
        {
            Debug.Log(item.FullName);
        }
        

    }


}
