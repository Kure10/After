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

    public void AddAttackBattleLog(BattleController.AttackInfo attackInfo, Unit activeUnit , Unit otherUnit)
    {
        string logMessage = string.Empty;
        string roll = string.Empty;

        int i = 0;
        foreach (int rollNumber in attackInfo.dicesRoll)
        {
            if(i >= attackInfo.dicesRoll.Count -1)
                roll += rollNumber.ToString();
            else
                roll += rollNumber.ToString() +",";
            
            i++;
        }

        logMessage = $"{activeUnit._name} attacked to unit {otherUnit._name} with {attackInfo.dices} dices against threath {otherUnit._threat}. Roll {roll} --> {attackInfo.success} damage";

        historyLog.text = $" Record: {index}   {logMessage} \n{historyLog.text}";
        index++;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
        scrollBar.value = 1;
    }

}
