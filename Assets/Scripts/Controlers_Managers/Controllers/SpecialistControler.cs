using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialistControler : MonoBehaviour
{
    [Space]
    [Header("Managers")]
    [SerializeField] SpecialistManager specManager;
    [SerializeField] SelectionManager selectionManager;

    [Header("Character Setup")]
    [SerializeField] TileFactory tileFactory;
    [SerializeField] GameObject characterPrefab;

    [Space]
    [Header("Utility things")]
    [SerializeField]
    private uWindowSpecController specUWindowUi;

    public delegate void OnCharacterLeaveCoreGame();
    public static OnCharacterLeaveCoreGame onCharacterLeaveCoreGame;

    public List<Character> InGameSpecialists = new List<Character>();

    private List<Character> InMissionSpecialist = new List<Character>();

    private List<Character> OutGoingCharacters = new List<Character>();


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
                OutGoingCharacters.Add(character);
                this.OrderCharactersLeave(character);
            }
            else
            {
                Debug.LogError("Specialista nebyl prirazen do listu inMissionSpec. Neco se stalo spatne..");
            }
        }
    }

    public void OrderCharactersLeave(Character character)
    {
        Vector2Int coord = new Vector2Int(5, 10);
        var path = tileFactory.FindPath(Geometry.GridFromPoint(character.transform.position), coord);
        if (path != null)
        {
            if (character.TryGetComponent(out Character person))
            {
                // person.Register(this);
                person.AddCommand(new Move(character.gameObject, path));

                person.State = "Moving To Mission";
            }
        }
    }

    private void Update()
    {

        foreach (Character character in OutGoingCharacters)
        {
            var tmp = character.Execute();
            if (tmp == Result.Success /*== "Waiting"*/)
            {
                // TodO k tomuto Eventu pak pripsat vsechno co se ma stat až odejde.......
                character.GetBlueprint().IsOnMission = true;
                onCharacterLeaveCoreGame?.Invoke();
            }

        }
    }


    //IEnumerator OnStartingLeaving(Character character)
    //{
    //    Vector2Int coord = new Vector2Int(5, 10);
    //    var path = tileFactory.FindPath(Geometry.GridFromPoint(character.transform.position), coord);
    //    if (path != null)
    //    {
    //        //Move to target and if the target tile has some default action, add it to stack of actions
    //        //Debris is unwalkable, but for the purpose of cleaning, you can enter at first field
    //        if (character.TryGetComponent(out Character person))
    //        {
    //            // person.Register(this);
    //            person.AddCommand(new Move(character.gameObject, path));
    //            selectionManager.Register(person);
    //            person.State = "Moving To Mission";

    //            //  Debug.Log("Result : "   +  tmp);
    //        }
    //    }

    //    yield return new WaitUntil(() => IsSpecReadyToLeave(character));

    //}

}
