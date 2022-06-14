using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLogPanel : MonoBehaviour
{
    //public List<BattleLog> battleLogs = new List<BattleLog>();

    [SerializeField] Text historyLog;

    [SerializeField] RectTransform textRectTransform;

    [SerializeField] Scrollbar scrollBar;

    private int index = 0;

    public void AddBattleLog(string logMessage)
    {
        historyLog.text = $" Record: {index}   {logMessage} \n{historyLog.text}";


        index++;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
        scrollBar.value = 1;
    }

    public void AddAttackBattleLog(BattleController.AttackInfo attackInfo, IClickAble activeUnit, IClickAble targetObject)
    {
        string logMessage = string.Empty;
        string roll = string.Empty;

        int i = 0;
        foreach (int rollNumber in attackInfo.dicesValueRoll)
        {
            if (i >= attackInfo.dicesValueRoll.Count - 1)
                roll += rollNumber.ToString();
            else
                roll += rollNumber.ToString() + ",";

            i++;
        }

        if (targetObject is Unit targetUnit)
            logMessage = $"{activeUnit.GetName} attacked to unit {targetUnit.GetName} with {attackInfo.dices} dices against threath {targetUnit._threat}. Roll {roll} --> {attackInfo.success} damage";
        if(targetObject is Obstacle targetObstacle && targetObject is IDamageable damagable)
        {
            logMessage = $"{activeUnit.GetName} attacked to unit {targetObstacle.GetName} with {attackInfo.dices} dices against threath {damagable.GetThreat}. Roll {roll} --> {attackInfo.success} damage";
        }
          
        historyLog.text = $" Record: {index}   {logMessage} \n{historyLog.text}";
        index++;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
        scrollBar.value = 1;
    }

}
