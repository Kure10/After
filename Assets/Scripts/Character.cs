using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update

    private Command command;
    private Specialists blueprint;

    public void SetBlueprint(Specialists specialist)
    {
        blueprint = specialist;
    }

    //TODO prozatim - tohle je treba udelat nejak rozumne, ale k tomu je treba pohrabat se v Specialists.cs
    public int GetTechLevel()
    {
        return 1;
        return blueprint.Tel; //TODO hmmm, nejak mizi blueprint,nevim proc
    }
    public Result Execute()
    {
        if (command != null)
        {
            return command.Execute();
        }

        return Result.Failure;
    }
    
    public Command GetCommand()
    {
        return command;
    }
    

    public void AddCommand(Command command)
    {
        this.command = command;
    }

}
