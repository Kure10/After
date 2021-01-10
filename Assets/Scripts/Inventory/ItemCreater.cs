using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            default:
                Debug.LogWarning("Item type is not defined Error in: " + this.name + " -> item name: " + blueprint.name);
                break;
        }

        return gameObject;
    }

    private void CreateResource(ItemBlueprint blueprint, GameObject gameObject)
    {
        Debug.LogWarning("Todo Resource is not implemented"); // Todo
    }

    private void CreateBackpack(ItemBlueprint blueprint, GameObject gameObject)
    {
        Backpack backpack = gameObject.AddComponent<Backpack>();
        backpack.SetupItem(blueprint.capacity, blueprint.name, blueprint.Type, blueprint.Sprite);

        AddModifications(blueprint, backpack);
    }

    private void CreateActiveItem(ItemBlueprint blueprint, GameObject gameObject)
    {
        ActiveItem activeItem = gameObject.AddComponent<ActiveItem>();
        activeItem.SetupItem(blueprint.useCount, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);

        AddModifications(blueprint, activeItem);
    }

    private void CreateArmor(ItemBlueprint blueprint, GameObject gameObject)
    {
        Armor armor = gameObject.AddComponent<Armor>();
        armor.SetupItem(blueprint.absorbation, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);

        AddModifications(blueprint, armor);
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

    private void AddModifications(ItemBlueprint blueprint, Item item)
    {
        item.Modificators = blueprint.modificators;
    }

}
