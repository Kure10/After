using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update

    private List<Command> commands;
    void Start()
    {
        commands = new List<Command>();
    }

    // Update is called once per frame
    void Update()
    {
        if (commands.Any())
        {
            if (commands.First().Execute())
            {
                commands.RemoveAt(0);
            }
        }
    }

    public void AddCommand(Command command)
    {
        commands.Add(command);
    }

   /* public void Move(List<Vector2Int> path)
    {
        //finish this move and then get new path
        if (target.Count > 0)
        {
            var newList = new List<Vector2Int>();
            newList.Add(target[0]);
            newList.AddRange(path);
            target = newList;
            return;
        }
        accumulatedTime = 0;
        target = path;
    }*/
    
}
