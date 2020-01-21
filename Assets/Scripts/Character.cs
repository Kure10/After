using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update

    private Command command;
    void Start()
    {
    }
    


    public Result Execute()
    {
        if (command != null)
        {
            return command.Execute();
        }

        return Result.Failure;
    }

    public void AddCommand(Command command)
    {
        this.command = command;
    }

}
