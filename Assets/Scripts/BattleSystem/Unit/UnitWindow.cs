using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitWindow : MonoBehaviour
{
    //[SerializeField] Text name;
    [Header("Stats")]
    [SerializeField] Text threat;
    [SerializeField] Text health;
    [SerializeField] Text range;
    [SerializeField] Text damage;
    [SerializeField] GameObject healthImageValue;

    [Space]
    [SerializeField] GameObject deadCross;

    [Header("Image")]
    [SerializeField] Image image;

    public void UpdateStats(Unit unit)
    {
        threat.text = unit._threat.ToString();

        UpdateHealthBar(unit.CurrentHealth, unit.MaxHealth);

        UpdateRange(unit);

        UpdateMilitary(unit);

        image.sprite = unit._sprite;

        UpdateAliveStatus(unit.IsDead);
    }

    public void UpdateHealthBar (int current , int max)
    {
        health.text = $"{current} / {max}";

        float amount = (float)current / (float)max;

        healthImageValue.transform.localScale = new Vector3(amount,1,1);
    }

    public void UpdateRange(Unit unit)
    {
        int rangeMax = 0;
        int rangeMin = 0;

        if (unit.ActiveWeapon == null)
        {
            rangeMax = unit._rangeMax;
            rangeMin = unit._rangeMin;
        }
        else
        {
            rangeMax = unit.ActiveWeapon.RangeMax;
            rangeMin = unit.ActiveWeapon.RangeMin;
        }

        string rangeText = $"{rangeMin}/{rangeMax}";

        if (rangeMin == rangeMax)
        {
            rangeText = rangeMax.ToString();
        }

        range.text = rangeText;
    }

    public void UpdateAliveStatus(bool isDead)
    {
        deadCross.SetActive(isDead);
    }

    private void UpdateMilitary(Unit unit)
    {
        if(unit.ActiveWeapon != null)
        {
            int military = BattleSystem.CalcMilitary(unit);
            
            damage.text = military.ToString();
        }
        else
        {
            damage.text = unit._military.ToString();
        }
    }
}
