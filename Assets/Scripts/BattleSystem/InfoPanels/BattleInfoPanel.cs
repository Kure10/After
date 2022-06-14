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
        foreach (Unit item in InfoUnitsList)
        {
            if (selectedUnit._id == item._id)
            {
                item.IsActive = activate;
            }
        }
    }

    public void DeleteUnitFromOrder(Unit selectedUnit)
    {
        Unit unit = null;

        foreach (var item in InfoUnitsList)
        {
            if (selectedUnit._id == item._id)
            {
                unit = item;
            }
        }

        if(unit != null)
        {
            InfoUnitsList.Remove(unit);
            Destroy(unit.gameObject, 0.5f);
        }
    }

    public void UpdateUnitNewTurnOrder (List<Unit> unitsOnBattlefield, GameObject unitPrefab)
    {
        foreach (var item in InfoUnitsList)
        {
            Destroy(item.gameObject);
        }
        InfoUnitsList.Clear();

        foreach (Unit unit in unitsOnBattlefield)
        {
            if(!unit.IsDead)
            {
                var go = Instantiate(unitPrefab, container);
                var un = go.GetComponent<Unit>();
                un.InitUnit(unit);
                InfoUnitsList.Add(un);
            }
        }
    }

    public void UpdateUnitData(Unit unitToUpdate)
    {
        foreach (var unit in InfoUnitsList)
        {
            if (unitToUpdate._id == unit._id)
            {
                unit.UpdateData();
            }
        }

    }

    public void RestartDataForNewBattle()
    {
        foreach (Unit unit in InfoUnitsList)
        {
            Destroy(unit.gameObject);
        }
        InfoUnitsList.Clear();
    }

}
