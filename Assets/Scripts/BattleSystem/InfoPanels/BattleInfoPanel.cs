using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfoPanel : MonoBehaviour
{
    [SerializeField] public Transform container;

    public List<Unit> InfoUnitsList = new List<Unit>();

    public void InitStartOrder(List<Unit> unitsOnBattlefield, GameObject unitPrefab)
    {
        foreach (Unit unit in unitsOnBattlefield)
        {
            var go = Instantiate(unitPrefab, container);
            var un = go.GetComponent<Unit>();
            un.InitUnit(unit);
            InfoUnitsList.Add(un);
        }
    }

    public void UpdateUnitOrder(Unit selectedUnit, bool activate)
    {
        foreach (var item in InfoUnitsList)
        {
            if (selectedUnit._id == item._id)
            {
                item.IsActive = activate;
            }
        }
    }
}
