using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialistControler : MonoBehaviour
{
    [Space]
    [Header("Managers")]
    [SerializeField] SpecialistManager specManager;

    [Header("Character Setup")]
    [SerializeField] TileFactory tileFactory;
    [SerializeField] GameObject characterPrefab;

    [Space]
    [Header("Utility things")]
    [SerializeField]
    private uWindowSpecController specUWindowUi;

    private List<Character> InGameSpecialists = new List<Character>();

    private List<Character> InMissionSpecialist = new List<Character>();

    public void CreateStartingCharacters(Vector2Int charStartingPosition)
    {
        List<Specialists> startingSpec = new List<Specialists>();
        startingSpec = specManager.GetStartingSpecialists();

        List<Vector2Int> alreadyPlaced = new List<Vector2Int>();
        Character character = new Character();

        foreach (var specialist in startingSpec)
        {
            var gridPoint = tileFactory.FindFreeTile(charStartingPosition, alreadyPlaced).First();
            alreadyPlaced.Add(gridPoint);
            var person = Instantiate(characterPrefab, Geometry.PointFromGrid(gridPoint), Quaternion.identity);
            character = person.GetComponent<Character>();
            character.SetBlueprint(specialist);

            InGameSpecialists.Add(character);
        }
    }


    public void AddAllSpecialistToUI()
    {
        List<Character> playersGainedSpecialist = new List<Character>();

        playersGainedSpecialist.AddRange(InGameSpecialists);
        playersGainedSpecialist.AddRange(InMissionSpecialist);

        for (int i = 0; i < playersGainedSpecialist.Count; i++)
        {
            specUWindowUi.AddSpecHolder(playersGainedSpecialist[i]);
        }
    }

    public List<Character> PassSpecToMissionSelection()
    {
        return InGameSpecialists;
    }

    public void MoveSpecialistToMission(List<Character> list )
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
    }


}
