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

        UpdateRange(unit._rangeMax);

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

    public void UpdateRange(int range)
    {
        if(range <= 1)
        {
            this.range.text = "M";
        }
        else
        {
            this.range.text = range.ToString();
        }
    }

    public void UpdateAliveStatus(bool isDead)
    {
        deadCross.SetActive(isDead);
    }

    private void UpdateMilitary(Unit unit)
    {
        if(unit.ActiveWeapon != null)
        {
            int military = unit.ActiveWeapon.Modificators[0].TestChangeVal + unit._military;
            damage.text = military.ToString();
        }
        else
        {
            damage.text = unit._military.ToString();
        }
    }
}
