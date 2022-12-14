using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEngine;

public class Character : MonoBehaviour, IResourceHolder
{

    private Command command;
    private Specialists blueprint;
    private CurrentStats curentStats; // staty charackteru
    private CurrentStats _finalStats; // staty s itemama ?  mozna to udelam jinak ?
    private LifeEnergy lifeEnergy;
    private IWorkSource source;
    public string State; //just pure text for now

    private List<SpecInventorySlot> charactersSlots;

    private List<SpecInventorySlot> backPackSlots;

    public List<SpecInventorySlot> CharacterSlots { get { return charactersSlots; } set { charactersSlots = value; } }

    public List<SpecInventorySlot> CharacterBackPackSlots { get { return backPackSlots; } set { backPackSlots = value; } }

    public List<Item> GetInventory 
    { 
        get 
        {
            List<Item> inventory = new List<Item>();

            foreach (SpecInventorySlot item in charactersSlots)
                inventory.Add(item.CurrentItem.item);
            
            return inventory; 
        } 
    }
    public List<Item> GetBackpackInventory
    {
        get
        {
            List<Item> inventory = new List<Item>();

            foreach (SpecInventorySlot item in backPackSlots)
                inventory.Add(item.CurrentItem.item);

            return inventory;
        }
    }

    public Backpack GetCharacterBackpack
    {
        get
        {
            Backpack backpack = null;

            foreach (SpecInventorySlot slot in charactersSlots)
            {
                if(slot != null && slot.CurrentItem.item is Backpack back)
                {
                    backpack = back;
                    break;
                }
            }

            return backpack;
        }
    }

    public Weapon GetCharacterWeapon (bool forLeftHand)
    {
        Weapon weapon = null;

        foreach (SpecInventorySlot slot in charactersSlots)
        {
            if(forLeftHand)
            {
                if (slot.GetBodyPart == SpecInventorySlot.BodyPart.LeftHand)
                {
                    if (slot != null && slot.CurrentItem.item is Weapon weapon1)
                    {
                        weapon = weapon1;
                    }
                }
            }
            else
            {
                if (slot.GetBodyPart == SpecInventorySlot.BodyPart.RightHand)
                {
                    if (slot != null && slot.CurrentItem.item is Weapon weapon1)
                    {
                        weapon = weapon1;
                    }
                }
            }
        }

        return weapon;
    }

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

    public void Initialized(Specialists specialist)
    {
        blueprint = new Specialists();
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

    public float ModifyStamina(float value)
    {
        lifeEnergy.CurrentStamina = value;
        return lifeEnergy.CurrentStamina;
    }

    public void ModifyLife(float value)
    {
        lifeEnergy.CurrentLife = value;
    }

    public void RecalculateFinalStats()
    {
        _finalStats = Stats;
        // finalni staty + Itemy v inventari..

        //foreach (var item in _inventory)
        //{
        //    //if(item is Weapon weapon)
        //    //{
        //    //    weapon
        //    //}
        //    //else if (item is Armor armor)
        //    //{

        //    //}
        //    //else if (item is Backpack backpack)
        //    //{

        //    //}
        //    //else if (item is ActiveItem activeItem)
        //    //{

        //    //}
        //    //else
        //    //{

        //    //}
        //}
    }



    public void ModifiCharacterAtribute(string atribute, int value)
    {
        
        switch (atribute)
        {
            case "Stamina":
                LifeEnergy.CurrentStamina = value;
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
    public void OnItemChange(Item item, GameObject itemGo , SpecInventorySlot specSlot)
    {
        SpecInventorySlot slot;

        if (specSlot.IsBackpack)
        {
            slot = backPackSlots[specSlot.GetIndex];
        }
        else
        {
            slot = charactersSlots[specSlot.GetIndex];
        }

        //add item
        if (item != null)
        {
            slot.SerCurrentItemWithoutNotify = (item, itemGo);

        }
        else // remove item
        {
            slot.SerCurrentItemWithoutNotify = (null, null);
        }

        // možna nastavic item slot atd.

    }
}
