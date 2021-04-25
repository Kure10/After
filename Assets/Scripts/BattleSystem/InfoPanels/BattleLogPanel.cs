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
    //public void AddLog(string logMessage)
    //{
    //    battleLogs[index].transform.SetAsFirstSibling();
    //    battleLogs[index].SetLog(logMessage);

    //    index++;

    //    if(index >= battleLogs.Count)
    //        index = 0;
    //}

    public void AddBattleLog(string logMessage)
    {
        historyLog.text = $" Record: {index}   {logMessage} \n{historyLog.text}";

        index++;

        LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);

        scrollBar.value = 1;
    }

}
