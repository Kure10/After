﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleSystem
{
    public static int CalculateAmountDices(Character character)
    {
        int amountDices = 0;
        amountDices += character.Stats.military;
        return amountDices;
    }

    public static int CalculateAmountDices(Unit unit)
    {
        int amountDices = 0;
        amountDices += unit._damage;
        return amountDices;
    }

    public static int CalculateAmountSuccess(int dices, int enemyThreat)
    {
        int amountSuccess = 0;

        for (int i = 0; i < dices; i++)
        {
            int diceNumber = Random.Range(1, 7); // Todo Lepsi random

            while (true)
            {
                if (diceNumber % 6 == 0)
                    diceNumber += Random.Range(1, 7); // Todo Lepsi random
                else
                    break;
            }

            var success = CalculateThrow(diceNumber, enemyThreat);

            if (success)
                amountSuccess++;
            
        }

        return amountSuccess;
    }

    private static bool CalculateThrow (int diceNumber , int peakNumber)
    {
        var result = false;
        result = diceNumber >= peakNumber;
        return result;
    }

}
