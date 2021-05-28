using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCreater : MonoBehaviour
{
    public GameObject CreateItemByType(ItemBlueprint blueprint, GameObject prefab)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.name = "Item_ " + blueprint.name;

        switch (blueprint.Type)
        {
            case ItemBlueprint.ItemType.None:
                Debug.LogWarning("Item has none type Error in: ItemCreater  -> item name: " + blueprint.name);
                break;
            case ItemBlueprint.ItemType.ArmorSpec:
                CreateArmor(blueprint, gameObject);
                break;
            case ItemBlueprint.ItemType.BagSpec:
                CreateBackpack(blueprint, gameObject);
                break;
            case ItemBlueprint.ItemType.ItemSpec:
                CreateActiveItem(blueprint, gameObject);
                break;
            case ItemBlueprint.ItemType.ResBasic:
                CreateResource(blueprint, gameObject);
                break;
            case ItemBlueprint.ItemType.WeapSpec:
                CreateWeapon(blueprint, gameObject);
                break;
            case ItemBlueprint.ItemType.ResSpecial:
                CreateResourceSpecial(blueprint, gameObject);
                break;
            default:
                Debug.LogWarning("Item type is not defined Error in: " + this.name + " -> item name: " + blueprint.name);
                break;
        }


        return gameObject;
    }

    public GameObject CreateItemFromItem(Item item, GameObject prefab)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.name = item.name;

        if (item is Backpack backpack)
        {
            CreateBackpack(backpack, gameObject);
        }
        else if (item is Weapon weapon)
        {
            CreateWeapon(weapon, gameObject);
        }
        else if (item is Armor armor)
        {
            CreateArmor(armor, gameObject);
        }
        else if (item is ActiveItem activeItem)
        {
            CreateActiveItem(activeItem, gameObject);
        }
        else
        {
            CreateResource(item, gameObject);

           // CreateResourceSpecial(blueprint, gameObject);
        }

        return gameObject;
    }

    private void CreateResourceSpecial(ItemBlueprint blueprint, GameObject gameObject)
    {
        Debug.LogWarning("Todo ResourceSpecial is not implemented"); // Todo
    }

    private void CreateResource(ItemBlueprint blueprint, GameObject gameObject)
    {
        Debug.LogWarning("Todo Resource is not implemented"); // Todo
    }

    private void CreateResource(Item item, GameObject gameObject)
    {
        Debug.LogWarning("Todo Resource is not implemented"); // Todo
    }

    private void CreateBackpack(ItemBlueprint blueprint, GameObject gameObject)
    {
        Backpack backpack = gameObject.AddComponent<Backpack>();
        backpack.SetupItem(blueprint.capacity, blueprint.name, blueprint.Type, blueprint.Sprite);

        AddModifications(blueprint, backpack);
    }

    private void CreateBackpack(Backpack _backpack, GameObject gameObject)
    {
        Backpack backpack = gameObject.AddComponent<Backpack>();
        backpack.SetupItem(_backpack.Capacity, _backpack.name, _backpack.Type, _backpack.Sprite);

        AddModifications(_backpack, backpack);
    }

    private void CreateActiveItem(ItemBlueprint blueprint, GameObject gameObject)
    {
        ActiveItem activeItem = gameObject.AddComponent<ActiveItem>();
        activeItem.SetupItem(blueprint.useCount, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);

        AddModifications(blueprint, activeItem);
    }

    private void CreateActiveItem(ActiveItem _activeItem, GameObject gameObject)
    {
        ActiveItem activeItem = gameObject.AddComponent<ActiveItem>();
        activeItem.SetupItem(_activeItem.UseCount, _activeItem.IsRepairable, _activeItem.Name, _activeItem.Type, _activeItem.Sprite);

        AddModifications(_activeItem, activeItem);
    }

    private void CreateArmor(ItemBlueprint blueprint, GameObject gameObject)
    {
        Armor armor = gameObject.AddComponent<Armor>();
        armor.SetupItem(blueprint.absorbation, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);

        AddModifications(blueprint, armor);
    }

    private void CreateArmor(Armor _armor, GameObject gameObject)
    {
        Armor armor = gameObject.AddComponent<Armor>();
        armor.SetupItem(_armor.Absorbation, _armor.IsRepairable, _armor.name, _armor.Type, _armor.Sprite);

        AddModifications(_armor, armor);
    }

    private void CreateWeapon(ItemBlueprint blueprint, GameObject gameObject)
    {
        Weapon weapon = gameObject.AddComponent<Weapon>();
        weapon.SetupItem(blueprint.useCount, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);

        weapon.IsIndestructible = blueprint.isIndestructible;

        if (weapon.IsRepairable)
        {
            weapon.RepairCost = blueprint.repairCost;
            weapon.RepairBlock = blueprint.repairBlock;
        }

        weapon.RangeMin = blueprint.rangeMin;
        weapon.RangeMax = blueprint.rangeMax;

        AddModifications(blueprint, weapon);
    }

    private void CreateWeapon(Weapon _weapon, GameObject gameObject)
    {
        Weapon weapon = gameObject.AddComponent<Weapon>();
        weapon.SetupItem(_weapon.UseCount, _weapon.IsRepairable, _weapon.name, _weapon.Type, _weapon.Sprite);

        weapon.IsIndestructible = _weapon.IsIndestructible;

        if (weapon.IsRepairable)
        {
            weapon.RepairCost = _weapon.RepairCost;
            weapon.RepairBlock = _weapon.RepairBlock;
        }

        weapon.RangeMin = _weapon.RangeMin;
        weapon.RangeMax = _weapon.RangeMax;

        AddModifications(_weapon, weapon);
    }

    private void AddModifications(ItemBlueprint blueprint, Item item)
    {
        item.Modificators = blueprint.modificators;
    }

    private void AddModifications(Item item, Item itemToModifi)
    {
        itemToModifi.Modificators = item.Modificators;
    }
}
