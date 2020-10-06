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

    public List<Character> InGameSpecialists = new List<Character>();

    private List<Character> InMissionSpecialist = new List<Character>();


    private List<Character> OutGoingCharacters = new List<Character>();


    bool isOnRightPosition = false;

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
        foreach (Character item in list)
        {
            Character character = InGameSpecialists.Find(x => x == item);

            if (character != null)
            {
                if(OutGoingCharacters.Contains(character))
                {
                    Debug.LogError("Specialista se snazi jit na misi i kdyz uz na nejake je...");
                    continue;
                }

                OutGoingCharacters.Add(character);
               // InGameSpecialists.Remove(character);

                //ToDo move spec from map TO Mission where ever it is .. :D
                StartCoroutine(oncoroutine(character));

            }
            else
            {
                Debug.LogError("Specialista nebyl prirazen do listu inMissionSpec. Neco se stalo spatne..");
            }
        }
    }

    public void TestMove(List<Character> list)
    {
        foreach (Character item in list)
        {
            OutGoingCharacters.Add(item);
            StartCoroutine(oncoroutine(item));
        }
            
    }


    IEnumerator oncoroutine(Character character)
    {
        Vector2Int coord = new Vector2Int(5, 10);  
        var path = tileFactory.FindPath(Geometry.GridFromPoint(character.transform.position), coord);
        if (path != null)
        {
            //Move to target and if the target tile has some default action, add it to stack of actions
            //Debris is unwalkable, but for the purpose of cleaning, you can enter at first field
            if (character.TryGetComponent(out Character person))
            {
                // person.Register(this);
                person.AddCommand(new Move(character.gameObject, path));
                person.State = "Moving";
                var tmp = person.Execute();
                Debug.Log("Result : "   +  tmp);
            }
        }

        yield return new WaitUntil( () => IsSpecReadyToLeave(character) );

    }


    public bool IsSpecReadyToLeave(Character character)
    {

        if(isOnRightPosition)
        {
            InGameSpecialists.Remove(character);
            this.InMissionSpecialist.Add(character);
            return true;
        }
        else
        {
            return false;
        }

    }

    private void Update()
    {
        if(OutGoingCharacters.Count > 0)
        {
            foreach (Character character in OutGoingCharacters)
            {

                if(true ) /* character pozice je x ,y  -> je na miste kde odejde z mapy.. */
                {
                    // tady bude mit každy specialista nejakou primenou true nebo false jestli je nebo neni na pozici kdy ma odejit z mapy..
                     isOnRightPosition = true;
                }
                else
                {
                    isOnRightPosition = false;
                }


            }

        }


    }


}
