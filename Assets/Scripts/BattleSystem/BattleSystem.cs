using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleSystem
{
    static public int minimumThreat = 2;

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
            amountDices = CalcMilitary(unit);
        }
        else
        {
            amountDices = unit._military;
        }

        return amountDices;
    }

    public static int CalcMilitary(Unit unit)
    {
        int amountDices = 0;

        foreach (ItemBlueprint.BonusModificators modificator in unit.ActiveWeapon.Modificators)
        {
            if (modificator.TestModificator == ItemBlueprint.TestModificator.Battle)
            {
                if (modificator.TypeModificator == ItemBlueprint.TypeModificator.DiceCountMod)
                {
                    ItemBlueprint.MathKind type = modificator.MathKind;
                    int value = modificator.TestChangeVal;

                    switch (type)
                    {
                        case ItemBlueprint.MathKind.None:
                            break;
                        case ItemBlueprint.MathKind.plus:
                            amountDices = unit._military + value;
                            break;
                        case ItemBlueprint.MathKind.minus:
                            amountDices = unit._military - value;
                            break;
                        case ItemBlueprint.MathKind.times:
                            float floatMilitaryPower = unit._military * value;
                            int finalPower = Mathf.RoundToInt(floatMilitaryPower);
                            amountDices = finalPower;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (modificator.AtributeModificator == ItemBlueprint.AtributeModificator.MiL)
            {
                ItemBlueprint.MathKind type = modificator.MathKind;
                int value = modificator.AtributeChangeVal;

                switch (type)
                {
                    case ItemBlueprint.MathKind.None:
                        break;
                    case ItemBlueprint.MathKind.plus:
                        amountDices = unit._military + value;
                        break;
                    case ItemBlueprint.MathKind.minus:
                        amountDices = unit._military - value;
                        break;
                    case ItemBlueprint.MathKind.times:
                        float floatMilitaryPower = unit._military * value;
                        int finalPower = Mathf.RoundToInt(floatMilitaryPower);
                        amountDices = finalPower;
                        break;
                    default:
                        break;
                }
            }
        }

        return amountDices;
    }

    public static int CalculateAmountSuccess(int dices, Unit attackingUnit, Unit defendingUnit, out List<int> dicesRoll)
    {
        dicesRoll = new List<int>();
        int amountSuccess = 0;

        int finalThreat = defendingUnit._threat;

        if(attackingUnit.ActiveWeapon != null)
        {
            foreach (ItemBlueprint.BonusModificators modificator in attackingUnit.ActiveWeapon.Modificators)
            {
                if (modificator.TestModificator == ItemBlueprint.TestModificator.Battle)
                {
                    if (modificator.TypeModificator == ItemBlueprint.TypeModificator.DiffChange)
                    {
                        ItemBlueprint.MathKind type = modificator.MathKind;
                        int value = modificator.TestChangeVal;

                        switch (type)
                        {
                            case ItemBlueprint.MathKind.None:
                                break;
                            case ItemBlueprint.MathKind.plus:
                                finalThreat += value;
                                break;
                            case ItemBlueprint.MathKind.minus:
                                finalThreat -= value;
                                break;
                            case ItemBlueprint.MathKind.times:

                                float floatThreat = finalThreat * value;
                                finalThreat = Mathf.RoundToInt(floatThreat);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        if(finalThreat < 2)
        {
            finalThreat = minimumThreat;
        }

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
            var success = CalculateThrow(diceNumber, finalThreat);

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
