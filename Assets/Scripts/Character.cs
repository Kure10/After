using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEngine;

public class Character : MonoBehaviour, IResourceHolder
{

    private Command command;
    private Specialists blueprint;
    private CurrentStats curentStats;
    private IWorkSource source;
    public string State; //just pure text for now


    public bool PassedTheTest = false;

    public CurrentStats Stats { get { return this.curentStats; } }

    public Character()
    {
        Amount = new ResourceManager.ResourceAmount();
    }

    public void SetBlueprint(Specialists specialist)
    {
        blueprint = specialist;

        curentStats = new CurrentStats();
        curentStats.level = specialist.Level;
        curentStats.military = specialist.Mil;
        curentStats.science = specialist.Scl;
        curentStats.social = specialist.Sol;
        curentStats.tech = specialist.Tel;
        curentStats.karma = specialist.Kar;

    }

    public Specialists GetBlueprint()
    {
        return blueprint;
    }
    
    public Color GetColor()
    {
        return blueprint.SpecialistColor;
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

    public bool Register(IWorkSource workSource)
    {
        source?.Unregister(this);
        if (workSource.Register(this))
        {
            source = workSource;
            return true;
        }

        return false;
    }

    public ResourceManager.ResourceAmount Amount { get; set; }

    public void Set(ResourceManager.ResourceAmount amount)
    {
        Amount = amount;
    }

    public ResourceManager.ResourceAmount Add(ResourceManager.ResourceAmount amount)
    {
        Amount += amount;
        GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>().Register(this);
        return new ResourceManager.ResourceAmount();//TODO ma vracet zbytek - tzn. kdyz pridas 11 resourcu, 1 vrat, bo unese jen 10
    }

    public ResourceManager.ResourceAmount Remove(ResourceManager.ResourceAmount amount)
    {
        Amount -= amount;
        return amount; //TODO vrat skutecny pocet ziskanych resources
    }

    public void ModifiCharacterAtribute(string atribute, int value)
    {
        
        switch (atribute)
        {
            case "Stamina":
                this.blueprint.Stamina += value;
                break;
            case "Mil":
                this.blueprint.Mil += value;
                break;
            case "Sol":
                this.blueprint.Sol += value;
                break;
            case "Tel":
                this.blueprint.Tel += value;
                break;
            case "Kar":
                this.blueprint.Kar += value;
                break;
            case "Scl":
                this.blueprint.Scl += value;
                break;
            default:
                Debug.LogError("Character Error in: " + this.name);
                break;
        }
    }
}
