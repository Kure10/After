using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreater : MonoBehaviour
{
    public GameObject CreateItemByType(ItemBlueprint blueprint, GameObject prefab)
    {
        GameObject gameObject = Instantiate(prefab);

        switch (blueprint.Type)
        {
            case ItemBlueprint.ItemType.None:
                Debug.LogError("Item has none type Error in: " + this.name + " -> item name: " + blueprint.name);
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
                Debug.LogError("Item type is not defined Error in: " + this.name + " -> item name: " + blueprint.name);
                break;
        }

        

        return gameObject;
    }

    private void CreateResource(ItemBlueprint blueprint, GameObject gameObject)
    {
        Debug.Log("Todo Resource is not implemented"); // Todo
    }

    private void CreateBackpack(ItemBlueprint blueprint, GameObject gameObject)
    {
        Backpack backpack = gameObject.AddComponent<Backpack>();
        backpack.SetupItem(blueprint.capacity, blueprint.name, backpack.Type, backpack.Sprite);
    }

    private void CreateActiveItem(ItemBlueprint blueprint, GameObject gameObject)
    {
        ActiveItem activeItem = gameObject.AddComponent<ActiveItem>();
        activeItem.SetupItem(blueprint.useCount, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);
    }

    private void CreateArmor(ItemBlueprint blueprint, GameObject gameObject)
    {
        Armor armor = gameObject.AddComponent<Armor>();
        armor.SetupItem(blueprint.absorbation, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);
    }

    private void CreateWeapon(ItemBlueprint blueprint, GameObject gameObject)
    {
        Weapon weapon = gameObject.AddComponent<Weapon>();
        weapon.SetupItem(blueprint.useCount, blueprint.isRepairable, blueprint.name, blueprint.Type, blueprint.Sprite);

        if (weapon.IsRepairable)
        {
            weapon.RepairCost = blueprint.repairCost;
            weapon.RepairBlock = blueprint.repairBlock;
        }

        weapon.RangeMin = blueprint.rangeMin;
        weapon.RangeMax = blueprint.rangeMax;
    }

}
