using System.Collections;
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

        if (unit.ActiveWeapon != null)
        {
            amountDices = unit._military + unit.ActiveWeapon.Modificators[0].AtributeChangeVal;
        }
        else
        {
            amountDices = unit._military;
        }

        return amountDices;
    }

    public static int CalculateAmountSuccess(int dices, int enemyThreat , out List<int> dicesRoll)
    {
        dicesRoll = new List<int>();
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

            dicesRoll.Add(diceNumber);
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
