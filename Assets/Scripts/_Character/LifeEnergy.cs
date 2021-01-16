using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeEnergy
{
    private float maxLife;
    private float maxStamina;

    private float currentLife = 1;
    private float currentStamina = 1;


    public LifeEnergy(CurrentStats stats)
    {
        maxLife = LifeEnergy.CalcHealth(stats);
        maxStamina = LifeEnergy.CalcStamina(stats);

        RestroToMax(true);
        RestroToMax();
    }

    public float CurrentLife
    {
        get { return this.currentLife; }
        set
        {
            currentLife += value;
            if (currentLife > maxLife)
                currentLife = maxLife;
            if (currentLife < 0)
                Debug.Log("Character is Dead");
        }
    }
    public float CurrentStamina
    {
        get { return this.currentStamina; }
        set
        {
            currentStamina += value;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
            if (currentStamina <= 0)
                Debug.Log("Character is out of Stamina -> Probably dead");
        }
    }

    static private float CalcStamina(CurrentStats stats)
    {
        return 57600 + 2400 * stats.level;
    }

    static private float CalcHealth(CurrentStats stats)
    {
        return 40 + stats.level + 4 * stats.military + 2 * stats.tech + stats.science + stats.social;
    }

    private void RestroToMax(bool restoreStamina = false)
    {
        if (restoreStamina)
            currentStamina = maxStamina;
        else
            currentLife = maxLife;

    }

    public void RecalcLifeEnergy(CurrentStats stats)
    {
        maxLife = LifeEnergy.CalcHealth(stats);
        maxStamina = LifeEnergy.CalcStamina(stats);
    }

    public float PercentHealth
    {
        get
        {
            return ((float)currentLife / (float)maxLife) * 100;
        }
    }
    public float PercentStamina
    {
        get
        {
            return ((float)currentStamina / (float)maxStamina) * 100;
        }
    }
}
