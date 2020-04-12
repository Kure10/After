using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update

    private Command command;
    private Specialists blueprint;
    private IWorkSource source;

    public void SetBlueprint(Specialists specialist)
    {
        blueprint = specialist;
    }

    //TODO prozatim - tohle je treba udelat nejak rozumne, ale k tomu je treba pohrabat se v Specialists.cs
    public int GetTechLevel()
    {
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

    public void Register(IWorkSource workSource)
    {
        source?.Unregister(this);
        workSource.Register(this);
        source = workSource;
    }
}
