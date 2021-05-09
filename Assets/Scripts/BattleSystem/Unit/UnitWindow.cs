﻿using System.Collections;
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

    [Header("Image")]
    [SerializeField] Image image;

    public void UpdateStats(Unit unit)
    {
        threat.text = unit._threat.ToString();

        health.text = unit.MaxHealth.ToString();

        UpdateRange(unit._rangeMax);

        damage.text = unit._damage.ToString();

        image.sprite = unit._sprite;
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


}
