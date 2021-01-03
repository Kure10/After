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
    private LifeEnergy lifeEnergy;
    private IWorkSource source;
    public string State; //just pure text for now

    private List<SpecInventorySlot> charactersSlots;

    public List<SpecInventorySlot> CharacterSlots { set { charactersSlots = value; } }

    private List<Item> inventory = new List<Item>();

    public List<Item> GetInventory { get {return this.inventory; } }

    public int AmountDicesInLastTest = 0;
    public int AmountSuccessDicesInLastTest = 0;
    public bool PassedTheTest = false;

    public CurrentStats Stats { get { return this.curentStats; } }
    public LifeEnergy LifeEnergy { get { return this.lifeEnergy; } }

    public Character()
    {
        Amount = new ResourceManager.ResourceAmount();
    }

    public string GetName()
    {
        return blueprint.FullName;
    }

    public void SetBlueprint(Specialists specialist)
    {
        blueprint = specialist;
        curentStats = new CurrentStats(specialist.Level, specialist.Mil, specialist.Sol, specialist.Tel, specialist.Kar, specialist.Scl);
        lifeEnergy = new LifeEnergy(curentStats);

    }

    public Specialists GetBlueprint()
    {
        return blueprint;
    }
    
    public Color GetColor()
    {
        return blueprint.SpecialistColor;
    }

    #region Movement Methods

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
        GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>().Register(this);
        Amount += amount;
        var surplus = ResourceManager.ReturnSurplus(Amount, 10);
        Amount -= surplus;
        return surplus;
    }

    public ResourceManager.ResourceAmount Remove(ResourceManager.ResourceAmount amount)
    {
        Amount -= amount;
        return amount; //TODO vrat skutecny pocet ziskanych resources
    }

    #endregion

    public void ModifiCharacterAtribute(string atribute, int value)
    {
        
        switch (atribute)
        {
            case "Stamina":
                LifeEnergy.CurrentStamina += value;
                break;
            case "Mil":
                Stats.military += value;
                break;
            case "Sol":
                Stats.social += value;
                break;
            case "Tel":
                Stats.tech += value;
                break;
            case "Kar":
                Stats.karma += value;
                break;
            case "Scl":
                Stats.science += value;
                break;
            default:
                Debug.LogError("Unknow Stats to c hange error in: " + this.name);
                break;
        }
    }

    // modifi current Stats.. According wearing items..
    public void OnItemChange (Item item, SpecInventorySlot specSlot)
    {
        inventory.Clear();
        foreach (SpecInventorySlot slot in charactersSlots)
        {
            inventory.Add(slot.CurrentItem.item);
        }


        //Item previousItem = GetItemFromSlotType(specSlot);

        //// pridavam item do volneho pole..
        //if (previousItem == null && item != null)
        //{
        //    inventory.Add(item);
        //}
        //else if (item == null) // odebiram item
        //{
        //    if (inventory.Contains(previousItem))
        //    {
        //        inventory.Remove(previousItem);
        //    }
        //}
        //else // menim item za jiny..
        //{
        //    if (inventory.Contains(previousItem))
        //    {
        //        inventory.Remove(previousItem);
        //        inventory.Add(item);
        //    }
        //}

        //var tmp = inventory.Count;
        //Debug.Log("Character: " + this.blueprint.FullName + "  Pocet: " + tmp);

        //foreach (Item it in inventory)
        //{
        //    if(it !=null)
        //     Debug.Log("Item: " + it.name);
        //}
        //Debug.Log("---------- ");
    }

    private Item GetItemFromSlotType(SpecInventorySlot passedSlot)
    {
        Item resultsItems = new Item();
        
        foreach (SpecInventorySlot charSlots in charactersSlots)
        {
            if (passedSlot.GetSlotType == charSlots.GetSlotType && passedSlot.GetIndex == charSlots.GetIndex)
                return passedSlot.CurrentItem.item;
        }

        return resultsItems;
    }


}
